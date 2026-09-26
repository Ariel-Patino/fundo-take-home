using System.Net;
using System.Net.Http.Json;
using Fundo.LoanEngine.Application.Messaging;
using Fundo.LoanEngine.Domain.Entities;
using Fundo.LoanEngine.Infrastructure.Persistence.Repositories;
using Fundo.LoanEngine.Infrastructure.Time;
using Fundo.LoanEngine.WebApi.Contracts;
using Fundo.LoanEngine.Infrastructure.Outbox;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Xunit;
using ApplicationEntity = Fundo.LoanEngine.Domain.Entities.Application;

namespace Fundo.LoanEngine.IntegrationTests;

[Collection(PostgreSqlCollection.Name)]
public sealed class ApplicationIntegrationTests(PostgreSqlFixture database)
{
    [Fact]
    public async Task CommitTransactionWithEvent_WhenDeferredOutboxConstraintFails_RollsBackAllRecords()
    {
        await database.ResetDatabaseAsync();
        await CreateDeferredOutboxFailureAsync();

        try
        {
            await using (var context = database.CreateDbContext())
            {
                var repository = new ApplicationRepository(context, new SystemClock());
                var customer = Customer.Create(
                    "Jordan",
                    "Lee",
                    "123 Main Street",
                    "CA",
                    "Acme",
                    "555-55-0101",
                    DateTimeOffset.UtcNow);
                var application = ApplicationEntity.Create(customer.Id, 5000, DateTimeOffset.UtcNow);
                customer.AttachApplication(application);

                repository.AddCustomer(customer);
                repository.AddApplication(application);

                var submittedEvent = new ApplicationSubmittedEvent(
                    customer.Id,
                    application.Id,
                    customer.FirstName,
                    customer.LastName,
                    customer.Address,
                    customer.State,
                    customer.CompanyName,
                    application.RequestedAmount,
                    "555-55-0101",
                    false);

                await Assert.ThrowsAsync<PostgresException>(
                    () => repository.CommitTransactionWithEventAsync(submittedEvent));
            }

            await using var verificationContext = database.CreateDbContext();
            Assert.Equal(0, await verificationContext.Customers.CountAsync());
            Assert.Equal(0, await verificationContext.Applications.CountAsync());
            Assert.Equal(0, await verificationContext.OutboxMessages.CountAsync());
        }
        finally
        {
            await DropDeferredOutboxFailureAsync();
        }
    }

    [Fact]
    public async Task PostApplications_WhenApplicationIsApproved_Returns200AndPersistsAggregateAndOutbox()
    {
        await database.ResetDatabaseAsync();
        using var api = CreateApiClient();

        using var response = await api.Client.PostAsJsonAsync("/api/applications", ValidRequest("CA", "555-55-0102"));
        var responseBody = await response.Content.ReadFromJsonAsync<SubmitApplicationResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseBody);
        Assert.Equal("Approved", responseBody.Status);
        Assert.NotNull(responseBody.CustomerId);
        Assert.NotNull(responseBody.ApplicationId);

        await using var context = database.CreateDbContext();
        Assert.Equal(1, await context.Customers.CountAsync());
        Assert.Equal(1, await context.Applications.CountAsync());
        Assert.Equal(1, await context.OutboxMessages.CountAsync());
    }

    [Fact]
    public async Task PostApplications_WhenStateIsBlocked_Returns422WithoutPersistence()
    {
        await database.ResetDatabaseAsync();
        using var api = CreateApiClient();

        using var response = await api.Client.PostAsJsonAsync("/api/applications", ValidRequest("NY", "555-55-0103"));
        var responseBody = await response.Content.ReadFromJsonAsync<SubmitApplicationResponse>();

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(responseBody);
        Assert.Equal("Denied", responseBody.Status);
        Assert.Contains("New York", responseBody.Reason);

        await using var context = database.CreateDbContext();
        Assert.Equal(0, await context.Customers.CountAsync());
        Assert.Equal(0, await context.Applications.CountAsync());
        Assert.Equal(0, await context.OutboxMessages.CountAsync());
    }

    [Fact]
    public async Task PostApplications_WhenSsnIsInvalid_Returns400WithoutPersistence()
    {
        await database.ResetDatabaseAsync();
        using var api = CreateApiClient();
        var request = ValidRequest("CA", "555-55-010X");

        using var response = await api.Client.PostAsJsonAsync("/api/applications", request);
        var responseBody = await response.Content.ReadFromJsonAsync<SubmitApplicationResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(responseBody);
        Assert.Equal("Invalid", responseBody.Status);
        Assert.Contains("nine digits", responseBody.Reason);

        await using var context = database.CreateDbContext();
        Assert.Equal(0, await context.Customers.CountAsync());
        Assert.Equal(0, await context.Applications.CountAsync());
        Assert.Equal(0, await context.OutboxMessages.CountAsync());
    }

    private ApiClientScope CreateApiClient()
    {
        var originalConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
        var originalMockBaseUrl = Environment.GetEnvironmentVariable("ExternalMockService__BaseUrl");
        var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                var outboxService = services.FirstOrDefault(descriptor =>
                    descriptor.ServiceType == typeof(IHostedService) &&
                    descriptor.ImplementationType == typeof(OutboxBackgroundService));
                if (outboxService is not null)
                {
                    services.Remove(outboxService);
                }
            });
        });

        try
        {
            Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", database.ConnectionString);
            Environment.SetEnvironmentVariable("ExternalMockService__BaseUrl", "http://127.0.0.1:1");
            return new ApiClientScope(factory, factory.CreateClient());
        }
        catch
        {
            factory.Dispose();
            throw;
        }
        finally
        {
            Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", originalConnectionString);
            Environment.SetEnvironmentVariable("ExternalMockService__BaseUrl", originalMockBaseUrl);
        }
    }

    private sealed class ApiClientScope(
        WebApplicationFactory<Program> factory,
        HttpClient client) : IDisposable
    {
        public HttpClient Client { get; } = client;

        public void Dispose()
        {
            Client.Dispose();
            factory.Dispose();
        }
    }

    private async Task CreateDeferredOutboxFailureAsync()
    {
        await using var connection = new NpgsqlConnection(database.ConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            """
            CREATE OR REPLACE FUNCTION reject_outbox_commit() RETURNS trigger
            LANGUAGE plpgsql AS $$
            BEGIN
                RAISE EXCEPTION 'forced deferred outbox failure';
            END;
            $$;
            CREATE CONSTRAINT TRIGGER reject_outbox_commit_trigger
            AFTER INSERT ON outbox_messages
            DEFERRABLE INITIALLY DEFERRED
            FOR EACH ROW EXECUTE FUNCTION reject_outbox_commit();
            """,
            connection);
        await command.ExecuteNonQueryAsync();
    }

    private async Task DropDeferredOutboxFailureAsync()
    {
        await using var connection = new NpgsqlConnection(database.ConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            "DROP TRIGGER IF EXISTS reject_outbox_commit_trigger ON outbox_messages; DROP FUNCTION IF EXISTS reject_outbox_commit();",
            connection);
        await command.ExecuteNonQueryAsync();
    }

    private static object ValidRequest(string state, string ssn) => new
    {
        firstName = "Jordan",
        lastName = "Lee",
        address = "123 Main Street",
        state,
        companyName = "Acme",
        requestedAmount = 5000m,
        ssn,
    };
}
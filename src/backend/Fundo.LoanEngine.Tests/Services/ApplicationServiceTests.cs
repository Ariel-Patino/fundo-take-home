using Fundo.LoanEngine.Application.Applications.SubmitApplication;
using Fundo.LoanEngine.Application.Interfaces;
using Fundo.LoanEngine.Application.Messaging;
using Fundo.LoanEngine.Application.Rules;
using Fundo.LoanEngine.Domain.Entities;
using ApplicationEntity = Fundo.LoanEngine.Domain.Entities.Application;
using Xunit;

namespace Fundo.LoanEngine.Tests.Services;

public sealed class ApplicationServiceTests
{
    [Fact]
    public async Task Handle_WhenRuleDeniesApplication_ReturnsReasonWithoutPersisting()
    {
        var repository = new InMemoryApplicationRepository();
        var ruleEngine = new RuleEngine([new StateDenyRule(["NY"])]);
        var handler = new SubmitApplicationHandler(
            repository,
            ruleEngine,
            new SubmitApplicationValidator(),
            new TestClock(DateTimeOffset.Parse("2026-01-01T12:00:00+00:00")));
        var command = new SubmitApplicationCommand("John", "Doe", "Street", "NY", "Company", 1000, "555-55-5555");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsApproved);
        Assert.Contains("New York", result.DenialReason);
        Assert.Empty(repository.Customers);
        Assert.Empty(repository.Applications);
        Assert.Empty(repository.SubmittedEvents);
    }

    [Theory]
    [InlineData("123-45-678")]
    [InlineData("123-45-678X")]
    [InlineData("123.45.6789")]
    public async Task Handle_WhenSsnIsMalformed_ReturnsValidationErrorWithoutPersisting(string ssn)
    {
        var repository = new InMemoryApplicationRepository();
        var handler = new SubmitApplicationHandler(
            repository,
            new RuleEngine(Array.Empty<IDenyRule>()),
            new SubmitApplicationValidator(),
            new TestClock(DateTimeOffset.Parse("2026-01-01T12:00:00+00:00")));
        var command = new SubmitApplicationCommand("John", "Doe", "Street", "CA", "Company", 1000, ssn);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsInvalid);
        Assert.False(result.IsApproved);
        Assert.Contains("nine digits", result.DenialReason);
        Assert.Empty(repository.Customers);
        Assert.Empty(repository.Applications);
        Assert.Empty(repository.SubmittedEvents);
    }

    [Fact]
    public async Task Handle_WhenSsnMatchesBlacklistAfterNormalization_ReturnsDenialWithoutPersisting()
    {
        var repository = new InMemoryApplicationRepository();
        var handler = new SubmitApplicationHandler(
            repository,
            new RuleEngine([new SsnBlacklistRule(["000-00-0000"])]),
            new SubmitApplicationValidator(),
            new TestClock(DateTimeOffset.Parse("2026-01-01T12:00:00+00:00")));
        var command = new SubmitApplicationCommand("John", "Doe", "Street", "CA", "Company", 1000, "000 00-0000");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsApproved);
        Assert.False(result.IsInvalid);
        Assert.Contains("blacklisted", result.DenialReason);
        Assert.Empty(repository.Customers);
        Assert.Empty(repository.Applications);
        Assert.Empty(repository.SubmittedEvents);
    }

    [Fact]
    public async Task Handle_WhenSsnAlreadyExists_UpdatesExistingCustomerAndApplication()
    {
        var repository = new InMemoryApplicationRepository();
        var createdAt = DateTimeOffset.Parse("2026-01-01T12:00:00+00:00");
        var updatedAt = createdAt.AddDays(1);
        var clock = new TestClock(createdAt);
        var handler = new SubmitApplicationHandler(
            repository,
            new RuleEngine(Array.Empty<IDenyRule>()),
            new SubmitApplicationValidator(),
            clock);

        var firstCommand = new SubmitApplicationCommand("Ariel", "Patino", "Street 1", "CA", "Tech", 1000, "555-55-5555");
        var firstResponse = await handler.Handle(firstCommand, CancellationToken.None);
        clock.UtcNow = updatedAt;

        var secondCommand = new SubmitApplicationCommand("Updated", "Updated", "Street 2", "TX", "Tech 2", 2000, " 555 55-5555 ");
        var secondResponse = await handler.Handle(secondCommand, CancellationToken.None);

        Assert.Single(repository.Customers);
        Assert.Single(repository.Applications);
        Assert.Equal(firstResponse.CustomerId, secondResponse.CustomerId);
        Assert.Equal(firstResponse.ApplicationId, secondResponse.ApplicationId);
        Assert.Equal(2, repository.SubmittedEvents.Count);
        Assert.False(repository.SubmittedEvents[0].IsReturningCustomer);
        Assert.True(repository.SubmittedEvents[1].IsReturningCustomer);

        var customer = Assert.Single(repository.Customers);
        Assert.Equal("Updated", customer.FirstName);
        Assert.Equal("Updated", customer.LastName);
        Assert.Equal("Street 2", customer.Address);
        Assert.Equal("TX", customer.State);
        Assert.Equal("Tech 2", customer.CompanyName);
        Assert.Equal("555555555", customer.Ssn);
        Assert.Equal(createdAt, customer.CreatedAt);
        Assert.Equal(updatedAt, customer.UpdatedAt);
        Assert.Equal(2000, customer.Application!.RequestedAmount);
        Assert.Equal(createdAt, customer.Application.CreatedAt);
        Assert.Equal(updatedAt, customer.Application.UpdatedAt);
        Assert.Equal("555-55-5555", repository.SubmittedEvents[1].Ssn);
    }

    private sealed class TestClock(DateTimeOffset utcNow) : IClock
    {
        public DateTimeOffset UtcNow { get; set; } = utcNow;
    }

    private sealed class InMemoryApplicationRepository : IApplicationRepository
    {
        public List<Customer> Customers { get; } = [];
        public List<ApplicationEntity> Applications { get; } = [];
        public List<ApplicationSubmittedEvent> SubmittedEvents { get; } = [];

        public Task<Customer?> GetCustomerWithApplicationBySsnAsync(
            string ssn,
            CancellationToken cancellationToken = default)
        {
            var customer = Customers.FirstOrDefault(
                candidate => string.Equals(
                    candidate.Ssn.Replace("-", string.Empty).Replace(" ", string.Empty),
                    ssn.Trim(),
                    StringComparison.Ordinal));
            return Task.FromResult(customer);
        }

        public void AddCustomer(Customer customer) => Customers.Add(customer);

        public void AddApplication(ApplicationEntity application)
        {
            Applications.Add(application);
            var customer = Customers.FirstOrDefault(candidate => candidate.Id == application.CustomerId);
            customer?.AttachApplication(application);
        }

        public Task CommitTransactionWithEventAsync(
            ApplicationSubmittedEvent @event,
            CancellationToken cancellationToken = default)
        {
            SubmittedEvents.Add(@event);
            return Task.CompletedTask;
        }
    }
}
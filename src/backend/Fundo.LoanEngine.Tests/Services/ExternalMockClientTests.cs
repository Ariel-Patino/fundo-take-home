using System.Net;
using Fundo.LoanEngine.Application.Messaging;
using Fundo.LoanEngine.Infrastructure.External;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Fundo.LoanEngine.Tests.Services;

public sealed class ExternalMockClientTests
{
    [Theory]
    [InlineData(false, "POST", "/external/applications")]
    [InlineData(true, "PUT", "/external/applications/555-55-5555")]
    public async Task SendApplicationAsync_UsesMethodForCustomerStatus(
        bool isReturningCustomer,
        string expectedMethod,
        string expectedPath)
    {
        var handler = new RecordingHandler();
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://mock-service")
        };
        var client = new ExternalMockClient(httpClient, NullLogger<ExternalMockClient>.Instance);
        var submittedEvent = new ApplicationSubmittedEvent(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "First",
            "Last",
            "Street",
            "CA",
            "Company",
            1000,
            "555-55-5555",
            isReturningCustomer);

        await client.SendApplicationAsync(submittedEvent);

        Assert.Equal(expectedMethod, handler.Method?.Method);
        Assert.Equal(expectedPath, handler.RequestUri?.AbsolutePath);
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        public HttpMethod? Method { get; private set; }
        public Uri? RequestUri { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Method = request.Method;
            RequestUri = request.RequestUri;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }
}
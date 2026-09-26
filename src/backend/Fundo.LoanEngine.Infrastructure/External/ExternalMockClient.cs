
using System.Net.Http.Json;
using Fundo.LoanEngine.Application.Interfaces;
using Fundo.LoanEngine.Application.Messaging;
using Microsoft.Extensions.Logging;

namespace Fundo.LoanEngine.Infrastructure.External;

public sealed class ExternalMockClient : IExternalMockClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ExternalMockClient> _logger;

    public ExternalMockClient(HttpClient httpClient, ILogger<ExternalMockClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task SendApplicationAsync(
        ApplicationSubmittedEvent evt,
        CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response;

        if (evt.IsReturningCustomer)
        {
            _logger.LogInformation("Updating external application {ApplicationId}.", evt.ApplicationId);
            response = await _httpClient.PutAsJsonAsync(
                $"/external/applications/{Uri.EscapeDataString(evt.Ssn)}",
                evt,
                cancellationToken);
        }
        else
        {
            _logger.LogInformation("Creating external application {ApplicationId}.", evt.ApplicationId);
            response = await _httpClient.PostAsJsonAsync(
                "/external/applications",
                evt,
                cancellationToken);
        }

        response.EnsureSuccessStatusCode();
    }
}
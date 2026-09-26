using Fundo.LoanEngine.Application.Messaging;

namespace Fundo.LoanEngine.Application.Interfaces;

public interface IExternalMockClient
{
    Task SendApplicationAsync(ApplicationSubmittedEvent evt, CancellationToken cancellationToken = default);
}
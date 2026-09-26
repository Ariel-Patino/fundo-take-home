namespace Fundo.LoanEngine.Application.Interfaces;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
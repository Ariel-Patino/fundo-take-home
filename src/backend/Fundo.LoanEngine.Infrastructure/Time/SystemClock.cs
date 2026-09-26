using Fundo.LoanEngine.Application.Interfaces;

namespace Fundo.LoanEngine.Infrastructure.Time;

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
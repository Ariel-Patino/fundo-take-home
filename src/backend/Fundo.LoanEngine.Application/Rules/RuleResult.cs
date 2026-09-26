namespace Fundo.LoanEngine.Application.Rules;

public sealed record RuleResult(bool IsDenied, string? Reason)
{
    public static RuleResult Pass() => new(false, null);
    public static RuleResult Deny(string reason) => new(true, reason);
}
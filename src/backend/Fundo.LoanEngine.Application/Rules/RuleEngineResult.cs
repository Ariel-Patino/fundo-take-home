namespace Fundo.LoanEngine.Application.Rules;

public sealed record RuleEngineResult(bool IsDenied, string? RuleName, string? Reason)
{
    public static RuleEngineResult Approved() => new(false, null, null);
    public static RuleEngineResult Denied(string ruleName, string reason) => new(true, ruleName, reason);
}
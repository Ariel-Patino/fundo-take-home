using Fundo.LoanEngine.Domain.Rules;
using Fundo.LoanEngine.Domain.ValueObjects;

namespace Fundo.LoanEngine.Application.Rules;

public sealed class SsnBlacklistRule : IDenyRule
{
    private readonly HashSet<string> _blacklistedSsns;

    public SsnBlacklistRule(IEnumerable<string> blacklistedSsns)
    {
        _blacklistedSsns = new HashSet<string>(
            blacklistedSsns.Select(ssn => Ssn.Create(ssn).Value),
            StringComparer.OrdinalIgnoreCase);
    }

    public string Name => nameof(SsnBlacklistRule);

    public RuleResult Evaluate(LoanApplicant data)
    {
        return !Ssn.TryCreate(data.Ssn, out var ssn)
            ? RuleResult.Deny("The provided SSN is invalid.")
            : _blacklistedSsns.Contains(ssn.Value)
            ? RuleResult.Deny("The provided SSN is blacklisted.")
            : RuleResult.Pass();
    }
}
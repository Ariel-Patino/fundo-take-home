using Fundo.LoanEngine.Domain.Rules;

namespace Fundo.LoanEngine.Application.Rules;

public sealed class StateDenyRule : IDenyRule
{
    private readonly HashSet<string> _blockedStates;

    public StateDenyRule(IEnumerable<string> blockedStates)
    {
        _blockedStates = new HashSet<string>(
            blockedStates.Select(state => state.Trim()),
            StringComparer.OrdinalIgnoreCase);
    }

    public string Name => nameof(StateDenyRule);

    public RuleResult Evaluate(LoanApplicant data)
    {
        var state = data.State.Trim();
        var stateName = string.Equals(state, "NY", StringComparison.OrdinalIgnoreCase)
            ? "New York (NY)"
            : state.ToUpperInvariant();
        return _blockedStates.Contains(state)
            ? RuleResult.Deny($"Applications from {stateName} state are not allowed.")
            : RuleResult.Pass();
    }
}
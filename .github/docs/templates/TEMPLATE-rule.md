
---

# 21. `docs/templates/TEMPLATE-rule.md`

```markdown
# TEMPLATE — Deny Rule

```csharp
namespace Fundo.Application.Applications.Rules;

public sealed class StateDenyRule(IOptions<RuleOptions> options) : IDenyRule
{
    private readonly HashSet<string> _blockedStates =
        new(options.Value.BlockedStates, StringComparer.OrdinalIgnoreCase);

    public string Name => "StateDenyRule";

    public RuleResult Evaluate(LoanApplicationData data)
    {
        if (_blockedStates.Contains(data.State))
            return RuleResult.Deny($"State {data.State} is not eligible.");

        return RuleResult.Pass();
    }
}
```
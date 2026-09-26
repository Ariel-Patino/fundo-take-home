# SKILL — Rule Engine

## Goal
Deny rules must be pluggable. Adding a new rule = adding one file. Zero changes to existing rules.

## Contract
```csharp
namespace Fundo.Application.Applications.Rules;

public interface IDenyRule
{
    string Name { get; }
    RuleResult Evaluate(LoanApplicationData data);
}

public sealed record RuleResult(bool IsDenied, string? Reason)
{
    public static RuleResult Pass() => new(false, null);
    public static RuleResult Deny(string reason) => new(true, reason);
}
public sealed class RuleEngine(IEnumerable<IDenyRule> rules)
{
    public RuleEngineResult Evaluate(LoanApplicationData data)
    {
        foreach (var rule in rules)
        {
            var result = rule.Evaluate(data);
            if (result.IsDenied)
                return new RuleEngineResult.Denied(rule.Name, result.Reason!);
        }
        return new RuleEngineResult.Approved();
    }
}
```
Built-in Rules
StateDenyRule — denies if state is in a configured blacklist (NY).

SsnBlacklistRule — denies if SSN is in a configured blacklist.
Adding a New Rule
Create MyNewRule.cs in Application/Applications/Rules/.

Implement IDenyRule.

Register in DI: services.AddSingleton<IDenyRule, MyNewRule>();

Done. No existing file changes.
Configuration
Blacklisted states and SSNs come from appsettings.json via IOptions<RuleOptions>.

Application layer reads RuleOptions through a port if needed, OR rules receive
their config via constructor injection from Infrastructure's DI.

Testing
Each rule gets its own test class. Engine gets an ordering test.

Checklist
- [ ] Rules are stateless
- [ ] Rules have a unique Name
- [ ] No rule knows about other rules
- [ ] No if chains in the engine

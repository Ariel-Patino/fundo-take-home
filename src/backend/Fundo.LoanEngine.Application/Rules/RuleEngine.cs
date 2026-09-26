using Fundo.LoanEngine.Domain.Rules;

namespace Fundo.LoanEngine.Application.Rules;

public sealed class RuleEngine(IEnumerable<IDenyRule> rules)
{
    private readonly IDenyRule[] _rules = rules.ToArray();

    public RuleEngineResult Evaluate(LoanApplicant applicant)
    {
        foreach (var rule in _rules)
        {
            var result = rule.Evaluate(applicant);
            if (result.IsDenied)
            {
                return RuleEngineResult.Denied(rule.Name, result.Reason!);
            }
        }

        return RuleEngineResult.Approved();
    }
}
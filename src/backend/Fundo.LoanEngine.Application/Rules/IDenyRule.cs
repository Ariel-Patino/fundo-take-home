using Fundo.LoanEngine.Domain.Rules;

namespace Fundo.LoanEngine.Application.Rules;

public interface IDenyRule
{
    string Name { get; }
    RuleResult Evaluate(LoanApplicant data);
}
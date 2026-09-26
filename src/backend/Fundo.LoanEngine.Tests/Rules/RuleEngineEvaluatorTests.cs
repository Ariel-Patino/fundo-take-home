using Fundo.LoanEngine.Application.Rules;
using Fundo.LoanEngine.Domain.Rules;
using Xunit;

namespace Fundo.LoanEngine.Tests.Rules;

public class RuleEngineEvaluatorTests
{
    [Fact]
    public void Evaluate_WhenMultipleRulesMatch_ReturnsFirstDenial()
    {
        var rules = new IDenyRule[]
        {
            new StateDenyRule(["NY"]),
            new SsnBlacklistRule(["000-00-0000"])
        };
        var engine = new RuleEngine(rules);
        var applicant = new LoanApplicant("John", "Doe", "Street 123", "NY", "Acme", 5000, "000-00-0000");

        var result = engine.Evaluate(applicant);

        Assert.True(result.IsDenied);
        Assert.Equal(nameof(StateDenyRule), result.RuleName);
        Assert.Contains("New York", result.Reason);
    }

    [Fact]
    public void Evaluate_WhenNoRuleMatches_Approves()
    {
        var engine = new RuleEngine([new StateDenyRule(["NY"]), new SsnBlacklistRule(["000-00-0000"])]);
        var applicant = new LoanApplicant("John", "Doe", "Street 123", "CA", "Acme", 5000, "111-11-1111");

        var result = engine.Evaluate(applicant);

        Assert.False(result.IsDenied);
        Assert.Null(result.RuleName);
        Assert.Null(result.Reason);
    }

    [Fact]
    public void Evaluate_WhenSsnIsBlacklisted_ReturnsDenialFromSsnRule()
    {
        var engine = new RuleEngine([new StateDenyRule(["NY"]), new SsnBlacklistRule(["000-00-0000"])]);
        var applicant = new LoanApplicant("John", "Doe", "Street 123", "CA", "Acme", 5000, "000-00-0000");

        var result = engine.Evaluate(applicant);

        Assert.True(result.IsDenied);
        Assert.Equal(nameof(SsnBlacklistRule), result.RuleName);
        Assert.Contains("blacklisted", result.Reason);
    }
}
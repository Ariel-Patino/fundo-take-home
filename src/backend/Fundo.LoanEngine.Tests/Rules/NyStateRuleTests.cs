using Fundo.LoanEngine.Application.Rules;
using Fundo.LoanEngine.Domain.Rules;
using Xunit;

namespace Fundo.LoanEngine.Tests.Rules;

public class NyStateRuleTests
{
    private readonly StateDenyRule _rule = new(["NY"]);

    [Theory]
    [InlineData("NY")]
    [InlineData("ny")]
    [InlineData(" Ny ")]
    public void Evaluate_WhenStateIsNy_Denies(string state)
    {
        var applicant = new LoanApplicant("John", "Doe", "Street 123", state, "Acme", 5000, "111-11-1111");
        var result = _rule.Evaluate(applicant);

        Assert.True(result.IsDenied);
        Assert.Contains("New York", result.Reason);
    }

    [Fact]
    public void Evaluate_WhenStateIsNotBlocked_Passes()
    {
        var applicant = new LoanApplicant("John", "Doe", "Street 123", "CA", "Acme", 5000, "111-11-1111");
        var result = _rule.Evaluate(applicant);

        Assert.False(result.IsDenied);
    }
}
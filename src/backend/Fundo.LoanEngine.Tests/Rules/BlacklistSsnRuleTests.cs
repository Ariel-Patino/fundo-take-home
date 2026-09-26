using Fundo.LoanEngine.Application.Rules;
using Fundo.LoanEngine.Domain.Rules;
using Xunit;

namespace Fundo.LoanEngine.Tests.Rules;

public class BlacklistSsnRuleTests
{
    private readonly SsnBlacklistRule _rule = new(["000-00-0000", "999-99-9999"]);

    [Theory]
    [InlineData("000-00-0000")]
    [InlineData("999-99-9999")]
    public void Evaluate_WhenSsnIsBlacklisted_Denies(string ssn)
    {
        var applicant = new LoanApplicant("John", "Doe", "Street 123", "FL", "Acme", 5000, ssn);
        var result = _rule.Evaluate(applicant);

        Assert.True(result.IsDenied);
        Assert.Contains("blacklisted", result.Reason);
    }

    [Fact]
    public void Evaluate_WhenSsnIsNotBlacklisted_Passes()
    {
        var applicant = new LoanApplicant("John", "Doe", "Street 123", "FL", "Acme", 5000, "123-00-9999");
        var result = _rule.Evaluate(applicant);

        Assert.False(result.IsDenied);
    }

    [Theory]
    [InlineData("000000000")]
    [InlineData("000 00 0000")]
    [InlineData(" 000-00-0000 ")]
    public void Evaluate_WhenSsnUsesDifferentFormatting_Denies(string ssn)
    {
        var applicant = new LoanApplicant("John", "Doe", "Street 123", "FL", "Acme", 5000, ssn);

        var result = _rule.Evaluate(applicant);

        Assert.True(result.IsDenied);
        Assert.Contains("blacklisted", result.Reason);
    }

    [Fact]
    public void Evaluate_WhenSsnIsMalformed_Denies()
    {
        var applicant = new LoanApplicant("John", "Doe", "Street 123", "FL", "Acme", 5000, "123-45-678x");

        var result = _rule.Evaluate(applicant);

        Assert.True(result.IsDenied);
        Assert.Contains("invalid", result.Reason);
    }
}
# TEMPLATE — Unit Test

```csharp
namespace Fundo.Application.Tests.Applications.Rules;

public sealed class StateDenyRuleTests
{
    [Fact]
    public void Evaluate_WhenStateIsNy_ReturnsDenied()
    {
        var options = Options.Create(new RuleOptions { BlockedStates = ["NY"] });
        var rule = new StateDenyRule(options);
        var data = new LoanApplicationData(State: "NY");

        var result = rule.Evaluate(data);

        result.IsDenied.Should().BeTrue();
        result.Reason.Should().Contain("NY");
    }

    [Fact]
    public void Evaluate_WhenStateIsCa_ReturnsPass()
    {
        var options = Options.Create(new RuleOptions { BlockedStates = ["NY"] });
        var rule = new StateDenyRule(options);
        var data = new LoanApplicationData(State: "CA");

        var result = rule.Evaluate(data);

        result.IsDenied.Should().BeFalse();
    }
}
```
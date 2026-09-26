# SKILL — Backend Testing

## Location
`backend/tests/Fundo.Domain.Tests/`
`backend/tests/Fundo.Application.Tests/`
`backend/tests/Fundo.WebApi.Tests/`

## Frameworks
- xUnit
- FluentAssertions
- NSubstitute (mocking ports)
- `Microsoft.AspNetCore.Mvc.Testing` for WebApi integration tests

## What to Test
1. **Rule Engine**: each deny rule + approval path + rule ordering.
2. **Returning customer path**: same SSN updates existing records.
3. **Endpoint**: POST /api/applications — approved, denied, validation error.

## What NOT to Test
- Getters/setters.
- EF Core itself.
- Trivial mappers.

## Test Naming
`MethodName_Scenario_ExpectedOutcome`
Example: `Evaluate_WhenStateIsNy_ReturnsDenied`

## Arrange-Act-Assert
Always. One assertion concept per test.

## Example (Rule Engine)
```csharp
public sealed class RuleEngineTests
{
    [Fact]
    public void Evaluate_WhenStateIsNy_ReturnsDenied()
    {
        var engine = new RuleEngine([new StateDenyRule(["NY"])]);
        var application = LoanApplicationFactory.Create(state: "NY");

        var result = engine.Evaluate(application);

        result.IsDenied.Should().BeTrue();
        result.Reason.Should().Contain("NY");
    }
}
```
WebApi Tests
Use WebApplicationFactory<Program>.

Use a real Postgres container via Testcontainers.PostgreSql.

Reset DB between tests with a transaction rollback or Respawn.
Checklist
- [ ] Tests are in /tests, never co-located
- [ ] No comments in tests
- [ ] No Thread.Sleep
- [ ] No shared mutable state between tests


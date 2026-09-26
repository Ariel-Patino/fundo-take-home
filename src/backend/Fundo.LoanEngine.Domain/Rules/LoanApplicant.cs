namespace Fundo.LoanEngine.Domain.Rules;

public record LoanApplicant(
    string FirstName,
    string LastName,
    string Address,
    string State,
    string CompanyName,
    decimal RequestedAmount,
    string Ssn
);
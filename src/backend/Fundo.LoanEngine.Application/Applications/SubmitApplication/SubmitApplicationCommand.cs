namespace Fundo.LoanEngine.Application.Applications.SubmitApplication;

public sealed record SubmitApplicationCommand(
    string FirstName,
    string LastName,
    string Address,
    string State,
    string CompanyName,
    decimal RequestedAmount,
    string Ssn)
{
    public SubmitApplicationCommand NormalizeSsn(string normalizedSsn) => this with { Ssn = normalizedSsn };
}
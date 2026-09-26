using Fundo.LoanEngine.Application.Applications.SubmitApplication;

namespace Fundo.LoanEngine.WebApi.Contracts;

public sealed record SubmitApplicationRequest(
    string FirstName,
    string LastName,
    string Address,
    string State,
    string CompanyName,
    decimal RequestedAmount,
    string Ssn)
{
    public SubmitApplicationCommand ToCommand() => new(
        FirstName,
        LastName,
        Address,
        State,
        CompanyName,
        RequestedAmount,
        Ssn);
}
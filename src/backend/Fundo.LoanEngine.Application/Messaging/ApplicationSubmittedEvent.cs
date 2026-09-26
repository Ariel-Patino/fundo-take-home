namespace Fundo.LoanEngine.Application.Messaging;

public sealed record ApplicationSubmittedEvent(
    Guid CustomerId,
    Guid ApplicationId,
    string FirstName,
    string LastName,
    string Address,
    string State,
    string CompanyName,
    decimal RequestedAmount,
    string Ssn,
    bool IsReturningCustomer);
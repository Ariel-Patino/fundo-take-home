namespace Fundo.LoanEngine.WebApi.Contracts;

public sealed record SubmitApplicationResponse(
    string Status,
    string? Reason,
    Guid? CustomerId,
    Guid? ApplicationId)
{
    public static SubmitApplicationResponse Approved(Guid customerId, Guid applicationId) =>
        new("Approved", null, customerId, applicationId);

    public static SubmitApplicationResponse Denied(string reason) =>
        new("Denied", reason, null, null);

    public static SubmitApplicationResponse Invalid(string reason) =>
        new("Invalid", reason, null, null);
}
namespace Fundo.LoanEngine.Application.Applications.SubmitApplication;

public sealed record SubmitApplicationResult(
    bool IsApproved,
    bool IsInvalid,
    Guid? CustomerId,
    Guid? ApplicationId,
    string? DenialReason)
{
    public static SubmitApplicationResult Approved(Guid customerId, Guid applicationId) =>
        new(true, false, customerId, applicationId, null);

    public static SubmitApplicationResult Denied(string reason) =>
        new(false, false, null, null, reason);

    public static SubmitApplicationResult Invalid(string reason) =>
        new(false, true, null, null, reason);
}
using Fundo.LoanEngine.Application.Applications.SubmitApplication;

namespace Fundo.LoanEngine.WebApi.Contracts;

public static class SubmitApplicationMappings
{
    public static SubmitApplicationResponse ToResponse(this SubmitApplicationResult result) =>
        result.IsApproved
            ? SubmitApplicationResponse.Approved(result.CustomerId!.Value, result.ApplicationId!.Value)
            : result.IsInvalid
                ? SubmitApplicationResponse.Invalid(result.DenialReason!)
                : SubmitApplicationResponse.Denied(result.DenialReason!);
}
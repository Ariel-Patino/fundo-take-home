namespace Fundo.LoanEngine.Application.Applications.SubmitApplication;

public sealed record SubmitApplicationValidationResult(bool IsValid, string? ErrorMessage)
{
    public static SubmitApplicationValidationResult Valid() => new(true, null);
    public static SubmitApplicationValidationResult Invalid(string errorMessage) => new(false, errorMessage);
}
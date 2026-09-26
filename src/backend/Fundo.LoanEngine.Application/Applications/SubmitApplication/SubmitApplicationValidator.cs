using Fundo.LoanEngine.Domain.ValueObjects;

namespace Fundo.LoanEngine.Application.Applications.SubmitApplication;

public sealed class SubmitApplicationValidator
{
    public SubmitApplicationValidationResult Validate(SubmitApplicationCommand command)
    {
        return Ssn.TryCreate(command.Ssn, out _)
            ? SubmitApplicationValidationResult.Valid()
            : SubmitApplicationValidationResult.Invalid("SSN must contain exactly nine digits.");
    }
}
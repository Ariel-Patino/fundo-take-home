using Fundo.LoanEngine.Application.Interfaces;
using Fundo.LoanEngine.Application.Messaging;
using Fundo.LoanEngine.Application.Rules;
using Fundo.LoanEngine.Domain.Entities;
using Fundo.LoanEngine.Domain.Rules;
using Fundo.LoanEngine.Domain.ValueObjects;
using ApplicationEntity = Fundo.LoanEngine.Domain.Entities.Application;

namespace Fundo.LoanEngine.Application.Applications.SubmitApplication;

public sealed class SubmitApplicationHandler(
    IApplicationRepository repository,
    RuleEngine ruleEngine,
    SubmitApplicationValidator validator,
    IClock clock)
{
    public async Task<SubmitApplicationResult> Handle(
        SubmitApplicationCommand command,
        CancellationToken cancellationToken)
    {
        var validation = validator.Validate(command);
        if (!validation.IsValid)
        {
            return SubmitApplicationResult.Invalid(validation.ErrorMessage!);
        }

        var ssn = Ssn.Create(command.Ssn);
        command = command.NormalizeSsn(ssn.Value);

        var applicant = new LoanApplicant(
            command.FirstName,
            command.LastName,
            command.Address,
            command.State,
            command.CompanyName,
            command.RequestedAmount,
            command.Ssn);

        var ruleResult = ruleEngine.Evaluate(applicant);
        if (ruleResult.IsDenied)
        {
            return SubmitApplicationResult.Denied(ruleResult.Reason ?? "The application was denied.");
        }

        var now = clock.UtcNow;
        var existingCustomer = await repository.GetCustomerWithApplicationBySsnAsync(
            ssn.Value,
            cancellationToken);
        var isReturningCustomer = existingCustomer is not null;
        Customer customer;
        ApplicationEntity application;

        if (existingCustomer is not null)
        {
            customer = existingCustomer;
            customer.NormalizeSsn(ssn.Value);
            customer.UpdateFrom(
                command.FirstName,
                command.LastName,
                command.Address,
                command.State,
                command.CompanyName,
                now);

            if (customer.Application is null)
            {
                application = ApplicationEntity.Create(customer.Id, command.RequestedAmount, now);
                customer.AttachApplication(application);
                repository.AddApplication(application);
            }
            else
            {
                application = customer.Application;
                application.UpdateRequestedAmount(command.RequestedAmount, now);
            }
        }
        else
        {
            customer = Customer.Create(
                command.FirstName,
                command.LastName,
                command.Address,
                command.State,
                command.CompanyName,
                command.Ssn,
                now);
            application = ApplicationEntity.Create(customer.Id, command.RequestedAmount, now);
            customer.AttachApplication(application);

            repository.AddCustomer(customer);
            repository.AddApplication(application);
        }

        var submittedEvent = new ApplicationSubmittedEvent(
            customer.Id,
            application.Id,
            customer.FirstName,
            customer.LastName,
            customer.Address,
            customer.State,
            customer.CompanyName,
            application.RequestedAmount,
            ssn.Formatted,
            isReturningCustomer);

        await repository.CommitTransactionWithEventAsync(submittedEvent, cancellationToken);

        return SubmitApplicationResult.Approved(customer.Id, application.Id);
    }
}
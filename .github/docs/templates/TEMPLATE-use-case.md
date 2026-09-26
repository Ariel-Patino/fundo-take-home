
---

# 22. `docs/templates/TEMPLATE-use-case.md`

```markdown
# TEMPLATE — Use Case

```csharp
namespace Fundo.Application.Applications.SubmitApplication;

public sealed record SubmitApplicationCommand(
    string FirstName,
    string LastName,
    AddressDto Address,
    string CompanyName,
    decimal RequestedAmount,
    string Ssn);

public sealed record SubmitApplicationResult(
    bool IsApproved,
    Guid? ApplicationId,
    string? DenialReason)
{
    public static SubmitApplicationResult Approved(Guid id) => new(true, id, null);
    public static SubmitApplicationResult Denied(string reason) => new(false, null, reason);
}

public sealed class SubmitApplicationHandler(
    ICustomerRepository customers,
    ILoanApplicationRepository applications,
    IOutboxWriter outbox,
    IUnitOfWork unitOfWork,
    RuleEngine engine,
    IClock clock)
{
    public async Task<SubmitApplicationResult> Handle(
        SubmitApplicationCommand command,
        CancellationToken ct)
    {
        var decision = engine.Evaluate(command.ToData());
        if (decision.IsDenied)
            return SubmitApplicationResult.Denied(decision.Reason);

        await using var tx = await unitOfWork.BeginTransactionAsync(ct);

        var now = clock.UtcNow;
        var ssn = Ssn.Create(command.Ssn);

        var customer = await customers.GetBySsnAsync(ssn, ct);
        if (customer is null)
        {
            customer = Customer.Create(
                command.FirstName, command.LastName,
                command.Address.ToDomain(), command.CompanyName, ssn, now);
            customers.Add(customer);
        }
        else
        {
            customer.UpdateFrom(
                command.FirstName, command.LastName,
                command.Address.ToDomain(), command.CompanyName, now);
        }

        var application = await applications.GetByCustomerIdAsync(customer.Id, ct);
        if (application is null)
        {
            application = LoanApplication.Create(customer.Id, command.RequestedAmount, now);
            applications.Add(application);
        }
        else
        {
            application.UpdateAmount(command.RequestedAmount, now);
        }

        await outbox.EnqueueAsync(
            LoanApplicationSubmitted.From(customer, application), ct);

        await unitOfWork.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return SubmitApplicationResult.Approved(application.Id);
    }
}
```
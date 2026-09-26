
# SKILL — Transactional Persistence + Outbox

## Requirement
Customer insert/update + Application insert/update + event publication = ONE unit of work.
If any fails → rollback everything. No half-saved data. No orphan events.

## Approach: Outbox Pattern (simplified)
1. In the SAME `DbContext.SaveChangesAsync()` transaction:
   - Save Customer
   - Save LoanApplication
   - Insert `OutboxMessage` row (payload JSON + status=Pending)
2. Background `OutboxPublisher` polls `OutboxMessages` where `Status = Pending`.
3. For each: HTTP call to external service. On 200 → mark `Processed`. On failure → increment `RetryCount`, backoff.
4. On successful publish → optionally publish to an in-process channel for observability.

## Why Outbox
- No 2-phase commit.
- No distributed transaction.
- Guarantees at-least-once delivery with idempotency on the consumer side (mock must be idempotent by SSN).

## Schema
```sql
CREATE TABLE outbox_messages (
    id UUID PRIMARY KEY,
    type TEXT NOT NULL,
    payload JSONB NOT NULL,
    status TEXT NOT NULL,
    retry_count INT NOT NULL DEFAULT 0,
    created_at TIMESTAMPTZ NOT NULL,
    processed_at TIMESTAMPTZ NULL
);
```
Handler
```csharp
public sealed class SubmitApplicationHandler(
    ICustomerRepository customers,
    ILoanApplicationRepository applications,
    IOutboxWriter outbox,
    IUnitOfWork unitOfWork,
    RuleEngine engine,
    IClock clock)
{
    public async Task<SubmitApplicationResult> Handle(SubmitApplicationCommand command, CancellationToken ct)
    {
        var decision = engine.Evaluate(command.ToData());
        if (decision.IsDenied)
            return SubmitApplicationResult.Denied(decision.Reason);

        await using var tx = await unitOfWork.BeginTransactionAsync(ct);

        var customer = await customers.GetBySsnAsync(command.Ssn, ct)
            ?? Customer.Create(command.ToCustomerData(), clock.UtcNow);
        customer.UpdateFrom(command.ToCustomerData(), clock.UtcNow);

        var application = await applications.GetByCustomerIdAsync(customer.Id, ct)
            ?? LoanApplication.Create(customer.Id, command.RequestedAmount, clock.UtcNow);
        application.UpdateAmount(command.RequestedAmount, clock.UtcNow);

        customers.AddOrUpdate(customer);
        applications.AddOrUpdate(application);

        await outbox.EnqueueAsync(new LoanApplicationEvent(customer, application), ct);

        await unitOfWork.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return SubmitApplicationResult.Approved(application.Id);
    }
}
```
Failure Semantics
DB fails mid-save → transaction rolls back → nothing persisted, no outbox row.

Outbox row saved but HTTP fails → transaction already committed → background worker retries.

HTTP 500 → retry. HTTP 4xx → mark Failed, log, alert (configurable).

Checklist
- [ ] Single SaveChangesAsync per use case
- [ ] Explicit transaction scope
- [ ] Outbox row inserted in the same transaction
- [ ] Background worker uses IHostedService + PeriodicTimer
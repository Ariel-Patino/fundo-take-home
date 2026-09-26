using Fundo.LoanEngine.Application.Interfaces;
using Fundo.LoanEngine.Application.Messaging;
using Fundo.LoanEngine.Domain.Entities;
using Fundo.LoanEngine.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using ApplicationEntity = Fundo.LoanEngine.Domain.Entities.Application;

namespace Fundo.LoanEngine.Infrastructure.Persistence.Repositories;

public sealed class ApplicationRepository(ApplicationDbContext db, IClock clock) : IApplicationRepository
{
    public async Task<Customer?> GetCustomerWithApplicationBySsnAsync(
        string ssn,
        CancellationToken cancellationToken = default)
    {
        return await db.Customers
            .Include(c => c.Application)
            .FirstOrDefaultAsync(
                customer => customer.Ssn == ssn || customer.Ssn.Replace("-", "").Replace(" ", "") == ssn,
                cancellationToken);
    }

    public void AddCustomer(Customer customer)
    {
        db.Customers.Add(customer);
    }

    public void AddApplication(ApplicationEntity application)
    {
        db.Applications.Add(application);
    }

    public async Task CommitTransactionWithEventAsync(
        ApplicationSubmittedEvent @event,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        db.OutboxMessages.Add(OutboxMessage.Create(
            nameof(ApplicationSubmittedEvent),
            JsonSerializer.Serialize(@event),
            clock.UtcNow));

        await db.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }
}
using Fundo.LoanEngine.Domain.Entities;
using Fundo.LoanEngine.Application.Messaging;
using ApplicationEntity = Fundo.LoanEngine.Domain.Entities.Application;

namespace Fundo.LoanEngine.Application.Interfaces;

public interface IApplicationRepository
{
    Task<Customer?> GetCustomerWithApplicationBySsnAsync(
        string ssn,
        CancellationToken cancellationToken = default);
    void AddCustomer(Customer customer);
    void AddApplication(ApplicationEntity application);
    Task CommitTransactionWithEventAsync(ApplicationSubmittedEvent @event, CancellationToken cancellationToken = default);
}
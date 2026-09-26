namespace Fundo.LoanEngine.Domain.Entities;

public sealed class Application
{
    private Application() { }

    private Application(Guid id, Guid customerId, decimal requestedAmount, DateTimeOffset createdAt)
    {
        Id = id;
        CustomerId = customerId;
        RequestedAmount = requestedAmount;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public decimal RequestedAmount { get; private set; }
    public Guid CustomerId { get; private set; }
    public Customer Customer { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static Application Create(Guid customerId, decimal requestedAmount, DateTimeOffset now)
    {
        if (customerId == Guid.Empty)
        {
            throw new ArgumentException("Customer ID is required.", nameof(customerId));
        }

        ValidateRequestedAmount(requestedAmount);
        return new Application(Guid.NewGuid(), customerId, requestedAmount, now);
    }

    public void UpdateRequestedAmount(decimal requestedAmount, DateTimeOffset now)
    {
        ValidateRequestedAmount(requestedAmount);
        RequestedAmount = requestedAmount;
        UpdatedAt = now;
    }

    private static void ValidateRequestedAmount(decimal requestedAmount)
    {
        if (requestedAmount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(requestedAmount), "Requested amount must be greater than zero.");
        }
    }
}
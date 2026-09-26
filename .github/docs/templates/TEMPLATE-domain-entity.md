# TEMPLATE — Domain Entity

```csharp
namespace Fundo.Domain.Customers;

public sealed class Customer
{
    private Customer() { }

    private Customer(
        Guid id,
        string firstName,
        string lastName,
        Address address,
        string companyName,
        Ssn ssn,
        DateTimeOffset createdAt)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Address = address;
        CompanyName = companyName;
        Ssn = ssn;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public Address Address { get; private set; } = null!;
    public string CompanyName { get; private set; } = null!;
    public Ssn Ssn { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static Customer Create(
        string firstName,
        string lastName,
        Address address,
        string companyName,
        Ssn ssn,
        DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name is required.");
        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name is required.");
        if (string.IsNullOrWhiteSpace(companyName))
            throw new DomainException("Company name is required.");

        return new Customer(Guid.NewGuid(), firstName, lastName, address, companyName, ssn, now);
    }

    public void UpdateFrom(
        string firstName,
        string lastName,
        Address address,
        string companyName,
        DateTimeOffset now)
    {
        FirstName = firstName;
        LastName = lastName;
        Address = address;
        CompanyName = companyName;
        UpdatedAt = now;
    }
}
```
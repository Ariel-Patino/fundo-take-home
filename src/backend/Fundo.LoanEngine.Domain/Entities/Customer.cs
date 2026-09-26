using SsnValue = Fundo.LoanEngine.Domain.ValueObjects.Ssn;

namespace Fundo.LoanEngine.Domain.Entities;

public sealed class Customer
{
    private Customer() { }

    private Customer(
        Guid id,
        string firstName,
        string lastName,
        string address,
        string state,
        string companyName,
        string ssn,
        DateTimeOffset createdAt)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Address = address;
        State = state;
        CompanyName = companyName;
        Ssn = ssn;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Address { get; private set; } = null!;
    public string State { get; private set; } = null!;
    public string CompanyName { get; private set; } = null!;
    public string Ssn { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public Application? Application { get; private set; }

    public static Customer Create(
        string firstName,
        string lastName,
        string address,
        string state,
        string companyName,
        string ssn,
        DateTimeOffset now)
    {
        ValidateRequired(firstName, nameof(firstName));
        ValidateRequired(lastName, nameof(lastName));
        ValidateRequired(address, nameof(address));
        ValidateRequired(state, nameof(state));
        ValidateRequired(companyName, nameof(companyName));
        var normalizedSsn = SsnValue.Create(ssn);

        var normalizedState = state.Trim();
        if (normalizedState.Length != 2)
        {
            throw new ArgumentException("State must be a two-letter code.", nameof(state));
        }

        return new Customer(
            Guid.NewGuid(),
            firstName.Trim(),
            lastName.Trim(),
            address.Trim(),
            normalizedState.ToUpperInvariant(),
            companyName.Trim(),
            normalizedSsn.Value,
            now);
    }

    public void UpdateFrom(
        string firstName,
        string lastName,
        string address,
        string state,
        string companyName,
        DateTimeOffset now)
    {
        ValidateRequired(firstName, nameof(firstName));
        ValidateRequired(lastName, nameof(lastName));
        ValidateRequired(address, nameof(address));
        ValidateRequired(state, nameof(state));
        ValidateRequired(companyName, nameof(companyName));

        var normalizedState = state.Trim();
        if (normalizedState.Length != 2)
        {
            throw new ArgumentException("State must be a two-letter code.", nameof(state));
        }

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Address = address.Trim();
        State = normalizedState.ToUpperInvariant();
        CompanyName = companyName.Trim();
        UpdatedAt = now;
    }

    public void AttachApplication(Application application)
    {
        ArgumentNullException.ThrowIfNull(application);

        if (application.CustomerId != Id)
        {
            throw new ArgumentException("Application must belong to this customer.", nameof(application));
        }

        if (Application is not null && Application.Id != application.Id)
        {
            throw new InvalidOperationException("A customer can have only one application.");
        }

        Application = application;
    }

    public void NormalizeSsn(string ssn)
    {
        Ssn = global::Fundo.LoanEngine.Domain.ValueObjects.Ssn.Create(ssn).Value;
    }

    private static void ValidateRequired(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
    }
}
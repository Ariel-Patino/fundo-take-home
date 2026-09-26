using Fundo.LoanEngine.Domain.Entities;
using ApplicationEntity = Fundo.LoanEngine.Domain.Entities.Application;
using Xunit;

namespace Fundo.LoanEngine.Tests.Rules;

public sealed class DomainEntityTests
{
    [Fact]
    public void CustomerCreate_WhenStateIsNotTwoLetters_Throws()
    {
        Assert.Throws<ArgumentException>(() => Customer.Create(
            "John",
            "Doe",
            "Street 1",
            "California",
            "Acme",
            "111-11-1111",
            DateTimeOffset.UtcNow));
    }

    [Fact]
    public void ApplicationCreate_WhenRequestedAmountIsNotPositive_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => ApplicationEntity.Create(
            Guid.NewGuid(),
            0,
            DateTimeOffset.UtcNow));
    }

    [Theory]
    [InlineData("123-45-6789")]
    [InlineData("123456789")]
    [InlineData(" 123 45-6789 ")]
    public void CustomerCreate_WhenSsnIsValid_StoresCanonicalDigits(string ssn)
    {
        var customer = Customer.Create(
            "John",
            "Doe",
            "Street 1",
            "CA",
            "Acme",
            ssn,
            DateTimeOffset.UtcNow);

        Assert.Equal("123456789", customer.Ssn);
    }

    [Theory]
    [InlineData("123-45-678")]
    [InlineData("123-45-67890")]
    [InlineData("123-4A-6789")]
    public void CustomerCreate_WhenSsnIsInvalid_Throws(string ssn)
    {
        Assert.Throws<ArgumentException>(() => Customer.Create(
            "John",
            "Doe",
            "Street 1",
            "CA",
            "Acme",
            ssn,
            DateTimeOffset.UtcNow));
    }
}
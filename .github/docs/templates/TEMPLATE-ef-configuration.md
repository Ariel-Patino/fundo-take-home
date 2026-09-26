# TEMPLATE — EF Core Configuration

```csharp
namespace Fundo.Infrastructure.Persistence.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(c => c.LastName).HasMaxLength(100).IsRequired();
        builder.Property(c => c.CompanyName).HasMaxLength(200).IsRequired();
        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.UpdatedAt).IsRequired();

        builder.OwnsOne(c => c.Ssn, ssn =>
        {
            ssn.Property(s => s.Value).HasColumnName("ssn").HasMaxLength(11).IsRequired();
            ssn.HasIndex(s => s.Value).IsUnique();
        });

        builder.OwnsOne(c => c.Address, address =>
        {
            address.Property(a => a.Street).HasColumnName("street").HasMaxLength(200).IsRequired();
            address.Property(a => a.City).HasColumnName("city").HasMaxLength(100).IsRequired();
            address.Property(a => a.State).HasColumnName("state").HasMaxLength(2).IsRequired();
            address.Property(a => a.Zip).HasColumnName("zip").HasMaxLength(10).IsRequired();
        });
    }
}
```
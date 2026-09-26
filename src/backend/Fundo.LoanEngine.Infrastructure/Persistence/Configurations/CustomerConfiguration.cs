
using Fundo.LoanEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fundo.LoanEngine.Infrastructure.Persistence.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");
        builder.HasKey(c => c.Id);

        builder.HasIndex(c => c.Ssn)
               .IsUnique();

        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.UpdatedAt).IsRequired();
        builder.Property(c => c.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(c => c.LastName).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Address).IsRequired().HasMaxLength(250);
        builder.Property(c => c.State).IsRequired().HasMaxLength(2);
        builder.Property(c => c.CompanyName).IsRequired().HasMaxLength(150);
        builder.Property(c => c.Ssn).IsRequired().HasMaxLength(11);
    }
}
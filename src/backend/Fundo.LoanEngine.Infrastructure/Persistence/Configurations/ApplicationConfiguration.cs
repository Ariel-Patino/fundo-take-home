using Fundo.LoanEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApplicationEntity = Fundo.LoanEngine.Domain.Entities.Application;

namespace Fundo.LoanEngine.Infrastructure.Persistence.Configurations;

public sealed class ApplicationConfiguration : IEntityTypeConfiguration<ApplicationEntity>
{
    public void Configure(EntityTypeBuilder<ApplicationEntity> builder)
    {
        builder.ToTable("applications");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.RequestedAmount)
               .HasPrecision(18, 2)
               .IsRequired();

        builder.HasOne(a => a.Customer)
               .WithOne(c => c.Application)
               .HasForeignKey<ApplicationEntity>(a => a.CustomerId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => a.CustomerId).IsUnique();
        builder.Property(a => a.CreatedAt).IsRequired();
        builder.Property(a => a.UpdatedAt).IsRequired();
    }
}
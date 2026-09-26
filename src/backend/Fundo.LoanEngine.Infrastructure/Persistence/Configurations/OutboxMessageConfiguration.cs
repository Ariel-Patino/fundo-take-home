using Fundo.LoanEngine.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fundo.LoanEngine.Infrastructure.Persistence.Configurations;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages");
        builder.HasKey(message => message.Id);
        builder.Property(message => message.EventType).HasMaxLength(200).IsRequired();
        builder.Property(message => message.Payload).HasColumnType("jsonb").IsRequired();
        builder.Property(message => message.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(message => message.RetryCount).IsRequired();
        builder.Property(message => message.CreatedAt).IsRequired();
        builder.HasIndex(message => new { message.Status, message.NextAttemptAt, message.CreatedAt });
    }
}
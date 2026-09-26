using Fundo.LoanEngine.Domain.Entities;
using Fundo.LoanEngine.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using ApplicationEntity = Fundo.LoanEngine.Domain.Entities.Application;

namespace Fundo.LoanEngine.Infrastructure.Persistence;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<ApplicationEntity> Applications => Set<ApplicationEntity>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
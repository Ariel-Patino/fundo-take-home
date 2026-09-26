# SKILL — Backend Infrastructure

## Purpose
Adapters: EF Core, Npgsql, HttpClient, background queue. References Application + Domain.

## Allowed Packages
- `Microsoft.EntityFrameworkCore`
- `Npgsql.EntityFrameworkCore.PostgreSQL`
- `Microsoft.Extensions.Http`
- `Microsoft.Extensions.Hosting`
- `System.Text.Json` (built-in)

## Structure
Fundo.Infrastructure/
├── Persistence/
│ ├── FundoDbContext.cs
│ ├── Configurations/
│ │ ├── CustomerConfiguration.cs
│ │ └── LoanApplicationConfiguration.cs
│ ├── Repositories/
│ │ ├── CustomerRepository.cs
│ │ └── LoanApplicationRepository.cs
│ ├── UnitOfWork.cs
│ └── Migrations/
├── Outbox/
│ ├── OutboxMessage.cs
│ ├── OutboxPublisher.cs
│ └── OutboxBackgroundService.cs
├── ExternalServices/
│ └── ExternalLoanServiceClient.cs
├── DependencyInjection.cs

## EF Core Rules
- `DbContext` is `sealed`, uses primary constructor for options.
- One `IEntityTypeConfiguration<T>` per entity, in `Configurations/`.
- `UseSnakeCaseNamingConvention()` (from `EFCore.NamingConventions`) for Postgres.
- Transactions via `IUnitOfWork.SaveChangesAsync()` — see `SKILL-transactional-outbox.md`.
- No lazy loading. Explicit `Include` only when necessary.
- Value objects mapped via `OwnsOne` / `OwnsMany`.

## HttpClient Rules
- Register with `AddHttpClient<IExternalLoanService, ExternalLoanServiceClient>()`.
- Set `BaseAddress` from config.
- Add `Polly` retries ONLY on the background worker (3 retries, exponential).
- Timeout: 10s.

## Checklist
- [ ] No business logic in repositories
- [ ] No `DbContext` leaking through ports
- [ ] Snake_case naming
- [ ] Migrations committed
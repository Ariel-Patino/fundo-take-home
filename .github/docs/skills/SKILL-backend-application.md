# SKILL — Backend Application

## Purpose
Use cases (orchestration), ports (interfaces), DTOs. Depends ONLY on Domain.

## Rules
- References: `Fundo.Domain` only.
- Optional: `FluentValidation` if it earns its place. Otherwise hand-rolled validators.
- No EF Core, no ASP.NET, no HttpClient here — define PORTS instead.

## Structure
Fundo.Application/
├── Abstractions/
│ ├── Persistence/
│ │ ├── ICustomerRepository.cs
│ │ ├── ILoanApplicationRepository.cs
│ │ └── IUnitOfWork.cs
│ ├── Messaging/
│ │ └── IEventPublisher.cs
│ └── Time/
│ └── IClock.cs
├── Customers/
│ └── GetOrCreateCustomer/
│ ├── GetOrCreateCustomerCommand.cs
│ ├── GetOrCreateCustomerHandler.cs
│ └── GetOrCreateCustomerResult.cs
├── Applications/
│ ├── SubmitApplication/
│ │ ├── SubmitApplicationCommand.cs
│ │ ├── SubmitApplicationHandler.cs
│ │ └── SubmitApplicationResult.cs
│ └── Rules/
│ ├── IDenyRule.cs
│ └── RuleEngine.cs
└── DependencyInjection.cs

## Ports First
Every infrastructure concern becomes an interface here:
- `ICustomerRepository`, `ILoanApplicationRepository`, `IUnitOfWork`
- `IEventPublisher` (or `IOutboxWriter`)
- `IExternalLoanService` (used only by the background handler)
- `IClock` (for deterministic tests)

## Handlers
- One class per use case. `sealed`.
- Constructor injection of ports.
- Return a `Result<T>` record, not exceptions for expected flows.
- No `async void`. Always `Task<T>`.

## Rule Engine
See `SKILL-rule-engine.md`.

## Checklist
- [ ] Only references Domain
- [ ] No infra types leak into signatures
- [ ] Handlers are `sealed`
- [ ] Result types are records
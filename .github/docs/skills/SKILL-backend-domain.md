# SKILL — Backend Domain

## Purpose
Pure business model. Zero dependencies. No EF, no ASP.NET, no Newtonsoft, no nothing.

## Rules
- Project file MUST NOT contain ANY `<PackageReference>` except the .NET SDK implicit ones.
- No attributes from external libs.
- Entities expose behavior, not public setters.
- Value Objects for SSN, Address, Money when it earns its place.
- Domain events as plain records, raised by aggregates (no MediatR here).

## Allowed Contents
- Entities (Customer, LoanApplication)
- Value Objects (Ssn, Address, Money, State)
- Domain exceptions (e.g., `InvalidSsnException`)
- Domain events (`LoanApplicationApprovedEvent`, `CustomerCreatedEvent`)
- Pure interfaces for domain services ONLY if a rule genuinely needs one

## Forbidden
- `using Microsoft.*`
- `using System.Data.*`
- Anemic models with public getters/setters everywhere
- Static helpers that mutate global state

## Naming
- Entities: singular nouns (`Customer`, `LoanApplication`)
- Value Objects: singular, immutable (`Ssn`, `Address`)
- Exceptions: `XxxException`

## Checklist
- [ ] No external references
- [ ] Business invariants enforced in constructors/factories
- [ ] No public setters unless strictly required by EF (use private setters + backing config)
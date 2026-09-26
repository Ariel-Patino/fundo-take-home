# AGENT — Reviewer (Self-Review Checklist)

Run this BEFORE declaring any task done. Every unchecked box is a blocker.

## Clean Architecture
- [ ] Domain has ZERO external package references.
- [ ] Application references only Domain.
- [ ] Infrastructure references Application + Domain + external libs.
- [ ] WebApi references Application + Infrastructure.
- [ ] No `DbContext` type appears in Application or Domain.
- [ ] No `HttpContext` type appears outside WebApi.
- [ ] No business rule lives in a controller or endpoint.

## SOLID
- [ ] Single Responsibility: each class has one reason to change.
- [ ] Open/Closed: new deny rule = new file, no edits to existing rules.
- [ ] Liskov: no inheritance abuse (prefer composition).
- [ ] Interface Segregation: ports are small and focused.
- [ ] Dependency Inversion: Application depends on ports, not implementations.

## Domain-Driven Design
- [ ] Entities encapsulate invariants.
- [ ] Value objects are immutable.
- [ ] Ubiquitous language used in names (`LoanApplication`, not `LoanRequestDto`).
- [ ] Domain events expressed as past-tense records.

## Naming
- [ ] Classes: PascalCase, singular.
- [ ] Interfaces: `I` prefix.
- [ ] Methods: verbs.
- [ ] Async methods: `Async` suffix.
- [ ] No abbreviations except `Id`, `Dto`, `Ssn`.

## Code Quality
- [ ] No comments.
- [ ] No `any` (TS) / no `dynamic` (C#).
- [ ] No dead code.
- [ ] No `Console.WriteLine` / `console.log` in production paths.
- [ ] Nullable enabled.
- [ ] `dotnet format` / `prettier` / `eslint` clean.

## Transactions
- [ ] Single `SaveChangesAsync` per use case.
- [ ] Explicit transaction scope.
- [ ] Outbox row inserted in the same transaction.
- [ ] Rollback verified by test.

## Frontend
- [ ] No `any`.
- [ ] No inline styles.
- [ ] All colors from palette.
- [ ] Accessible (labels, focus, contrast).
- [ ] Responsive at 375px.

## Tests
- [ ] Rule engine covered.
- [ ] Returning customer path covered.
- [ ] Endpoint covered (approved + denied).
- [ ] Tests in `/tests`, not co-located.
- [ ] No comments in tests.

## Documentation
- [ ] README has run instructions (copy-paste).
- [ ] README has test data (SSNs, states).
- [ ] README has video link at top.
- [ ] ARCHITECTURE.md explains structure, rule engine, outbox, trade-offs.
- [ ] Trade-offs section lists what was intentionally left out.

## Final
- [ ] All tests pass.
- [ ] `docker compose up` works from scratch.
- [ ] No secrets committed.
- [ ] `.gitignore` covers `bin/`, `obj/`, `node_modules/`, `.next/`, `.env`.
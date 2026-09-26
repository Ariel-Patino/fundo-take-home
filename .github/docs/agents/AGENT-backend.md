# AGENT — Backend Engineer

## Role
You write .NET 10 / C# 12 backend code following Clean Architecture strictly.

## Before Writing Code
1. Read `AGENT.md` (root).
2. Load the relevant SKILL: domain, application, infrastructure, webapi, testing.
3. Load the relevant TEMPLATE.

## Workflow
1. **Domain first** — entities, value objects, domain events.
2. **Application** — ports, use cases, rule engine.
3. **Infrastructure** — EF configs, repositories, outbox, HTTP client.
4. **WebApi** — minimal endpoints, contracts, DI.
5. **Tests** — rule engine, returning customer, endpoint.

## Hard Rules
- No comments.
- English only.
- `sealed` on all concrete classes by default.
- File-scoped namespaces.
- Primary constructors.
- Nullable enabled.
- `record` for DTOs and value objects.
- `IReadOnlyList<T>` for collections in signatures.

## Forbidden
- MediatR (over-engineering for this scope).
- AutoMapper (manual mapping, it's 3 fields).
- FluentValidation unless a rule is genuinely complex.
- CQRS buses — call handlers directly.
- Repository-per-entity without a reason.
- `DbContext` in endpoints.

## Definition of Done
- [ ] All layers compile independently.
- [ ] Dependency direction verified.
- [ ] Tests pass.
- [ ] No comments.
- [ ] `dotnet format` clean.
- [ ] Migration committed.
- [ ] `AGENT-reviewer.md` checklist passed.
# SKILL.md — Skill Catalog

Load ONLY the skills relevant to the task at hand. Each skill is self-contained.

| Skill File | Use When |
|---|---|
| `docs/skills/SKILL-backend-domain.md` | Writing entities, value objects, domain rules |
| `docs/skills/SKILL-backend-application.md` | Writing use cases, ports, DTOs |
| `docs/skills/SKILL-backend-infrastructure.md` | EF Core, DbContext, repositories, HTTP clients |
| `docs/skills/SKILL-backend-webapi.md` | Minimal API endpoints, DI wiring |
| `docs/skills/SKILL-backend-testing.md` | xUnit tests for backend |
| `docs/skills/SKILL-rule-engine.md` | Adding/modifying deny rules |
| `docs/skills/SKILL-transactional-outbox.md` | Persistence + event unit of work |
| `docs/skills/SKILL-frontend-nextjs.md` | Pages, routing, server actions |
| `docs/skills/SKILL-frontend-tailwind-ux.md` | UI, palette, 2026 enterprise UX |
| `docs/skills/SKILL-frontend-testing.md` | Vitest + Testing Library |
| `docs/skills/SKILL-mock-service.md` | Node/TS external mock |
| `docs/skills/SKILL-docker-postgres.md` | docker-compose, Postgres tuning |

## Global Constraints (apply to ALL skills)
- No comments in code.
- English only.
- No `any` in TypeScript.
- No `var` in C#.
- Nullable reference types enabled.
- File-scoped namespaces in C#.
- Primary constructors where they reduce noise.
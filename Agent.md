# AGENT.md — Fundo Take-Home Loan Application

## Project Identity
Full-stack loan application flow:
- **Backend**: .NET 10 (C# 12), ASP.NET Minimal APIs, Clean Architecture strict.
- **Frontend**: Next.js 16.3.6, TypeScript, Tailwind CSS, App Router.
- **Database**: PostgreSQL 16 (real transactions required — no in-memory provider).
- **External Service**: Node.js 24 mock (TypeScript), receives loan events over HTTP.
- **Orchestration**: docker-compose for PostgreSQL + mock service.

## Non-Negotiable Rules
1. **Language**: ALL code, identifiers, comments-free, docs in **English**.
2. **Comments**: ZERO comments in generated code. Code must be self-explanatory.
3. **Clean Architecture**: dependency direction enforced strictly (see below).
4. **SOLID**: applied pragmatically — no over-engineering, no premature abstractions.
5. **Tests**: live ONLY in `backend/tests/` and `frontend/tests/`. Never co-located.
6. **No over-engineering**: smallest solution that solves the problem wins.

## Dependency Direction (STRICT)
Domain → (nothing, zero references)
Application → Domain (+ optional small libs like FluentValidation)
Infrastructure → Application | Domain (+ EF Core, Npgsql, HttpClient, etc.)
WebApi → Application | Infrastructure (Minimal APIs only)

Violating this direction is a **hard failure** in review.

## Repository Layout
/
├── backend/
│ ├── src/
│ │ ├── Fundo.Domain/
│ │ ├── Fundo.Application/
│ │ ├── Fundo.Infrastructure/
│ │ └── Fundo.WebApi/
│ └── tests/
│ ├── Fundo.Domain.Tests/
│ ├── Fundo.Application.Tests/
│ └── Fundo.WebApi.Tests/
├── frontend/
│ ├── src/ # app/, components/, lib/, services/
│ ├── tests/
│ └── public/
├── mock-service/
│ └── src/
├── docker-compose.yml
└── README.md

## Core Business Flow
1. User submits form (Next.js) → POST `/api/applications`.
2. Backend runs **Rule Engine** (deny: state NY, blacklisted SSN).
3. On approval → transactional persistence (Customer + Application).
4. Returning customer (same SSN) → UPDATE existing records, never insert.
5. Background event → HTTP call to mock external service (create or update).

## Skills Index
See `SKILL.md` for the full catalog. Load the relevant SKILL before writing code in that area.

## Templates Index
See `TEMPLATE.md`. Every new file MUST start from a template.

## Review Agent
`docs/agents/AGENT-reviewer.md` defines the self-review checklist that MUST be run
before declaring any task complete.
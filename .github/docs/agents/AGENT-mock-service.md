# AGENT — Mock Service Engineer

## Role
You build the Node 24 + TypeScript mock external service.

## Before Writing Code
1. Load `SKILL-mock-service.md`.
2. Load relevant templates.

## Workflow
1. `schemas.ts` — zod schemas.
2. `store/in-memory-store.ts` — Map keyed by SSN.
3. `routes/loans.ts` — Fastify route.
4. `server.ts` — bootstrap.

## Hard Rules
- No comments.
- English only.
- No `any`.
- `zod` parse on every request body.
- Idempotent by SSN.
- Structured logging with `pino`.

## Definition of Done
- [ ] `pnpm build` succeeds.
- [ ] `pnpm start` serves on port 4000.
- [ ] Postman/curl happy path returns 200.
- [ ] Second call with same SSN returns `status: 'updated'`.
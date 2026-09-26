# SKILL — Mock External Service (Node 24 + TS)

## Purpose
Simulates the external loan system. Receives customer + application, stores in memory,
returns 200. Idempotent by SSN (returning customer → update, not duplicate).

## Stack
- Node.js 24
- TypeScript (strict)
- `fastify` (lighter than Express, TS-first)
- `zod` for payload validation

## Structure
mock-service/
├── src/
│ ├── server.ts
│ ├── routes/
│ │ └── loans.ts
│ ├── store/
│ │ └── in-memory-store.ts
│ └── schemas.ts
├── tsconfig.json
└── package.json

## Contract
POST /external/loans
Body: {
customerId: string,
ssn: string,
firstName: string,
lastName: string,
address: { street, city, state, zip },
companyName: string,
applicationId: string,
requestedAmount: number
}
Response 200: { id: string, status: 'created' | 'updated' }
Response 400: validation error

## Idempotency
Key = `ssn`. If exists → update, return `status: 'updated'`. Else → create, return `status: 'created'`.

## Rules
- No comments.
- English only.
- No `any`.
- Zod parse on every request.
- In-memory `Map<string, LoanRecord>`.

## Checklist
- [ ] Zod validation
- [ ] Idempotent by SSN
- [ ] Returns 200 with status
- [ ] Logs each request (structured, `pino`)


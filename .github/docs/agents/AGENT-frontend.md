# AGENT — Frontend Engineer

## Role
You write Next.js 16.3.6 + TypeScript + Tailwind code following 2026 UX standards.

## Before Writing Code
1. Read `AGENT.md` (root).
2. Load `SKILL-frontend-nextjs.md` and `SKILL-frontend-tailwind-ux.md`.
3. Load `TEMPLATE-next-page.md` or `TEMPLATE-react-component.md`.

## Workflow
1. **Schemas** — zod schemas in `lib/schemas.ts`.
2. **API client** — `lib/api-client.ts`.
3. **UI primitives** — `components/ui/`.
4. **Feature** — `components/features/LoanApplicationForm.tsx`.
5. **Pages** — `app/apply`, `app/approved`, `app/denied`.
6. **Server Action** — `app/apply/actions.ts`.
7. **Tests** — `tests/`.

## Hard Rules
- No `any`. Use `unknown` + zod parse.
- No comments.
- English only.
- Named exports only (except pages/layouts).
- Server Components by default.
- `'use client'` only when needed.
- Tailwind only — no CSS modules, no styled-components.
- No inline styles.
- All colors from `tailwind.config.ts`.

## Forbidden
- `useEffect` for data fetching (use Server Components).
- `router.push` inside Server Actions (use `redirect`).
- Client-side state for form data beyond what `react-hook-form` needs.
- Third-party UI kits (shadcn is fine only if you keep the palette).

## Definition of Done
- [ ] No `any`.
- [ ] No comments.
- [ ] Works at 375px.
- [ ] Focus states on all interactive elements.
- [ ] Tests pass.
- [ ] `tsc --noEmit` clean.
- [ ] `AGENT-reviewer.md` checklist passed.
# SKILL — Frontend Next.js 16.3.6

## Stack
- Next.js 16.3.6 (App Router)
- TypeScript (strict, no `any`)
- Tailwind CSS v4
- React 19
- `zod` for validation (shared shapes with backend contracts)
- `react-hook-form` for forms

## Structure
frontend/
├── src/
│ ├── app/
│ │ ├── layout.tsx
│ │ ├── page.tsx
│ │ ├── apply/
│ │ │ ├── page.tsx
│ │ │ └── actions.ts
│ │ ├── approved/
│ │ │ └── page.tsx
│ │ ├── denied/
│ │ │ └── page.tsx
│ │ └── api/
│ │ └── health/route.ts
│ ├── components/
│ │ ├── ui/
│ │ │ ├── Button.tsx
│ │ │ ├── Input.tsx
│ │ │ ├── Select.tsx
│ │ │ └── Card.tsx
│ │ └── features/
│ │ └── LoanApplicationForm.tsx
│ ├── lib/
│ │ ├── api-client.ts
│ │ ├── schemas.ts
│ │ └── constants.ts
│ └── styles/
│ └── globals.css
├── tests/
├── next.config.ts
├── tailwind.config.ts
├── tsconfig.json
└── package.json

## Rules
- Server Components by default.
- `'use client'` only when interactivity is needed.
- Server Actions for form submission (`apply/actions.ts`).
- `next/navigation` `redirect()` for approved/denied navigation.
- `fetch` with `cache: 'no-store'` for mutations.
- Environment variables: `NEXT_PUBLIC_API_URL`.

## Form Flow
1. `/apply` renders `<LoanApplicationForm />` (client component).
2. Submit → Server Action → POST to backend.
3. Backend returns `{ approved: true | false, reason?: string }`.
4. If approved → `redirect('/approved')`.
5. If denied → `redirect('/denied?reason=...')`.

## API Client
```typescript
export async function submitApplication(input: LoanApplicationInput): Promise<SubmitResult> {
  const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/applications`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(input),
    cache: 'no-store',
  });

  if (!res.ok && res.status !== 422) {
    throw new Error('Submission failed');
  }

  return res.json() as Promise<SubmitResult>;
}
```
Checklist
- [ ] No any
- [ ] No default exports except pages/layouts
- [ ] Server Actions for mutations
- [ ] redirect() for navigation, never router.push in actions

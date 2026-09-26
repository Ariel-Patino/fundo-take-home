# SKILL — Frontend Testing

## Location
`frontend/tests/`

## Stack
- Vitest
- `@testing-library/react`
- `@testing-library/user-event`
- `msw` for API mocking

## Structure
frontend/tests/
├── unit/
│ ├── schemas.test.ts
│ └── api-client.test.ts
├── components/
│ └── LoanApplicationForm.test.tsx
└── setup.ts

## What to Test
1. Form validation (zod schema).
2. Form submission calls the correct API.
3. Error state renders when API fails.
4. Redirect logic on approved/denied.

## What NOT to Test
- Tailwind classes.
- Next.js internals.
- Snapshot tests (banned).

## Example
```tsx
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { LoanApplicationForm } from '@/components/features/LoanApplicationForm';

describe('LoanApplicationForm', () => {
  it('shows validation error when SSN is empty', async () => {
    render(<LoanApplicationForm />);
    await userEvent.click(screen.getByRole('button', { name: /submit/i }));
    expect(await screen.findByText(/ssn is required/i)).toBeInTheDocument();
  });
});
```
Checklist
- [ ] Tests query by role, not by class
- [ ] No waitFor with arbitrary timeouts
- [ ] MSW handlers reset between tests
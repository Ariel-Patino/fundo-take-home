# TEMPLATE — Next.js Page

```tsx
import { redirect } from 'next/navigation';

export default function ApplyPage() {
  return (
    <main className="min-h-screen bg-surface">
      <div className="mx-auto max-w-xl px-4 py-16">
        <header className="mb-8 space-y-2">
          <h1 className="text-2xl font-semibold text-ink-900">Apply for a loan</h1>
          <p className="text-sm text-ink-500">
            Fill in your details. We will review your application instantly.
          </p>
        </header>

        <LoanApplicationForm />
      </div>
    </main>
  );
}
import Link from 'next/link';

interface DeniedPageProps {
  searchParams: Promise<{ reason?: string }>;
}

export default async function DeniedPage({ searchParams }: DeniedPageProps) {
  const { reason } = await searchParams;

  return (
    <main className="flex min-h-screen items-center justify-center px-4 py-12">
      <section className="w-full max-w-lg rounded-2xl border border-ink-100 border-l-4 border-l-danger-500 bg-white p-7 shadow-card sm:p-9">
        <span className="mb-5 flex size-12 items-center justify-center rounded-full bg-danger-50 text-xl font-semibold text-danger-700" aria-hidden="true">!</span>
        <h1 className="text-2xl font-semibold tracking-tight text-ink-900">Application not approved</h1>
        <p className="mt-2 text-sm leading-6 text-ink-500">We’re unable to approve your application at this time.</p>

        {reason && (
          <div className="mt-5 rounded-lg border border-danger-200 bg-danger-50 p-4 text-sm leading-6 text-danger-700">
            <p className="font-semibold">Decision details</p>
            <p className="mt-1">{reason}</p>
          </div>
        )}

        <Link
          href="/"
          className="mt-7 inline-flex min-h-11 items-center justify-center rounded-lg border border-ink-200 px-5 py-2.5 text-sm font-semibold text-ink-700 transition-colors hover:bg-ink-50 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-brand-500 focus-visible:ring-offset-2"
        >
          Return to application
        </Link>
      </section>
    </main>
  );
}
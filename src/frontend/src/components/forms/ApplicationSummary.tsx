'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import {
  applicationSummarySchema,
  applicationSummaryStorageKey,
  type ApplicationSummaryData,
} from '@/lib/schemas';

interface ApplicationSummaryProps {
  applicationId: string | null;
}

const currencyFormatter = new Intl.NumberFormat('en-US', {
  style: 'currency',
  currency: 'USD',
  maximumFractionDigits: 2,
});

export function ApplicationSummary({ applicationId }: ApplicationSummaryProps) {
  const [summary, setSummary] = useState<ApplicationSummaryData | null>(null);
  const [isLoaded, setIsLoaded] = useState(false);

  useEffect(() => {
    try {
      const storedSummary = sessionStorage.getItem(applicationSummaryStorageKey);
      sessionStorage.removeItem(applicationSummaryStorageKey);

      if (storedSummary) {
        const parsedSummary: unknown = JSON.parse(storedSummary);
        const validation = applicationSummarySchema.safeParse(parsedSummary);
        if (validation.success) setSummary(validation.data);
      }
    } catch {
      setSummary(null);
    } finally {
      setIsLoaded(true);
    }
  }, []);

  return (
    <main className="flex min-h-screen items-center justify-center px-4 py-12">
      <section className="w-full max-w-lg rounded-2xl border border-ink-100 border-l-4 border-l-success-500 bg-white p-7 shadow-card sm:p-9">
        <span className="mb-5 flex size-12 items-center justify-center rounded-full bg-success-50 text-xl font-semibold text-success-700" aria-hidden="true">✓</span>
        <h1 className="text-2xl font-semibold tracking-tight text-ink-900">Application approved</h1>
        <p className="mt-2 text-sm leading-6 text-ink-500">Your loan application has been approved and processed successfully.</p>

        {applicationId && (
          <p className="mt-4 rounded-lg bg-ink-50 px-4 py-3 text-sm text-ink-700">
            Reference: <span className="font-mono font-medium">{applicationId}</span>
          </p>
        )}

        {summary && (
          <div className="mt-6 border-t border-ink-100 pt-5">
            <h2 className="text-base font-semibold text-ink-900">Application summary</h2>
            <dl className="mt-4 divide-y divide-ink-100 rounded-lg border border-ink-100 px-4">
              <div className="flex flex-wrap justify-between gap-x-4 gap-y-1 py-3 text-sm">
                <dt className="text-ink-500">Applicant</dt>
                <dd className="font-medium text-ink-900">{summary.firstName} {summary.lastName}</dd>
              </div>
              <div className="flex flex-wrap justify-between gap-x-4 gap-y-1 py-3 text-sm">
                <dt className="text-ink-500">Address</dt>
                <dd className="text-right font-medium text-ink-900">{summary.address}, {summary.state}</dd>
              </div>
              <div className="flex flex-wrap justify-between gap-x-4 gap-y-1 py-3 text-sm">
                <dt className="text-ink-500">Company</dt>
                <dd className="font-medium text-ink-900">{summary.companyName}</dd>
              </div>
              <div className="flex flex-wrap justify-between gap-x-4 gap-y-1 py-3 text-sm">
                <dt className="text-ink-500">Requested amount</dt>
                <dd className="font-semibold text-ink-900">{currencyFormatter.format(summary.requestedAmount)}</dd>
              </div>
            </dl>
          </div>
        )}

        {isLoaded && !summary && (
          <p className="mt-5 rounded-lg bg-ink-50 px-4 py-3 text-sm leading-6 text-ink-500">
            Your application has been processed. The full summary is no longer available in this browser session.
          </p>
        )}

        <Link
          href="/"
          className="mt-7 inline-flex min-h-11 items-center justify-center rounded-lg bg-brand-700 px-5 py-2.5 text-sm font-semibold text-white transition-colors hover:bg-brand-800 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-brand-500 focus-visible:ring-offset-2"
        >
          Start another application
        </Link>
      </section>
    </main>
  );
}
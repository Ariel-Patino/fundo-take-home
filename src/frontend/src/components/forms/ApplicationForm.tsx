'use client';

import { zodResolver } from '@hookform/resolvers/zod';
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { useRouter } from 'next/navigation';
import { submitApplication } from '@/app/apply/actions';
import { DemoModePanel } from '@/components/forms/DemoModePanel';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Select } from '@/components/ui/select';
import {
  applicationSummaryStorageKey,
  loanApplicationSchema,
  usStates,
  type LoanApplicationInput,
} from '@/lib/schemas';

function formatSsn(value: string): string {
  const digits = value.replace(/\D/g, '').slice(0, 9);
  if (digits.length > 5) return `${digits.slice(0, 3)}-${digits.slice(3, 5)}-${digits.slice(5)}`;
  if (digits.length > 3) return `${digits.slice(0, 3)}-${digits.slice(3)}`;
  return digits;
}

export function ApplicationForm() {
  const router = useRouter();
  const [submissionError, setSubmissionError] = useState<string | null>(null);
  const form = useForm<LoanApplicationInput>({
    resolver: zodResolver(loanApplicationSchema),
    mode: 'onBlur',
    reValidateMode: 'onChange',
    defaultValues: {
      firstName: '',
      lastName: '',
      address: '',
      state: '',
      companyName: '',
      requestedAmount: undefined,
      ssn: '',
    },
  });

  const onSubmit = form.handleSubmit(async (data) => {
    setSubmissionError(null);
    const result = await submitApplication(data);
    if (result.status === 'error') {
      setSubmissionError(result.message);
      return;
    }

    if (result.status === 'Denied') {
      try {
        sessionStorage.removeItem(applicationSummaryStorageKey);
      } catch {}

      const reason = result.reason ? `?reason=${encodeURIComponent(result.reason)}` : '';
      router.replace(`/denied${reason}`);
      return;
    }

    const summary = {
      firstName: data.firstName,
      lastName: data.lastName,
      address: data.address,
      state: data.state,
      companyName: data.companyName,
      requestedAmount: data.requestedAmount,
    };

    try {
      sessionStorage.setItem(applicationSummaryStorageKey, JSON.stringify(summary));
    } catch {}

    const applicationId = result.applicationId ? `?applicationId=${encodeURIComponent(result.applicationId)}` : '';
    router.replace(`/success${applicationId}`);
  });

  function fillDemoFlow(data: LoanApplicationInput) {
    setSubmissionError(null);
    form.reset(data);
  }

  const errors = form.formState.errors;
  const ssnField = form.register('ssn');

  return (
    <section className="mx-auto w-full max-w-2xl px-4 py-10 sm:px-6 sm:py-14">
      <DemoModePanel onSelectFlow={fillDemoFlow} />
      <div className="mb-6 flex flex-wrap items-center gap-3" aria-label="Fundo application">
        <span className="flex size-10 items-center justify-center rounded-xl bg-brand-700 text-lg font-semibold text-white" aria-hidden="true">f</span>
        <span className="text-lg font-semibold tracking-tight text-ink-900">fundo</span>
        <span className="ml-auto rounded-full border border-brand-100 bg-white px-3 py-1 text-xs font-medium text-brand-800">Secure application</span>
      </div>

      <div className="overflow-hidden rounded-2xl border border-ink-100 bg-white shadow-card">
        <header className="border-b border-ink-100 px-6 py-7 sm:px-9 sm:py-8">
          <p className="mb-2 text-sm font-semibold uppercase tracking-[0.12em] text-brand-700">Personal loan</p>
          <h1 className="text-2xl font-semibold tracking-tight text-ink-900 sm:text-3xl">Let’s get started</h1>
          <p className="mt-2 max-w-prose text-sm leading-6 text-ink-500">Tell us a little about yourself and the amount you need. It only takes a few minutes.</p>
          <div className="mt-6 flex items-center gap-3" aria-label="Step 1 of 1: Application details">
            <span className="flex size-7 items-center justify-center rounded-full bg-brand-700 text-xs font-semibold text-white">1</span>
            <span className="text-sm font-medium text-ink-700">Application details</span>
            <span className="h-px flex-1 bg-ink-100" aria-hidden="true" />
            <span className="text-xs text-ink-500">About 3 minutes</span>
          </div>
        </header>

        <form onSubmit={onSubmit} noValidate className="space-y-6 px-6 py-7 sm:px-9 sm:py-8">
          {submissionError && (
            <div role="alert" className="rounded-lg border border-danger-200 bg-danger-50 px-4 py-3 text-sm text-danger-700">
              <p className="font-semibold">We couldn’t submit your application</p>
              <p className="mt-1">{submissionError}</p>
            </div>
          )}

          <fieldset disabled={form.formState.isSubmitting} className="space-y-6 disabled:opacity-70">
            <legend className="sr-only">Applicant information</legend>
            <div>
              <h2 className="text-base font-semibold text-ink-900">About you</h2>
              <p className="mt-1 text-sm text-ink-500">Use your legal name and current home address.</p>
            </div>

            <div className="grid grid-cols-1 gap-5 sm:grid-cols-2">
              <div>
                <Label htmlFor="firstName">First name</Label>
                <Input id="firstName" autoComplete="given-name" placeholder="e.g. Jordan" error={errors.firstName?.message} {...form.register('firstName')} />
              </div>
              <div>
                <Label htmlFor="lastName">Last name</Label>
                <Input id="lastName" autoComplete="family-name" placeholder="e.g. Lee" error={errors.lastName?.message} {...form.register('lastName')} />
              </div>
            </div>

            <div>
              <Label htmlFor="address">Street address</Label>
              <Input id="address" autoComplete="street-address" placeholder="Street address, apartment or suite" error={errors.address?.message} {...form.register('address')} />
            </div>

            <div className="grid grid-cols-1 gap-5 sm:grid-cols-2">
              <div>
                <Label htmlFor="state">State</Label>
                <Select id="state" autoComplete="address-level1" options={usStates} error={errors.state?.message} {...form.register('state')} />
              </div>
              <div>
                <Label htmlFor="companyName">Company name</Label>
                <Input id="companyName" autoComplete="organization" placeholder="Where you work" error={errors.companyName?.message} {...form.register('companyName')} />
              </div>
            </div>

            <div className="border-t border-ink-100 pt-6">
              <h2 className="text-base font-semibold text-ink-900">Loan details</h2>
              <p className="mt-1 text-sm text-ink-500">Enter the amount you’d like to request.</p>
            </div>

            <div className="grid grid-cols-1 gap-5 sm:grid-cols-2">
              <div>
                <Label htmlFor="requestedAmount">Requested amount</Label>
                <div className="relative">
                  <span className="pointer-events-none absolute inset-y-0 left-3 flex items-center text-sm text-ink-500" aria-hidden="true">$</span>
                  <Input id="requestedAmount" type="number" inputMode="decimal" min="1" max="1000000" step="0.01" placeholder="5,000" className="pl-8" error={errors.requestedAmount?.message} {...form.register('requestedAmount', { valueAsNumber: true })} />
                </div>
              </div>
              <div>
                <Label htmlFor="ssn">Social Security number</Label>
                <Input
                  id="ssn"
                  type="text"
                  inputMode="numeric"
                  autoComplete="off"
                  placeholder="123-45-6789"
                  maxLength={11}
                  hint="Format: 123-45-6789"
                  error={errors.ssn?.message}
                  {...ssnField}
                  onChange={(event) => {
                    void form.setValue('ssn', formatSsn(event.target.value), { shouldDirty: true, shouldValidate: form.formState.touchedFields.ssn });
                  }}
                />
              </div>
            </div>
          </fieldset>

          <div className="border-t border-ink-100 pt-6">
            <Button type="submit" isLoading={form.formState.isSubmitting}>
              {form.formState.isSubmitting ? 'Submitting application…' : 'Review my application'}
            </Button>
            <p className="mt-3 text-center text-xs leading-5 text-ink-500">By continuing, you confirm the information above is accurate.</p>
          </div>
        </form>
      </div>

      <p className="mt-5 text-center text-xs text-ink-500">Your information is transmitted securely and used to review your application.</p>
    </section>
  );
}
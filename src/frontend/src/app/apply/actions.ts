'use server';

import type { LoanApplicationInput } from '@/lib/schemas';
import type { ApplicationResponse } from '@/types';

export type SubmissionActionResult =
  | { status: 'error'; message: string }
  | { status: 'Approved'; applicationId: string | null }
  | { status: 'Denied'; reason: string | null };

export async function submitApplication(
  input: LoanApplicationInput,
): Promise<SubmissionActionResult> {
  const backendUrl = process.env.BACKEND_API_URL;
  if (!backendUrl) {
    return { status: 'error', message: 'Application service is not configured. Please try again later.' };
  }

  let result: ApplicationResponse;

  try {
    const response = await fetch(`${backendUrl}/api/applications`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(input),
      cache: 'no-store',
    });

    if (!response.ok && response.status !== 422) {
      return { status: 'error', message: 'We could not submit your application. Please try again.' };
    }

    result = await response.json() as ApplicationResponse;
  } catch {
    return { status: 'error', message: 'We could not reach the application service. Check your connection and try again.' };
  }

  if (result.status !== 'Approved' && result.status !== 'Denied') {
    return { status: 'error', message: 'The application service returned an unexpected response. Please try again.' };
  }

  if (result.status === 'Approved') {
    return { status: 'Approved', applicationId: result.applicationId ?? null };
  }

  return { status: 'Denied', reason: result.reason ?? null };
}
import type { LoanApplicationInput } from '@/lib/schemas';

export type ApplicationFormData = LoanApplicationInput;

export interface ApplicationResponse {
  status: 'Approved' | 'Denied';
  reason?: string | null;
  customerId?: string;
  applicationId?: string;
}
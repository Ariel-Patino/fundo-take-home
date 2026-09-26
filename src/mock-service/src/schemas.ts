import { z } from 'zod';

export const applicationPayloadSchema = z.object({
  customerId: z.string().uuid(),
  applicationId: z.string().uuid(),
  firstName: z.string().trim().min(1).max(100),
  lastName: z.string().trim().min(1).max(100),
  address: z.string().trim().min(1).max(250),
  state: z.string().trim().regex(/^[A-Z]{2}$/),
  companyName: z.string().trim().min(1).max(150),
  requestedAmount: z.number().finite().positive(),
  ssn: z.string().trim().regex(/^\d{3}-\d{2}-\d{4}$/),
  isReturningCustomer: z.boolean(),
});

export type ApplicationPayload = z.infer<typeof applicationPayloadSchema>;
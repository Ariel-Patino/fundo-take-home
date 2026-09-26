import { z } from 'zod';

export const loanApplicationSchema = z.object({
  firstName: z.string().trim().min(1, 'Enter your first name.').max(100, 'First name must be 100 characters or fewer.'),
  lastName: z.string().trim().min(1, 'Enter your last name.').max(100, 'Last name must be 100 characters or fewer.'),
  address: z.string().trim().min(1, 'Enter your street address.').max(250, 'Address must be 250 characters or fewer.'),
  state: z.string().length(2, 'Select a state.'),
  companyName: z.string().trim().min(1, 'Enter your company name.').max(150, 'Company name must be 150 characters or fewer.'),
  requestedAmount: z.number({ error: 'Enter a loan amount.' })
    .finite('Enter a valid loan amount.')
    .positive('The amount must be greater than $0.')
    .max(1_000_000, 'The amount must be $1,000,000 or less.'),
  ssn: z.string().trim().regex(/^\d{3}-\d{2}-\d{4}$/, 'Use the format 123-45-6789.'),
});

export type LoanApplicationInput = z.infer<typeof loanApplicationSchema>;

export const applicationSummarySchema = z.object({
  firstName: z.string(),
  lastName: z.string(),
  address: z.string(),
  state: z.string().length(2),
  companyName: z.string(),
  requestedAmount: z.number().positive(),
});

export type ApplicationSummaryData = z.infer<typeof applicationSummarySchema>;
export const applicationSummaryStorageKey = 'fundo:processed-application-summary';

export const usStates = [
  { label: 'Alabama', value: 'AL' },
  { label: 'Alaska', value: 'AK' },
  { label: 'Arizona', value: 'AZ' },
  { label: 'Arkansas', value: 'AR' },
  { label: 'California', value: 'CA' },
  { label: 'Colorado', value: 'CO' },
  { label: 'Connecticut', value: 'CT' },
  { label: 'Delaware', value: 'DE' },
  { label: 'Florida', value: 'FL' },
  { label: 'Georgia', value: 'GA' },
  { label: 'Hawaii', value: 'HI' },
  { label: 'Idaho', value: 'ID' },
  { label: 'Illinois', value: 'IL' },
  { label: 'Indiana', value: 'IN' },
  { label: 'Iowa', value: 'IA' },
  { label: 'Kansas', value: 'KS' },
  { label: 'Kentucky', value: 'KY' },
  { label: 'Louisiana', value: 'LA' },
  { label: 'Maine', value: 'ME' },
  { label: 'Maryland', value: 'MD' },
  { label: 'Massachusetts', value: 'MA' },
  { label: 'Michigan', value: 'MI' },
  { label: 'Minnesota', value: 'MN' },
  { label: 'Mississippi', value: 'MS' },
  { label: 'Missouri', value: 'MO' },
  { label: 'Montana', value: 'MT' },
  { label: 'Nebraska', value: 'NE' },
  { label: 'Nevada', value: 'NV' },
  { label: 'New Hampshire', value: 'NH' },
  { label: 'New Jersey', value: 'NJ' },
  { label: 'New Mexico', value: 'NM' },
  { label: 'New York', value: 'NY' },
  { label: 'North Carolina', value: 'NC' },
  { label: 'North Dakota', value: 'ND' },
  { label: 'Ohio', value: 'OH' },
  { label: 'Oklahoma', value: 'OK' },
  { label: 'Oregon', value: 'OR' },
  { label: 'Pennsylvania', value: 'PA' },
  { label: 'Rhode Island', value: 'RI' },
  { label: 'South Carolina', value: 'SC' },
  { label: 'South Dakota', value: 'SD' },
  { label: 'Tennessee', value: 'TN' },
  { label: 'Texas', value: 'TX' },
  { label: 'Utah', value: 'UT' },
  { label: 'Vermont', value: 'VT' },
  { label: 'Virginia', value: 'VA' },
  { label: 'Washington', value: 'WA' },
  { label: 'West Virginia', value: 'WV' },
  { label: 'Wisconsin', value: 'WI' },
  { label: 'Wyoming', value: 'WY' },
];
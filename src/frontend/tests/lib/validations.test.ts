import { loanApplicationSchema } from '@/lib/schemas';

const validApplication = {
  firstName: 'Jordan',
  lastName: 'Lee',
  address: '123 Main Street',
  state: 'CA',
  companyName: 'Acme',
  requestedAmount: 5000,
  ssn: '123-45-6789',
};

const invalidFieldCases = [
  { field: 'firstName', value: '', message: 'Enter your first name.' },
  { field: 'lastName', value: ' ', message: 'Enter your last name.' },
  { field: 'address', value: '', message: 'Enter your street address.' },
  { field: 'state', value: '', message: 'Select a state.' },
  { field: 'companyName', value: '', message: 'Enter your company name.' },
  { field: 'requestedAmount', value: undefined, message: 'Enter a loan amount.' },
  { field: 'requestedAmount', value: 0, message: 'The amount must be greater than $0.' },
  { field: 'requestedAmount', value: 1_000_001, message: 'The amount must be $1,000,000 or less.' },
  { field: 'ssn', value: '', message: 'Use the format 123-45-6789.' },
  { field: 'ssn', value: '12345', message: 'Use the format 123-45-6789.' },
] as const;

describe('loanApplicationSchema', () => {
  it.each(invalidFieldCases)('rejects invalid $field values', ({ field, value, message }) => {
    const result = loanApplicationSchema.safeParse({ ...validApplication, [field]: value });

    expect(result.success).toBe(false);
    if (!result.success) {
      expect(result.error.issues).toContainEqual(expect.objectContaining({ path: [field], message }));
    }
  });

  it('accepts a valid application with a positive requested amount', () => {
    expect(loanApplicationSchema.safeParse(validApplication).success).toBe(true);
  });
});
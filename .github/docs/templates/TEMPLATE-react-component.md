# TEMPLATE — React Client Component

```tsx
'use client';

import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { loanApplicationSchema, type LoanApplicationInput } from '@/lib/schemas';
import { submitApplication } from '@/app/apply/actions';

export function LoanApplicationForm() {
  const form = useForm<LoanApplicationInput>({
    resolver: zodResolver(loanApplicationSchema),
    defaultValues: {
      firstName: '',
      lastName: '',
      companyName: '',
      requestedAmount: 0,
      ssn: '',
      address: { street: '', city: '', state: '', zip: '' },
    },
  });

  const onSubmit = form.handleSubmit(async (data) => {
    await submitApplication(data);
  });

  return (
    <form onSubmit={onSubmit} className="space-y-5">
      {/* fields */}
    </form>
  );
}
```
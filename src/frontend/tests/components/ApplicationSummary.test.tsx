import { render, screen, waitFor } from '@testing-library/react';
import { ApplicationSummary } from '@/components/forms/ApplicationSummary';
import { applicationSummaryStorageKey } from '@/lib/schemas';

describe('ApplicationSummary', () => {
  afterEach(() => {
    sessionStorage.clear();
  });

  it('shows the processed application details without retaining them in session storage', async () => {
    sessionStorage.setItem(applicationSummaryStorageKey, JSON.stringify({
      firstName: 'Jordan',
      lastName: 'Lee',
      address: '123 Main Street',
      state: 'CA',
      companyName: 'Acme',
      requestedAmount: 5000,
    }));

    render(<ApplicationSummary applicationId="application-123" />);

    expect(await screen.findByText('Application summary')).toBeInTheDocument();
    expect(screen.getByText('Jordan Lee')).toBeInTheDocument();
    expect(screen.getByText('123 Main Street, CA')).toBeInTheDocument();
    expect(screen.getByText('Acme')).toBeInTheDocument();
    expect(screen.getByText('$5,000.00')).toBeInTheDocument();
    expect(screen.getByText('application-123')).toBeInTheDocument();
    await waitFor(() => expect(sessionStorage.getItem(applicationSummaryStorageKey)).toBeNull());
  });
});
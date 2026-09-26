import { fireEvent, render, screen, waitFor } from '@testing-library/react';
import { ApplicationForm } from '@/components/forms/ApplicationForm';
import { applicationSummaryStorageKey } from '@/lib/schemas';

const mockRouterReplace = jest.fn();
jest.mock('next/navigation', () => ({ useRouter: () => ({ replace: mockRouterReplace }) }));

const originalFetch = globalThis.fetch;
const originalBackendApiUrl = process.env.BACKEND_API_URL;

afterEach(() => {
  globalThis.fetch = originalFetch;
  if (originalBackendApiUrl === undefined) {
    delete process.env.BACKEND_API_URL;
  } else {
    process.env.BACKEND_API_URL = originalBackendApiUrl;
  }
  sessionStorage.clear();
  mockRouterReplace.mockReset();
});

function fillValidApplication() {
  fireEvent.change(screen.getByLabelText(/first name/i), { target: { value: 'Jordan' } });
  fireEvent.change(screen.getByLabelText(/last name/i), { target: { value: 'Lee' } });
  fireEvent.change(screen.getByLabelText(/street address/i), { target: { value: '123 Main Street' } });
  fireEvent.change(screen.getByLabelText(/^state$/i), { target: { value: 'CA' } });
  fireEvent.change(screen.getByLabelText(/company name/i), { target: { value: 'Acme' } });
  fireEvent.change(screen.getByLabelText(/requested amount/i), { target: { value: '5000' } });
  fireEvent.change(screen.getByLabelText(/social security number/i), { target: { value: '123456789' } });
}

describe('ApplicationForm', () => {
  it('shows clear validation errors when submitted empty', async () => {
    const fetchMock = jest.fn();
    globalThis.fetch = fetchMock;
    render(<ApplicationForm />);

    fireEvent.click(screen.getByRole('button', { name: /review my application/i }));

    expect(await screen.findByText('Enter your first name.')).toBeInTheDocument();
    expect(screen.getByText('Enter your last name.')).toBeInTheDocument();
    expect(screen.getByText('Enter your street address.')).toBeInTheDocument();
    expect(screen.getByText('Select a state.')).toBeInTheDocument();
    expect(screen.getByText('Enter your company name.')).toBeInTheDocument();
    expect(screen.getByText('Enter a loan amount.')).toBeInTheDocument();
    expect(screen.getByText('Use the format 123-45-6789.')).toBeInTheDocument();
    expect(fetchMock).not.toHaveBeenCalled();
  });

  it('announces a recoverable submission error', async () => {
    process.env.BACKEND_API_URL = 'http://backend.test';
    let finishRequest: ((response: Response) => void) | undefined;
    const fetchMock = jest.fn(() => new Promise<Response>((resolve) => {
      finishRequest = resolve;
    }));
    globalThis.fetch = fetchMock;
    render(<ApplicationForm />);

    fillValidApplication();
    fireEvent.click(screen.getByRole('button', { name: /review my application/i }));

    await waitFor(() => expect(fetchMock).toHaveBeenCalled());
    expect(screen.getByRole('button', { name: /submitting application/i })).toBeDisabled();
    finishRequest?.({ ok: false, status: 503 } as Response);

    expect(await screen.findByRole('alert')).toHaveTextContent('We could not submit your application. Please try again.');
  });

  it('routes denied applications with the decision reason', async () => {
    process.env.BACKEND_API_URL = 'http://backend.test';
    const fetchMock = jest.fn().mockResolvedValue({
      ok: false,
      status: 422,
      json: async () => ({ status: 'Denied', reason: 'Applications from New York (NY) state are not allowed.' }),
    } as Response);
    globalThis.fetch = fetchMock;
    render(<ApplicationForm />);

    fillValidApplication();
    fireEvent.click(screen.getByRole('button', { name: /review my application/i }));

    await waitFor(() => expect(mockRouterReplace).toHaveBeenCalledWith(
      '/denied?reason=Applications%20from%20New%20York%20(NY)%20state%20are%20not%20allowed.',
    ));
    expect(fetchMock).toHaveBeenCalledWith(
      'http://backend.test/api/applications',
      expect.objectContaining({ method: 'POST', cache: 'no-store' }),
    );
    expect(sessionStorage.getItem(applicationSummaryStorageKey)).toBeNull();
  });

  it('routes approved applications with a safe request summary', async () => {
    process.env.BACKEND_API_URL = 'http://backend.test';
    const fetchMock = jest.fn().mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => ({ status: 'Approved', applicationId: 'application-123' }),
    } as Response);
    globalThis.fetch = fetchMock;
    render(<ApplicationForm />);

    fillValidApplication();
    fireEvent.click(screen.getByRole('button', { name: /review my application/i }));

    await waitFor(() => expect(mockRouterReplace).toHaveBeenCalledWith('/success?applicationId=application-123'));
    expect(fetchMock).toHaveBeenCalledWith(
      'http://backend.test/api/applications',
      expect.objectContaining({ method: 'POST', cache: 'no-store' }),
    );
    const storedSummary = JSON.parse(sessionStorage.getItem(applicationSummaryStorageKey) ?? 'null') as Record<string, unknown>;
    expect(storedSummary).toMatchObject({
      firstName: 'Jordan',
      lastName: 'Lee',
      address: '123 Main Street',
      state: 'CA',
      companyName: 'Acme',
      requestedAmount: 5000,
    });
    expect(storedSummary).not.toHaveProperty('ssn');
  });
});
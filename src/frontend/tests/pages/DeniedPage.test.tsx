import { render, screen } from '@testing-library/react';
import DeniedPage from '@/app/denied/page';

describe('DeniedPage', () => {
  it('shows the decision reason supplied by the application response', async () => {
    const page = await DeniedPage({
      searchParams: Promise.resolve({ reason: 'Applications from New York (NY) state are not allowed.' }),
    });

    render(page);

    expect(screen.getByRole('heading', { name: /application not approved/i })).toBeInTheDocument();
    expect(screen.getByText('Applications from New York (NY) state are not allowed.')).toBeInTheDocument();
    expect(screen.getByRole('link', { name: /return to application/i })).toHaveAttribute('href', '/');
  });
});
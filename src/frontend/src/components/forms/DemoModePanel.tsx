'use client';

import { useEffect, useState } from 'react';
import type { LoanApplicationInput } from '@/lib/schemas';

interface DemoFlow {
  label: string;
  data: LoanApplicationInput;
}

interface DemoModePanelProps {
  onSelectFlow: (data: LoanApplicationInput) => void;
}

export const demoFlows: readonly DemoFlow[] = [
  {
    label: 'Fill Flow 1-Approved CA',
    data: {
      firstName: 'Jordan',
      lastName: 'Lee',
      address: '123 Main Street',
      state: 'CA',
      companyName: 'Acme',
      requestedAmount: 5000,
      ssn: '555-55-0101',
    },
  },
  {
    label: 'Fill Flow 2-Denied NY',
    data: {
      firstName: 'Jordan',
      lastName: 'Lee',
      address: '123 Main Street',
      state: 'NY',
      companyName: 'Acme',
      requestedAmount: 5000,
      ssn: '555-55-0102',
    },
  },
  {
    label: 'Fill Flow 3-Denied SSN Blacklist',
    data: {
      firstName: 'Jordan',
      lastName: 'Lee',
      address: '123 Main Street',
      state: 'CA',
      companyName: 'Acme',
      requestedAmount: 5000,
      ssn: '000-00-0000',
    },
  },
  {
    label: 'Fill Flow 4-Returning Customer',
    data: {
      firstName: 'Jordan',
      lastName: 'Morgan',
      address: '456 Updated Avenue',
      state: 'CA',
      companyName: 'Acme Updated',
      requestedAmount: 6500,
      ssn: '555-55-0101',
    },
  },
];

export function DemoModePanel({ onSelectFlow }: DemoModePanelProps) {
  const [isVisible, setIsVisible] = useState(false);

  useEffect(() => {
    function handleKeyDown(event: KeyboardEvent) {
      if (event.key === 'Alt' && !event.ctrlKey && !event.shiftKey && !event.metaKey && !event.repeat) {
        event.preventDefault();
        setIsVisible((visible) => !visible);
      }

      if (event.key === 'Escape') {
        setIsVisible(false);
      }
    }

    window.addEventListener('keydown', handleKeyDown);
    return () => window.removeEventListener('keydown', handleKeyDown);
  }, []);

  if (!isVisible) return null;

  return (
    <aside
      aria-label="Demo mode controls"
      className="fixed bottom-4 right-4 z-50 w-[calc(100%-2rem)] max-w-sm rounded-xl border border-warning-200 bg-white p-4 shadow-card sm:bottom-6 sm:right-6"
    >
      <div className="mb-3 flex items-start justify-between gap-3">
        <div>
          <p className="text-sm font-semibold text-ink-900">Demo mode</p>
          <p className="mt-1 text-xs leading-5 text-ink-500">Prefills sample data only. Review and submit each flow yourself.</p>
        </div>
        <button
          type="button"
          aria-label="Close demo mode"
          onClick={() => setIsVisible(false)}
          className="flex size-8 shrink-0 items-center justify-center rounded-md text-ink-500 transition-colors hover:bg-ink-50 hover:text-ink-900 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-brand-500 focus-visible:ring-offset-2"
        >
          <span aria-hidden="true">×</span>
        </button>
      </div>
      <div className="grid gap-2">
        {demoFlows.map((flow) => (
          <button
            key={flow.label}
            type="button"
            onClick={() => onSelectFlow(flow.data)}
            className="min-h-10 rounded-lg border border-ink-200 px-3 py-2 text-left text-sm font-medium text-ink-700 transition-colors hover:border-brand-300 hover:bg-brand-50 hover:text-brand-900 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-brand-500 focus-visible:ring-offset-2"
          >
            {flow.label}
          </button>
        ))}
      </div>
      <p className="mt-3 text-xs text-ink-500">Press Alt or Esc to hide these controls.</p>
    </aside>
  );
}
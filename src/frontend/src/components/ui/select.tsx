import React from 'react';

interface SelectProps extends React.ComponentPropsWithoutRef<'select'> {
  error?: string;
  options: { label: string; value: string }[];
}

export const Select = React.forwardRef<HTMLSelectElement, SelectProps>(
  ({ options, error, className = '', id, 'aria-describedby': describedBy, ...props }, ref) => {
    const errorId = error && id ? `${id}-error` : undefined;
    const ariaDescribedBy = [describedBy, errorId].filter(Boolean).join(' ') || undefined;

    return (
      <div>
        <select
          ref={ref}
          id={id}
          aria-invalid={Boolean(error)}
          aria-describedby={ariaDescribedBy}
          className={`block min-h-11 w-full rounded-lg border bg-white px-3 py-2.5 text-base text-ink-900 shadow-sm outline-none transition-colors focus-visible:ring-2 focus-visible:ring-brand-500 focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:bg-ink-50 ${
            error ? 'border-danger-500' : 'border-ink-200 hover:border-ink-400'
          } ${className}`}
          {...props}
        >
          <option value="">Choose a state</option>
          {options.map((option) => (
            <option key={option.value} value={option.value}>
              {option.label} ({option.value})
            </option>
          ))}
        </select>
        {error && <p id={errorId} className="mt-1.5 text-xs font-medium text-danger-700">{error}</p>}
      </div>
    );
  },
);

Select.displayName = 'Select';
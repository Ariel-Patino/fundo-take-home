import React from 'react';

interface InputProps extends React.ComponentPropsWithoutRef<'input'> {
  error?: string;
  hint?: string;
}

export const Input = React.forwardRef<HTMLInputElement, InputProps>(
  ({ error, hint, className = '', id, 'aria-describedby': describedBy, ...props }, ref) => {
    const errorId = error && id ? `${id}-error` : undefined;
    const hintId = hint && id ? `${id}-hint` : undefined;
    const ariaDescribedBy = [describedBy, hintId, errorId].filter(Boolean).join(' ') || undefined;

    return (
      <div>
        <input
          ref={ref}
          id={id}
          aria-invalid={Boolean(error)}
          aria-describedby={ariaDescribedBy}
          className={`block min-h-11 w-full rounded-lg border bg-white px-3 py-2.5 text-base text-ink-900 shadow-sm outline-none transition-colors placeholder:text-ink-400 focus-visible:ring-2 focus-visible:ring-brand-500 focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:bg-ink-50 ${
            error ? 'border-danger-500' : 'border-ink-200 hover:border-ink-400'
          } ${className}`}
          {...props}
        />
        {hint && <p id={hintId} className="mt-1.5 text-xs text-ink-500">{hint}</p>}
        {error && <p id={errorId} className="mt-1.5 text-xs font-medium text-danger-700">{error}</p>}
      </div>
    );
  },
);

Input.displayName = 'Input';
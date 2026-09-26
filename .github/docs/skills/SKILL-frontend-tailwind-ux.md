# SKILL — Tailwind + UX 2026

## Palette (Enterprise Lending, 2026)
Calm, trustworthy, low-saturation. No neon. No gradients unless subtle.

```ts
// tailwind.config.ts theme.extend.colors
{
  brand: {
    50:  '#f4f7fb',
    100: '#e6eef7',
    200: '#c7d9ec',
    300: '#9bb8d8',
    400: '#6890bd',
    500: '#4a72a3',  // primary
    600: '#3a5b85',
    700: '#2f4a6c',
    800: '#283d58',
    900: '#1f2f44',
  },
  ink: {
    50:  '#f7f8f9',
    500: '#6b7280',
    700: '#374151',
    900: '#111827',
  },
  surface: '#fafbfc',
  success: { 500: '#2f855a' },
  warning: { 500: '#b7791f' },
  danger:  { 500: '#9b2c2c' },
}
```
### Typography
#### Font: Inter (via next/font/google).
#### Sizes: text-sm body, text-base inputs, text-2xl headings.
#### Weights: 400 body, 500 labels, 600 headings.

### Spacing
#### Use Tailwind scale only. No arbitrary values unless justified.
#### Form fields: space-y-5. Cards: p-8. Sections: space-y-8.

### Components
#### Button variants: primary | secondary | ghost | danger.
#### Input with label, error, hint slots.
#### Focus rings: focus-visible:ring-2 focus-visible:ring-brand-500 focus-visible:ring-offset-2.

### UX Principles (2026)
#### Clarity over decoration — every element earns its place.
#### Progressive disclosure — one decision per screen when possible.
#### Inline validation — validate on blur, not on every keystroke.
#### Accessible by default — ARIA labels, keyboard nav, contrast ≥ 4.5:1.
#### Responsive — mobile-first, md: and lg: breakpoints.
#### Motion — subtle (transition-colors, duration-150). No bouncy.
#### Empty/error/loading states — always designed, never forgotten.
#### No dark mode in v1 (state it in README if omitted).

### Form Layout
#### Card (max-w-xl, mx-auto, mt-16, p-8, rounded-xl, border border-ink-100, bg-white)
####  Header (title + subtitle)
#### Form (space-y-5)
    *** Row (grid grid-cols-2 gap-4) for First/Last name ***
    *** Address (street, city, state select, zip) ***
    *** Company name ***
    *** Requested amount (number input with $ prefix) ***
    *** SSN (masked input, format ###-##-####) ***
    *** Submit button (w-full, primary) ***

### Denied Page
#### Centered card.

### Danger accent (left border border-l-4 border-danger-500).
#### Reason text from query param.
#### "Try again" button → /apply.
#### Approved Page
####  Centered card.

### Success accent.

#### Application ID displayed.
#### "Start over" button.

### Checklist
- [ ] No inline style={} unless dynamic
- [ ] No hardcoded hex outside tailwind.config.ts
- [ ] All interactive elements have focus states
- [ ] All inputs have associated <label>
- [ ] All pages work at 375px width
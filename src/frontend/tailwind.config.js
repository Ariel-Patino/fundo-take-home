/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./src/**/*.{js,ts,jsx,tsx,mdx}"],
  theme: {
    extend: {
      colors: {
        brand: {
          50: '#f4f7fb',
          100: '#e6eef7',
          200: '#c7d9ec',
          300: '#9bb8d8',
          400: '#6890bd',
          500: '#4a72a3',
          600: '#3a5b85',
          700: '#2f4a6c',
          800: '#283d58',
          900: '#1f2f44',
        },
        ink: {
          50: '#f7f8f9',
          100: '#e7eaee',
          200: '#d1d5db',
          400: '#9ca3af',
          500: '#6b7280',
          700: '#374151',
          900: '#111827',
        },
        danger: {
          50: '#fdf2f2',
          200: '#e8b4b4',
          500: '#9b2c2c',
          700: '#7f1d1d',
        },
        success: {
          50: '#f0fdf4',
          500: '#2f855a',
          700: '#166534',
        },
      },
      boxShadow: {
        card: '0 16px 40px -20px rgba(31, 47, 68, 0.22)',
      },
    },
  },
  plugins: [],
};
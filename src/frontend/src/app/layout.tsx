import './globals.css';
import React from 'react';

export const metadata = {
  title: 'Fundo Loan Application',
  description: 'Loan Engine Application',
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en">
      <body className="min-h-screen">{children}</body>
    </html>
  );
}
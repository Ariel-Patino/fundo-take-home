import { ApplicationSummary } from '@/components/forms/ApplicationSummary';

interface SuccessPageProps {
  searchParams: Promise<{ applicationId?: string }>;
}

export default async function SuccessPage({ searchParams }: SuccessPageProps) {
  const { applicationId } = await searchParams;
  return <ApplicationSummary applicationId={applicationId ?? null} />;
}
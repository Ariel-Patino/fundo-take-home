import pino from 'pino';

export const logger = pino({
  level: process.env.LOG_LEVEL ?? 'info',
});

export function redactSsn(payload: unknown): unknown {
  if (typeof payload !== 'object' || payload === null || !('ssn' in payload)) {
    return payload;
  }

  const payloadRecord = payload as Record<string, unknown>;
  if (typeof payloadRecord.ssn !== 'string') {
    return payload;
  }

  return {
    ...payloadRecord,
    ssn: `***-**-${payloadRecord.ssn.slice(-4)}`,
  };
}
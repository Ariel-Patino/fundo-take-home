import type { ApplicationPayload } from '../schemas';

export interface UpsertResult {
  status: 'created' | 'updated';
  application: ApplicationPayload;
}

class MemoryStore {
  private readonly applications = new Map<string, ApplicationPayload>();

  public upsert(payload: ApplicationPayload): UpsertResult {
    const status = this.applications.has(payload.ssn) ? 'updated' : 'created';
    this.applications.set(payload.ssn, payload);
    return { status, application: payload };
  }

  public getBySsn(ssn: string): ApplicationPayload | undefined {
    return this.applications.get(ssn);
  }

  public getAll(): ApplicationPayload[] {
    return Array.from(this.applications.values());
  }
}

export const memoryStore = new MemoryStore();
import { Router, type Request, type Response } from 'express';
import { applicationPayloadSchema } from '../schemas';
import { logger, redactSsn } from '../logger';
import { memoryStore } from '../store/memoryStore';

const router = Router();

router.post('/applications', (req: Request, res: Response) => {
  const requestBody: unknown = req.body;
  const validation = applicationPayloadSchema.safeParse(requestBody);

  if (!validation.success) {
    logger.warn({
      event: 'mock.application.rejected',
      method: req.method,
      path: req.originalUrl,
      payload: redactSsn(requestBody),
      issues: validation.error.issues,
    }, 'External application payload rejected');
    return res.status(400).json({ status: 'error', message: 'Invalid application payload' });
  }

  const result = memoryStore.upsert(validation.data);
  logger.info({
    event: `mock.application.${result.status}`,
    method: req.method,
    path: req.originalUrl,
    payload: redactSsn(result.application),
  }, 'External application processed');

  return res.status(200).json({
    id: result.application.applicationId,
    status: result.status,
  });
});

router.put('/applications/:ssn', (req: Request, res: Response) => {
  const pathSsn = req.params.ssn;
  const requestBody: unknown = req.body;
  const validation = applicationPayloadSchema.safeParse(requestBody);

  if (!validation.success) {
    logger.warn({
      event: 'mock.application.rejected',
      method: req.method,
      path: req.originalUrl,
      payload: redactSsn(requestBody),
      issues: validation.error.issues,
    }, 'External application payload rejected');
    return res.status(400).json({ status: 'error', message: 'Invalid application payload' });
  }

  if (validation.data.ssn !== pathSsn) {
    logger.warn({
      event: 'mock.application.rejected',
      method: req.method,
      path: req.originalUrl,
      payload: redactSsn(requestBody),
    }, 'External application SSN did not match the route');
    return res.status(400).json({ status: 'error', message: 'Route SSN must match payload SSN' });
  }

  const result = memoryStore.upsert(validation.data);
  logger.info({
    event: `mock.application.${result.status}`,
    method: req.method,
    path: req.originalUrl,
    payload: redactSsn(result.application),
  }, 'External application processed');

  return res.status(200).json({
    id: result.application.applicationId,
    status: result.status,
  });
});


router.get('/applications', (_req: Request, res: Response) => {
  return res.status(200).json(memoryStore.getAll());
});

export default router;
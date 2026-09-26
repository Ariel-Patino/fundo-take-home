import express from 'express';
import externalApplicationsRouter from './routes/externalApplications';
import { logger } from './logger';

const app = express();
const PORT = process.env.PORT || 5001;

app.use(express.json());

app.use('/external', externalApplicationsRouter);

app.get('/health', (_req, res) => {
  res.status(200).send('Mock External Service is running');
});

app.listen(PORT, () => {
  logger.info({ event: 'mock.service.started', port: PORT }, 'Mock external service listening');
});
# SKILL — Docker + PostgreSQL 16

## docker-compose.yml
```yaml
services:
  postgres:
    image: postgres:16-alpine
    container_name: fundo-postgres
    environment:
      POSTGRES_USER: ${POSTGRES_USER:?Set POSTGRES_USER in .env}
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD:?Set POSTGRES_PASSWORD in .env}
      POSTGRES_DB: ${POSTGRES_DB:?Set POSTGRES_DB in .env}
    ports:
      - "5432:5432"
    volumes:
      - fundo_pgdata:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U $${POSTGRES_USER} -d $${POSTGRES_DB}"]
      interval: 5s
      timeout: 5s
      retries: 10
    command: >
      postgres
      -c shared_buffers=256MB
      -c max_connections=100
      -c log_statement=ddl
      -c timezone=UTC

  mock-service:
    build: ./mock-service
    container_name: fundo-mock
    ports:
      - "4000:4000"
    environment:
      PORT: 4000
      NODE_ENV: production

volumes:
  fundo_pgdata:
```
### Best Practices Applied
- Alpine image.
- Named volume for persistence.
- Healthcheck so backend can depends_on: condition: service_healthy.
- UTC timezone.
- log_statement=ddl for dev visibility, not all (noise).
- Explicit shared_buffers for local dev predictability.

### Connection String
Load the connection string from .NET User Secrets for local development or from the deployment environment's secret manager. Do not store credentials in tracked settings files.

```cmd
cd backend/src/Fundo.Infrastructure
dotnet ef migrations add InitialCreate -s ../Fundo.WebApi
dotnet ef database update -s ../Fundo.WebApi
```

Checklist
- [ ] Healthcheck present
- [ ] Named volume
- [ ] No latest tags
- [ ] Mock service built from Dockerfile
# Architecture

## Structure and dependency direction

The solution separates the backend into four projects under `src/backend`:

- `Fundo.LoanEngine.Domain`: `Customer`, `Application`, the `Ssn` value object, and rule input/result primitives. It has no external package references and owns core invariants such as valid nine-digit SSNs, two-character state codes, and positive requested amounts.
- `Fundo.LoanEngine.Application`: submit-application command, validator, handler, rule engine, and ports such as `IApplicationRepository`, `IExternalMockClient`, and `IClock`. It references only Domain.
- `Fundo.LoanEngine.Infrastructure`: EF Core/Npgsql persistence, migrations, HTTP client, `SystemClock`, and the Outbox worker. It implements Application ports and references Application and Domain.
- `Fundo.LoanEngine.WebApi`: Minimal API contracts/endpoints and dependency-injection composition. It references Application and Infrastructure; endpoints do not access `DbContext`.

The frontend is in `src/frontend` (Next.js App Router, React Hook Form, Zod, and Tailwind). The external mock is in `src/mock-service` (Node 24, Express, Zod, Pino, and an in-memory Map). `docker-compose.yml` currently runs PostgreSQL; the API, frontend, and mock run as local processes.

## Rule engine

`RuleEngine` evaluates the registered `IDenyRule` implementations in order and returns the first denial, otherwise approval. `StateDenyRule` rejects configured states (NY by default). `SsnBlacklistRule` canonicalizes SSNs before comparison with configured blocked values. To add a rule, implement `IDenyRule` in a new Application rules file and register it in `Fundo.LoanEngine.WebApi/DependencyInjection.cs`; the engine itself does not change.

The submit handler validates and canonicalizes the SSN before constructing the rule input or querying for a returning customer. The database stores canonical nine-digit SSNs; outbound mock events format them as `###-##-####` to match the mock-service contract.

## Submission and transaction boundary

`POST /api/applications` maps the request to `SubmitApplicationCommand`, invokes `SubmitApplicationHandler`, and maps the result to `200 Approved`, `422 Denied`, or `400 Invalid`.

For an approved submission, the handler tracks Customer/Application inserts or updates and calls `ApplicationRepository.CommitTransactionWithEventAsync`. The repository opens one explicit EF/PostgreSQL transaction, adds an `ApplicationSubmittedEvent` payload to `outbox_messages`, calls `SaveChangesAsync` once, and commits. Therefore, the business rows and Outbox row are atomic inside PostgreSQL: if any statement or commit fails, the whole transaction rolls back. A deferred PostgreSQL constraint-trigger integration test deliberately fails at commit and checks that all three tables remain empty.

This is not a distributed transaction with the HTTP service. After the database commit, `OutboxBackgroundService` polls pending records, claims rows using `FOR UPDATE SKIP LOCKED`, and sends HTTP outside the database transaction. On success it marks the message processed. Transient/network and server failures use exponential backoff up to ten attempts; 4xx responses and exhausted retries are marked failed for inspection/recovery. A crash after a remote success but before the local processed mark can cause redelivery, so the mock performs an SSN-keyed upsert. The design guarantees durable, at-least-once delivery attempts, not exactly-once delivery or atomic commit across PostgreSQL and HTTP.

## Design choices and trade-offs

- PostgreSQL is the durable store; the Outbox replaces a broker for this take-home and keeps business writes and event intent in one ACID transaction. A worker provides asynchronous HTTP delivery without blocking the API request.
- The mock intentionally stores data in memory and loses it on restart. This keeps the external dependency demonstrative, while PostgreSQL remains durable.
- The app uses one application record per customer (one-to-one) and updates it for repeat submissions. Multiple historical applications are out of scope.
- The external event is posted for a new customer and put for a returning customer. SSN is the mock idempotency key. Logs mask the SSN; the event remains in the database Outbox to support delivery.
- Authentication, a message broker, durable mock storage, multiple application history, and a production-grade dead-letter/replay console are out of scope. Failed Outbox rows require operational inspection/replay rather than a separate dashboard.
- SSN is canonicalized and validated, but database encryption-at-rest and broader regulated-data controls are deployment responsibilities and are not implemented in this sample.

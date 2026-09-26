# Fundo Loan Application

**Video demo:** https://www.loom.com/share/87bf620fc5a44ad394ded18b85718931

A small loan application flow with a Next.js frontend, .NET 10 API, PostgreSQL persistence, and an Express mock external service.

## Prerequisites

- Windows PowerShell (commands below use PowerShell).
- Node.js 24.21.0 and npm.
- .NET SDK 10.0.301.
- Docker Desktop running with the Linux container engine enabled.

## Run locally

Run each service in a separate terminal from the repository root.

### 1. Configure and start PostgreSQL

Copy the environment template and fill in private local values. Do not commit `.env`.

```powershell
Copy-Item .env.example .env
code .env
```

Set `POSTGRES_DB`, `POSTGRES_USER`, `POSTGRES_PASSWORD`, and `POSTGRES_PORT` to values of your choice. Then start PostgreSQL:

```powershell
docker compose up -d postgres
docker compose ps
```

Wait for the PostgreSQL service health status to become `healthy`. The compose file starts PostgreSQL only; the app processes run locally in the following steps. The named `postgres_data` volume preserves the database between restarts.

### 2. Configure .NET User Secrets

ASP.NET Core does not read Compose's `.env` file. The following PowerShell command reads the local values and stores the connection string in .NET User Secrets for the Web API project:

```powershell
$postgres = @{}
Get-Content .env | ForEach-Object {
	if ($_ -match '^\s*([^#][^=]*)=(.*)$') {
		$postgres[$matches[1].Trim()] = $matches[2].Trim()
	}
}
$connectionString = "Host=localhost;Port=$($postgres['POSTGRES_PORT']);Database=$($postgres['POSTGRES_DB']);Username=$($postgres['POSTGRES_USER']);Password=$($postgres['POSTGRES_PASSWORD'])"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" $connectionString --project src/backend/Fundo.LoanEngine.WebApi
```

The API applies the committed EF Core migrations at startup. No database password is stored in tracked application settings.

### 3. Start the mock external service

```powershell
npm ci --prefix src/mock-service
npm --prefix src/mock-service run typecheck
npm --prefix src/mock-service run start
```

The mock listens on `http://localhost:5001` by default. It keeps received records in memory, so its data resets when the process restarts. Structured request logs mask the SSN except for its final four digits.

### 4. Start the Web API

```powershell
$env:ASPNETCORE_ENVIRONMENT = 'Development'
$env:ASPNETCORE_URLS = 'http://localhost:5000'
dotnet run --project src/backend/Fundo.LoanEngine.WebApi
```

The API is available at `http://localhost:5000`. In Development, the OpenAPI document is available at `http://localhost:5000/openapi/v1.json`.

### 5. Start the frontend

```powershell
npm ci --prefix src/frontend
$env:BACKEND_API_URL = 'http://localhost:5000'
npm --prefix src/frontend run dev
```

Open `http://localhost:3000`. `BACKEND_API_URL` is read by the Next.js Server Action and does not need the `NEXT_PUBLIC_` prefix.

### Stop services

Stop each local process with Ctrl+C, then stop PostgreSQL:

```powershell
docker compose down
```

The database volume is retained. To permanently delete local database data, run `docker compose down -v`.

## Tests

Run backend unit tests and the PostgreSQL/WebAPI integration tests from the repository root:

```powershell
dotnet test src/backend/Fundo.LoanEngine.Tests/Fundo.LoanEngine.Tests.csproj
dotnet test src/backend/Fundo.LoanEngine.IntegrationTests/Fundo.LoanEngine.IntegrationTests.csproj
```

The integration project starts a disposable PostgreSQL container through Testcontainers; Docker must be running. It includes a deferred PostgreSQL constraint failure test that verifies `Customer`, `Application`, and Outbox rows all roll back, plus WebAPI approval, NY denial, and invalid-SSN responses.

Run frontend checks:

```powershell
npm --prefix src/frontend run typecheck
npm --prefix src/frontend test -- --runInBand
npm --prefix src/frontend run build
```

The mock service intentionally has no test suite. Its TypeScript check is `npm --prefix src/mock-service run typecheck`.

## Demo test data

All examples below use a positive requested amount and otherwise valid required fields. Use any non-empty first name, last name, address, and company name.

| Flow | State | SSN | Expected result |
| --- | --- | --- | --- |
| Approved, new customer | CA | `555-55-0101` | Approved; creates one customer/application. |
| Denied by state | NY | `555-55-0102` | Denied by the state rule. |
| Denied by SSN blacklist | CA | `000-00-0000` | Denied by the SSN rule. |
| Also blacklisted | CA | `999-99-9999` or `123-45-6789` | Denied by the SSN rule. |
| Returning customer | CA | `555-55-0101` again | Updates the prior customer/application; change the name or amount to see the update. |

The returning-customer demonstration requires the same PostgreSQL volume to remain in place between submissions. The mock service's in-memory record is updated by SSN while the process remains running.

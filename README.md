# 1st-semester-projekt

Blazor WebAssembly frontend + ASP.NET Core Web API backend, with SQL Server (EF Core) for relational data and MongoDB for attachment/image storage.

## Projects

| Path | Purpose |
| --- | --- |
| `backend/api` | ASP.NET Core Web API (controllers, EF Core, JWT auth) |
| `blazor/blazor` | Blazor WebAssembly client |
| `Shared` | Cross-project DTOs (login, role constants) |
| `Tests` | xUnit + Moq + FluentAssertions test project |

## Prerequisites

- Docker (required — runs SQL Server, MongoDB, the API, and the Blazor client)
- .NET 10 SDK (only needed if you want to run the API/Blazor outside Docker via `./run.sh`, or to run `dotnet test`)

## Setup (Docker — recommended)

`docker compose up -d` brings up the full stack: SQL Server, MongoDB, the API, and the Blazor client.

1. **Copy environment file**
   ```sh
   cp .env.example .env
   ```
   Edit `.env` and set the passwords.

2. **Bring up the full stack**
   ```sh
   docker compose up -d
   ```
   Services:
   - SQL Server: `localhost:1433`
   - MongoDB: `localhost:27017`
   - API: <http://localhost:5000> (OpenAPI: <http://localhost:5000/openapi/v1.json>)
   - Blazor client: <http://localhost:5050>

   The Blazor client (`blazor/blazor/wwwroot/appsettings.json` → `ApiBaseUrl`) is configured to call the API at `http://localhost:5000`. The Docker setup only exposes the API over HTTP on port 5000 — there is no HTTPS endpoint inside Docker.

3. **Apply EF migrations** (only needed the first time, or after a schema change)
   ```sh
   dotnet ef database update --project backend/api
   ```

## Setup (local dev without Docker for the apps)

If you want hot-reload via `dotnet watch` instead of the `api` / `blazor` containers, bring up just the databases and run the apps locally:

```sh
docker compose up -d sqlserver mongodb
dotnet ef database update --project backend/api
./run.sh     # macOS / Linux
./run.ps1    # Windows PowerShell
```

- API: <http://localhost:5000> / <https://localhost:5001> (OpenAPI: <http://localhost:5000/openapi/v1.json>)
- Blazor: <http://localhost:5050> / <https://localhost:5051>

`ApiBaseUrl` is `http://localhost:5000`, which both `dotnet watch` and Docker bind, so the same client config works in either mode.

## Seeded accounts

`DbSeeder` (opt-in; uncomment the seeding block in `backend/api/Program.cs` to enable) creates:

| Role | Email | Password |
| --- | --- | --- |
| Admin | `admin@admin.com` | `Admin1234!` |
| Teacher | `teacher@teacher.com` | `Teacher1234!` |
| Student | `student@student.com` | `Student1234!` |

## Tests

```sh
dotnet test Tests/Tests.csproj
```

Uses xUnit, Moq, and FluentAssertions. EF integration tests use the in-memory provider.

## Teardown

```sh
docker compose down        # stop containers
docker compose down -v     # also delete database volumes
```

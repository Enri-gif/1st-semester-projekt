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

- .NET 10 SDK
- Docker (for SQL Server + MongoDB)

## Setup

1. **Copy environment file**
   ```sh
   cp .env.example .env
   ```
   Edit `.env` and set the passwords.

2. **Start databases**
   ```sh
   docker compose up -d
   ```
   This brings up SQL Server on `localhost:1433` and MongoDB on `localhost:27017`.

3. **Apply EF migrations**
   ```sh
   dotnet ef database update --project backend/api
   ```

4. **Run backend + Blazor together** (two `dotnet watch` processes)
   ```sh
   ./run.sh     # macOS / Linux
   ./run.ps1    # Windows PowerShell
   ```
   - API: <http://localhost:5000> / <https://localhost:5001> (OpenAPI: <http://localhost:5000/openapi/v1.json>)
   - Blazor: <http://localhost:5050> / <https://localhost:5051>

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

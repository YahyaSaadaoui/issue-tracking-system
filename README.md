# IssueDesk

IssueDesk is a compact issue-tracking system built to demonstrate a Clean Architecture .NET API with a modern React frontend. It covers project setup, ticket lifecycle management, assignment, status changes, comments, filtering, pagination, validation, and seeded local data.

## Highlights

- ASP.NET Core 9 API organized with Domain, Application, Infrastructure, and WebApi layers.
- CQRS-style commands and queries with MediatR.
- EF Core 9 with SQL Server, migrations, repositories, and unit of work.
- FluentValidation and Problem Details for consistent API validation and errors.
- React 19, Vite, TypeScript, React Query, Zod, Tailwind CSS, shadcn-style components, and lucide-react icons.
- Docker Compose setup for SQL Server, API, and frontend.
- xUnit unit and integration test projects.

## Repository Layout

```text
.
├── docker-compose.yml
├── IssueDesk/
│   ├── IssueDesk.sln
│   ├── frontend/              # React + Vite + TypeScript app
│   ├── src/
│   │   ├── Domain/            # Entities, value objects, enums, domain events
│   │   ├── Application/       # Commands, queries, handlers, validators, abstractions
│   │   ├── Infrastructure/    # EF Core DbContext, repositories, migrations, unit of work
│   │   └── WebApi/            # Minimal API endpoints, DI, Swagger, Problem Details
│   └── tests/
│       ├── UnitTests/
│       └── IntegrationTests/
└── README.md
```

## Quick Start With Docker

Prerequisites:

- Docker with Compose.
- Ports `1433`, `5080`, and `5173` available locally.

Run the full stack from the repository root:

```bash
docker compose up -d --build
```

Open the services:

| Service | URL |
| --- | --- |
| Frontend | http://localhost:5173 |
| Swagger UI | http://localhost:5080/swagger |
| Health check | http://localhost:5080/health |

The API applies EF Core migrations and seeds the database during startup.

Stop the stack:

```bash
docker compose down
```

Remove the SQL Server volume/data if you need a clean local reset:

```bash
docker compose down -v
```

## Local Development

Start SQL Server in Docker:

```bash
docker run --name issuedesk-sql \
  -e ACCEPT_EULA=Y \
  -e SA_PASSWORD=Your_password123 \
  -p 1433:1433 \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

Apply database migrations:

```bash
cd IssueDesk
dotnet ef database update -p src/Infrastructure -s src/WebApi
```

Run the API:

```bash
dotnet run --project IssueDesk/src/WebApi
```

Run the frontend:

```bash
cd IssueDesk/frontend
npm ci
npm run dev
```

The Vite app proxies `/api` requests to the API. In Docker, `VITE_PROXY_TARGET` is set to `http://api:8080`; locally it can point to `http://localhost:5080`.

## API Overview

Development base URL:

```text
http://localhost:5080/api
```

| Method | Endpoint | Description |
| --- | --- | --- |
| `GET` | `/projects` | List projects. |
| `POST` | `/projects` | Create a project. |
| `POST` | `/tickets` | Create a ticket. |
| `GET` | `/tickets/{id}` | Get ticket details and comments. |
| `GET` | `/projects/{projectId}/tickets` | List project tickets with filters and pagination. |
| `POST` | `/tickets/{id}/assign` | Assign a ticket. |
| `POST` | `/tickets/{id}/status` | Change ticket status. |
| `POST` | `/tickets/{id}/comments` | Add a ticket comment. |

Create a project:

```bash
curl -X POST http://localhost:5080/api/projects \
  -H "Content-Type: application/json" \
  -d '{"name":"Payments","key":"PAY"}'
```

Create a ticket:

```bash
curl -X POST http://localhost:5080/api/tickets \
  -H "Content-Type: application/json" \
  -d '{
    "projectId":"<PROJECT_GUID>",
    "title":"Customer cannot pay",
    "description":"Fails during 3DS confirmation",
    "priority":"High"
  }'
```

Filter tickets:

```bash
curl "http://localhost:5080/api/projects/<PROJECT_GUID>/tickets?status=InProgress&priority=High&page=1&pageSize=10"
```

## Common Commands

```bash
# Build the solution
dotnet build IssueDesk/IssueDesk.sln

# Run backend tests
dotnet test IssueDesk/IssueDesk.sln

# Run the API
dotnet run --project IssueDesk/src/WebApi

# Create a migration
dotnet ef migrations add <Name> -p IssueDesk/src/Infrastructure -s IssueDesk/src/WebApi -o Persistence/Migrations

# Apply migrations
dotnet ef database update -p IssueDesk/src/Infrastructure -s IssueDesk/src/WebApi

# Frontend checks
cd IssueDesk/frontend
npm ci
npm run lint
npm run build
```

## Configuration

The backend reads `ConnectionStrings:DefaultConnection`. Docker Compose injects it with SQL Server service DNS:

```text
Server=sqlserver,1433;Database=IssueDeskDb;User Id=sa;Password=Your_password123;TrustServerCertificate=True;Encrypt=False;
```

For local development outside Docker, configure the connection string in `IssueDesk/src/WebApi/appsettings.Development.json`, user secrets, or environment variables.

## Troubleshooting

API container is unhealthy:

- Check SQL Server readiness with `docker compose ps`.
- Review API logs with `docker compose logs api`.
- If migrations changed, rebuild with `docker compose up -d --build`.

Frontend cannot reach the API:

- Confirm the API health endpoint works: `curl http://localhost:5080/health`.
- In Docker, keep `VITE_PROXY_TARGET=http://api:8080`.
- Locally, point the frontend proxy to `http://localhost:5080`.

SQL Server startup is slow:

- The first boot can take 10-30 seconds.
- Wait for the `sqlserver` service to become healthy before testing the API.

## Good First Improvements

- Add screenshots or a GIF of the ticket workflow.
- Add CI for backend tests and frontend lint/build.
- Add seed data documentation with sample project IDs.
- Expand integration tests for ticket status transitions.
- Add environment examples for local frontend/API configuration.

## License

MIT License. See [LICENSE](./LICENSE).

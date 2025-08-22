# IssueDesk: A Clean Architecture-Based Issue Tracker

**Tech Stack:** .NET 9, EF Core, MediatR, and React (Vite + TS)

IssueDesk is a compact yet practical issue and help tracking application. It serves as a demonstration of **Clean Architecture**, **Domain-Driven Design (DDD) + Command Query Responsibility Segregation (CQRS)**, and a contemporary **React** frontend.

> **Technology Stack Details**
>
> * **Backend:** ASP.NET Core 9, EF Core 9 (utilizing SQL Server), MediatR, FluentValidation, Serilog, Swagger
> * **Frontend:** React 18, Vite, TypeScript, React Query, Axios, Zod, Tailwind (shadcn/ui)
> * **DevOps:** Docker Compose (for API, DB, and Web), xUnit for testing

## Key Features

* Organize tickets by projects, each with a unique name and key.
* Complete ticket lifecycle management: **New → InProgress → Resolved → Closed**.
* Functionality to assign tickets, update their status, and add comments.
* Implementation of **CQRS** handlers, enhanced with **FluentValidation** and **domain events**.
* Uses **EF Core code-first migrations** for database management.
* **Swagger/OpenAPI** integration, featuring Problem Details for error handling.
* A **React**-based user interface with filtering, pagination, dialogs, and states for loading/error.

## Getting Started

### Prerequisites

* Docker (with Compose)
* Node.js version 20 or higher
* .NET SDK 9.0 or newer (required only for running the API outside of Docker)

### 1) Running with Docker

Execute the following command from the root of the repository:

```bash
docker compose up -d --build
```


**Accessible URLs**

* **Frontend:** [http://localhost:5173](https://www.google.com/url?sa=E&q=http%3A%2F%2Flocalhost%3A5173)
* **API (Swagger):** [http://localhost:5080/swagger](https://www.google.com/url?sa=E&q=http%3A%2F%2Flocalhost%3A5080%2Fswagger)
* **Health Check:** [http://localhost:5080/health](https://www.google.com/url?sa=E&q=http%3A%2F%2Flocalhost%3A5080%2Fhealth) **→** **{"status":"ok"}**

**Services and Ports**

* **sqlserver** **on port 1433**
* **api** **(ASP.NET) on port 5080 (internally runs on 8080)**
* **web** **(Vite dev server) on port 5173**

> **The API automatically applies EF Core migrations and seeds the database with initial data upon startup.**

---

### 2) Local Development (API & Web running outside Docker)

**First, run SQL Server in a Docker container:**

```
docker run --name issuedesk-sql \
  -e ACCEPT_EULA=Y -e SA_PASSWORD=Your_password123 \
  -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
```

**Next, apply the database migrations:**

```
cd IssueDesk
dotnet ef database update -p src/Infrastructure -s src/WebApi
```

**To run the API:**

```
dotnet run --project IssueDesk/src/WebApi
# API will be available at http://localhost:5080/swagger
```

**To run the Frontend:**

```
cd IssueDesk/frontend
npm ci
npm run dev
# Frontend will be available at http://localhost:5173
```

**The development server is configured to proxy** **/api** **requests to** **http://localhost:5080**. This can be overridden:

```
# In IssueDesk/frontend/.env.development (this is optional)
VITE_PROXY_TARGET=http://localhost:5080
```

---

## API Overview

**The base URL in the development environment is** **http://localhost:5080/api**


| **Method** | **Endpoint**                          | **Description**                                       |
| ---------- | ------------------------------------- | ----------------------------------------------------- |
| **GET**    | **/projects**                         | **Retrieves a list of projects.**                     |
| **POST**   | **/projects**                         | **Creates a new project.**                            |
| **POST**   | **/tickets**                          | **Creates a new ticket.**                             |
| **GET**    | **/tickets/{id}**                     | **Retrieves a ticket by its ID.**                     |
| **GET**    | **/projects/{projectId}/tickets?...** | **Paged and filtered list of tickets for a project.** |
| **POST**   | **/tickets/{id}/assign**              | **Assigns a ticket to a user.**                       |
| **POST**   | **/tickets/{id}/status**              | **Changes the status of a ticket.**                   |
| **POST**   | **/tickets/{id}/comments**            | **Adds a comment to a ticket.**                       |

### Sample cURL Commands

**Create a new project:**

```
curl -X POST http://localhost:5080/api/projects \
  -H "Content-Type: application/json" \
  -d '{"name":"Payments","key":"PAY"}'
```

**Create a new ticket:**

```
curl -X POST http://localhost:5080/api/tickets \
  -H "Content-Type: application/json" \
  -d '{
    "projectId":"<PROJECT_GUID>",
    "title":"Customer cannot pay",
    "description":"Fails on 3DS",
    "priority":"High"
  }'
```

**Filter tickets:**

```
curl "http://localhost:5080/api/projects/<PROJECT_GUID>/tickets?status=InProgress&priority=High&page=1&pageSize=10"
```

---

## Architectural Design

```
src/
  Domain/          // Contains Entities, Value Objects, Enums, and Domain Events (free of EF attributes)
  Application/     // CQRS elements (Commands/Queries + Handlers), Validators, and Abstractions
  Infrastructure/  // EF Core DbContext, Repositories, Migrations, and Unit of Work
  WebApi/          // ASP.NET Core endpoints, Dependency Injection, Serilog, Swagger, and ProblemDetails

frontend/          // React + Vite + TypeScript application (features React Query, Zod, shadcn/ui)
tests/
  UnitTests/       // Domain and Application level tests (using xUnit)
  IntegrationTests // (light demonstration)
```

### Core Architectural Decisions

* **CQRS & MediatR** **are used for separating commands and queries, and for domain notifications.**
* **FluentValidation** **is integrated with a pipeline behavior for automatic validation.**
* **EF Core 9** **with** **code-first migrations** **for SQL Server.**
* **Problem Details (RFC7807)** **standard for consistent API error responses.**
* **Serilog** **for structured application logging.**

---

## Frontend Details

* **Built with Vite, React, and TypeScript.**
* **React Query for efficient data fetching and caching.**
* **Axios client with a base URL set to** **/api**.
* **Styling with Tailwind and shadcn/ui components.**
* **Zod schemas that mirror the API contracts.**

**Development Commands:**

```
cd IssueDesk/frontend
npm ci
npm run dev        # Starts the Vite development server
npm run build      # Creates a production build
```

---

## Testing

**To run all tests, execute:**

```
dotnet test
```

* **UnitTests:** **Focus on domain invariants, and application handlers/validators.**
* **IntegrationTests (Optional):** **Includes minimal checks for endpoints.**

---

## Configuration Settings

### Connection String

**The API uses** **ConnectionStrings:DefaultConnection**. In a Docker environment, it's injected through **docker-compose.yml**:

```
Server=sqlserver,1433;Database=IssueDeskDb;User Id=sa;Password=Your_password123;TrustServerCertificate=True;Encrypt=False;
```

**For local execution (outside of Docker), this value can be placed in** **IssueDesk/src/WebApi/appsettings.Development.json**, or you can rely on the Infrastructure project's fallback to **localhost,1433**.

### Health & Logging

* **Health Check:** **GET /health** **→** **{ "status": "ok" }**
* **Serilog** **logs to the console; Docker logs will display startup information and EF migration details.**

## Troubleshooting Guide

**API container is "unhealthy" or exits with a** **PendingModelChangesWarning**

* **You may need to add and apply migrations:**

```
cd IssueDesk
dotnet ef migrations add Sync_$(date +%Y%m%d_%H%M) -p src/Infrastructure -s src/WebApi -o Persistence/Migrations
dotnet ef database update -p src/Infrastructure -s src/WebApi
docker compose up -d --build
```

**Frontend is unable to reach the API (**ENOTFOUND api **or 404 error)**

* **Confirm that the API is running and healthy:** **curl http://localhost:5080/health**
* **The Vite proxy target is set via** **VITE\_PROXY\_TARGET** **in Compose. For local development, it defaults to** **http://localhost:5080**.

**SQL Server is not ready**

* **The Docker Compose file includes a health check; the initial boot might take 10–20 seconds. Please try again once it's healthy:**
  docker inspect --format='{{.State.Health.Status}}' issue-tracking-system-sqlserver-1

## Repository Scripts & Useful Commands

```
# Build the entire solution
dotnet build IssueDesk/IssueDesk.sln

# Run the API project only
dotnet run --project IssueDesk/src/WebApi

# Create a new database migration
dotnet ef migrations add <Name> -p IssueDesk/src/Infrastructure -s IssueDesk/src/WebApi -o Persistence/Migrations

# Apply existing database migrations
dotnet ef database update -p IssueDesk/src/Infrastructure -s IssueDesk/src/WebApi
```

## License

**This project is licensed under the MIT License — see the** [LICENSE](https://www.google.com/url?sa=E&q=.%2FLICENSE) **file for details.**

## Additional Notes

**This repository contains my implementation of a small-scale issue tracker, designed to demonstrate Clean Architecture, CQRS, and a modern React UI. While the application's scope is intentionally limited, it is built with production standards in mind, incorporating validations, events, migrations, Problem Details, and a component-based, accessible frontend.**

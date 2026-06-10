# Contributing

Thanks for improving IssueDesk. Keep changes focused, easy to review, and aligned with the Clean Architecture boundaries already in the project.

## Local Setup

Run the full stack with Docker:

```bash
docker compose up -d --build
```

Or run the backend and frontend separately:

```bash
cd IssueDesk
dotnet restore
dotnet build IssueDesk.sln
```

```bash
cd IssueDesk/frontend
npm ci
npm run dev
```

## Backend Guidelines

- Keep domain rules in `IssueDesk/src/Domain`.
- Keep commands, queries, handlers, validators, and application abstractions in `IssueDesk/src/Application`.
- Keep EF Core, repositories, migrations, and persistence details in `IssueDesk/src/Infrastructure`.
- Keep HTTP endpoint wiring and API contracts in `IssueDesk/src/WebApi`.
- Add tests for domain rules, validators, and handler behavior when changing business logic.

## Frontend Guidelines

- Keep API access and schemas typed.
- Use React Query for server state.
- Keep route-level pages in `IssueDesk/frontend/src/pages` and reusable UI in components.
- Run lint and build before opening a PR.

## Checks

Use the checks that match your change:

```bash
dotnet build IssueDesk/IssueDesk.sln
dotnet test IssueDesk/IssueDesk.sln
```

```bash
cd IssueDesk/frontend
npm ci
npm run lint
npm run build
```

For Docker-related changes, also run:

```bash
docker compose up -d --build
docker compose ps
docker compose logs api
docker compose down
```

## Pull Request Checklist

- The change has one clear purpose.
- README or docs are updated when setup, commands, endpoints, or configuration change.
- New behavior is covered by tests where practical.
- No generated build artifacts, local secrets, database files, or logs are committed.

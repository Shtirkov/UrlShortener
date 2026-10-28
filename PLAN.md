# Plan

Stack: ASP.NET Core 10 (controllers), EF Core 10 + Npgsql, Redis (IDistributedCache), Postgres 18, Next.js 16 (App Router, Tailwind 4), Docker Compose.

Flow: the UI posts the long URL through a Next.js server action to `POST /api/urls`. The API stores it and returns `{ code, shortUrl, longUrl }`. `GET /{code}` on the API redirects (302) to the long URL, reading through Redis before Postgres.

## Phase 1 - Scaffolding
- [x] git repo initialised, root `.gitignore` covering .NET, Node, IDE and env files
- [x] `api/` solution with clean architecture projects: Domain, Application, Infrastructure, Api
- [x] `api/tests/` with UnitTests and IntegrationTests projects
- [x] `frontend/` created with Next.js 16, TypeScript, Tailwind, App Router
- [x] Both sides build cleanly (`dotnet build`, `npm run build`)

## Phase 2 - Backend
- [x] Domain: `ShortUrl` entity (Id, Code, LongUrl, CreatedAt)
- [x] Application: `UrlShortenerService` with `ShortenAsync` (validates absolute http/https URL, generates 7-char base62 code) and `ResolveAsync` (cache-aside via `IDistributedCache`)
- [x] Infrastructure: EF Core `AppDbContext`, unique index on `Code`, repository, Redis cache registration, initial migration
- [x] Api: `UrlsController` with `POST /api/urls` (201 / 400), `GET /{code}` (302 / 404), migrations applied on startup
- [x] Unit tests cover validation, code generation, cache hit, cache miss + populate, not found

## Phase 3 - Frontend
- [x] Dark, minimal single page: URL input, submit button, result with copy button, error message
- [x] Server action calls the API using runtime `API_URL`
- [x] Unit tests (Vitest + Testing Library) for the form and the action
- [x] Lint and build pass

## Phase 4 - Containerisation
- [x] `api/Dockerfile` (multi-stage, sdk -> aspnet)
- [x] `frontend/Dockerfile` (multi-stage, standalone output)
- [x] `docker-compose.yml` with postgres, redis (healthchecks), api, frontend
- [x] `docker compose up --build` starts everything; UI on :3001, API on :8080

## Phase 5 - Integration testing
- [x] API integration tests with WebApplicationFactory + Testcontainers (Postgres, Redis): create, redirect, 404, 400, cache populated, index present
- [x] End-to-end check against the running compose stack (UI -> API -> DB -> redirect)
- [x] All defects found are fixed and all test suites pass

## Phase 6 - Delivery
- [x] Minimal README (run, test)
- [x] Stack running via Docker Compose and ready for use

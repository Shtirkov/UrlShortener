# URL Shortener

ASP.NET Core 10 API (EF Core, Postgres, Redis cache) and a Next.js 16 frontend.

## Run

```sh
docker compose up --build -d
```

- UI: http://localhost:3001
- API: http://localhost:8080 (`POST /api/urls`, `GET /{code}`)

## Test

```sh
cd api && dotnet test        # unit + integration (Testcontainers, needs Docker)
cd frontend && npm test      # Vitest
```

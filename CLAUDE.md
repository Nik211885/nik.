# nik.app — Project Overview

Portfolio/blog website. Angular 21 frontend + ASP.NET Core 10 backend + PostgreSQL.

## Repository Layout

```
nik.app/
├── backend/          # ASP.NET Core 10 Web API
├── font-end/         # Angular 21 SPA (folder name is intentional typo, do not rename)
├── .github/          # CI workflows
└── .vscode/          # Shared editor config
```

## Tech Stack

| Layer      | Technology                                      |
|------------|-------------------------------------------------|
| Backend    | .NET 10, ASP.NET Core, EF Core 10, PostgreSQL   |
| Validation | FluentValidation 12                             |
| Auth       | JWT Bearer (Microsoft.AspNetCore.Authentication)|
| Frontend   | Angular 21, TypeScript 5.9, RxJS 7.8            |
| Styling    | Bootstrap 5.3, Bootstrap Icons                  |
| Storage    | Cloudinary (file uploads)                       |
| Deploy     | Cloudflare Workers (frontend via Wrangler)      |

## Sub-project Docs

- [backend/CLAUDE.md](backend/CLAUDE.md) — C# conventions, patterns, architecture
- [font-end/CLAUDE.md](font-end/CLAUDE.md) — Angular conventions, patterns, architecture

## Running the Project

```bash
# Backend (from /backend)
dotnet run

# Frontend (from /font-end)
npm start
```

Backend auto-migrates the database on startup via `dbContext.Database.MigrateAsync()`.
Swagger UI is served at the root `/` in development mode.

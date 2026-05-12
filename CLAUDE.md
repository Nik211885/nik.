# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

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

## Commands

### Backend (run from `/backend`)

```bash
dotnet run                                    # Start dev server (auto-migrates DB)
dotnet build                                  # Compile only
dotnet ef migrations add {Name}               # Add a new EF Core migration
dotnet ef migrations remove                   # Remove the last migration
dotnet ef database update                     # Apply migrations without running the app
```

### Frontend (run from `/font-end`)

```bash
npm start                                     # Dev server (ng serve)
npm run build                                 # Production build
npx wrangler deploy                           # Deploy to Cloudflare Workers
```

Swagger UI is served at root `/` in development mode. Backend auto-migrates on startup via `dbContext.Database.MigrateAsync()`.

## Cross-project Architecture

### API Contract

The frontend calls the backend using **relative paths** (`this.http.get('api/albums')`). The `authInterceptor` prepends `environment.baseApiUrl` (set in `font-end/src/environments/environment.ts`) to all non-Cloudinary requests. Backend routes follow `api/{resource}` (lowercase, plural); all mutation endpoints are named (`/create`, `/update`, `/delete`).

### Authentication Flow

1. Backend issues JWT access + refresh tokens on login.
2. Frontend stores both in `localStorage` under keys `access_token` / `refresh_token`.
3. `authInterceptor` attaches `Bearer` token to every request; on 401 it calls the refresh endpoint, queues in-flight requests, and retries them. If refresh fails, it logs the user out.
4. Use `X-Skip-Auth-Request` header to bypass the interceptor entirely (e.g., public endpoints), or `X-Skip-Auth-Bear-Token` to skip token attachment only.

### i18n Error Keys

Backend error messages are **translation keys**, not English text (e.g., `"exception.not_found"`). These keys originate in `backend/ApplicationMessage.cs` and are expected to map to entries in the frontend translation dictionaries loaded by `LanguageService`. When adding a new backend error message, add the corresponding key to the frontend translation files.

### Pagination

Both sides use the same shape. Backend produces `PaginationItem<T>` (`data`, `totalItems`, `pageNumber`, `pageSize`, `pageCount`); frontend consumes the same interface and feeds it to the shared `<app-pagination>` component.

### File Uploads

Cloudinary uploads go directly from the frontend using absolute URLs — they bypass `authInterceptor` automatically (URL contains `cloudinary.com`). The `Files` entity on the backend stores metadata only.

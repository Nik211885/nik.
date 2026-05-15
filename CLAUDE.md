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
dotnet run                                    # Start dev server on http://localhost:5055
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

Swagger UI is served at `http://localhost:5055` in development mode. Backend auto-migrates and runs seeders on startup via `dotnet run`.

### Tests

**There are no tests in this project** — no `.spec.ts` files in the frontend, no test projects in the backend.

## Cross-project Architecture

### API Contract

The frontend calls the backend using **relative paths** (`this.http.get('api/albums')`). The `authInterceptor` prepends `environment.baseApiUrl` (set to `http://localhost:5055` in `font-end/src/environments/environment.ts`) to all non-Cloudinary requests. Backend routes follow `api/{resource}` (lowercase, plural); all mutation endpoints are named (`/create`, `/update`, `/delete`).

### Authentication Flow

1. Backend issues JWT access + refresh tokens on login.
2. Frontend stores both in `localStorage` under keys `access_token` / `refresh_token`.
3. `authInterceptor` attaches `Bearer` token to every request; on 401 it calls the refresh endpoint, queues in-flight requests, and retries them. If refresh fails, it logs the user out.
4. Use `X-Skip-Auth-Request` header to bypass the interceptor entirely (e.g., public endpoints), or `X-Skip-Auth-Bear-Token` to skip token attachment only.

### Admin Panel

The frontend has a full admin panel at `/admin` (lazy-loaded, separate from the public site):

```
/admin/dashboard
/admin/articles
/admin/albums
/admin/categories
/admin/tags
/admin/comments
/admin/files
/admin/users
/admin/languages
/admin/translations
/admin/sys-config
```

Admin code lives in `font-end/src/app/admin/` with its own layout, feature components, services, and shared components (`admin-table`, `admin-confirm-modal`, `cloudinary-upload`). The `authGuard` on the admin route is currently **commented out** — `/admin` is unprotected in the current codebase.

### i18n Error Keys

Backend error messages are **translation keys**, not English text (e.g., `"exception.not_found"`). These keys originate in `backend/ApplicationMessage.cs` and are expected to map to entries in the frontend translation dictionaries loaded by `LanguageService`. When adding a new backend error message, add the corresponding key to the frontend translation files.

`TranslatedHandlingMiddleware` (in `Pipes/Middlewares/`) intercepts responses and translates error key strings into the current request language before they reach the client.

The `acceptLanguageInterceptor` (frontend) adds an `Accept-Language` header to every HTTP request so the backend knows which language to use when translating error messages.

### App Initialization

On startup the frontend runs two `provideAppInitializer` calls (in `app.config.ts`):
1. `ConfigService.readConfig()` — loads runtime config
2. `LanguageService.init()` — loads translations and restores saved language

### Startup Seeders

Every `dotnet run` runs two idempotent seeders after migrations:
- `LanguageSeeder` — seeds translation keys and their EN/VI values
- `AlbumSeeder` — seeds default album data

When adding new translation keys, add them to `LanguageSeeder.GetEntries()` so they are present on the next startup.

### Pagination

Both sides use the same shape. Backend produces `PaginationItem<T>` (`data`, `totalItems`, `pageNumber`, `pageSize`, `pageCount`); frontend consumes the same interface and feeds it to the shared `<app-pagination>` component.

### File Uploads

Cloudinary uploads go directly from the frontend using absolute URLs — they bypass `authInterceptor` automatically (URL contains `cloudinary.com`). The `Files` entity on the backend stores metadata only.

### Angular 21 Zoneless Change Detection

Angular 21 defaults to **zoneless mode** — `ApplicationRef.tick()` is a no-op, plain property assignments in HTTP subscribe callbacks do not update the view. Fixed globally via `font-end/src/app/core/interceptors/zone.interceptor.ts` which calls `queueMicrotask(() => appRef.components[0]?.changeDetectorRef.detectChanges())` after every HTTP response. See `font-end/CLAUDE.md` for full details and the list of failed approaches to avoid.

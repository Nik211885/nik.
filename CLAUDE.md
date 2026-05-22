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
npm start                                     # Dev server (ng serve) — http://localhost:4200
npm run build                                 # Production build
npm run deploy                                # Build + deploy to Cloudflare Workers
npm run preview                               # Build + local Wrangler preview
```

Swagger UI is served at `http://localhost:5055` in development mode. Backend auto-migrates and runs seeders on startup via `dotnet run`.

Backend also exposes `/health` (PostgreSQL health check) and `/metrics` (Prometheus) in all environments.

### Docker

```bash
docker compose up --build                     # Full stack (backend + frontend + postgres)
docker compose -f docker-compose.prod.yml up  # Production compose
```

### Build & push Docker images to Docker Hub

**Cả 2 Dockerfile đều bỏ build stage** — build local trước, Docker chỉ copy output vào image. Lý do: `npm ci` và `dotnet restore` bên trong Docker liên tục timeout do mạng WSL2 không ổn định.

```bash
# 1. Build Angular (run from /font-end)
npm run build -- --configuration production

# 2. Publish backend (run from /backend)
dotnet publish backend.csproj -c Release -o ./publish

# 3. Build frontend image (run from repo root)
docker build -t nik185/nik-frontend:latest ./font-end

# 4. Build backend image (run from repo root)
docker build -t nik185/nik-backend:latest ./backend

# 5. Push both
docker push nik185/nik-frontend:latest
docker push nik185/nik-backend:latest
```

> - `font-end/Dockerfile` — chỉ copy `dist/font-end/browser/browser` vào nginx (không chạy `npm ci`). Angular 21 output nằm ở `browser/browser/` do prerender build stage tạo ra thư mục lồng nhau.
> - `backend/Dockerfile` — chỉ copy thư mục `publish/` vào aspnet runtime (không chạy `dotnet restore`)

### ⚠️ Trước khi build — kiểm tra ổ đĩa

Docker Desktop lưu toàn bộ image/layer/build cache vào WSL2 virtual disk tại `C:\Users\ninhl\AppData\Local\Docker` — có thể lên tới **20GB+**. Nếu C: gần đầy, Docker daemon sẽ **crash giữa chừng khi build** mà không báo lỗi rõ ràng.

**Kiểm tra dung lượng trống:**
```powershell
Get-PSDrive C | ForEach-Object { "Free: $([math]::Round($_.Free/1GB,1))GB" }
```

Cần ít nhất **3-4GB free** trước khi build. Nếu không đủ, xóa cache Docker trước:

```bash
# Xóa build cache + dangling images (nhẹ, ~2-5GB)
docker system prune -f

# Xóa toàn bộ images không dùng (mạnh hơn, ~10-15GB — images sẽ pull lại sau)
docker system prune -a -f
```

Xóa thêm Temp nếu cần:
```powershell
Remove-Item -Recurse -Force $env:TEMP\* -ErrorAction SilentlyContinue
```

Copy `.env.example` to `.env` and fill in values before running Docker. Required variables:

| Variable | Purpose |
|---|---|
| `POSTGRES_DB`, `POSTGRES_USER`, `POSTGRES_PASSWORD` | PostgreSQL credentials |
| `JWT_SECURITY_KEY`, `JWT_ISSUER`, `JWT_AUDIENCE` | JWT config (access 15 min, refresh 7 days) |
| `CORS_ORIGINS` | Semicolon-separated allowed origins (overrides `appsettings.json`) |
| `CLOUDINARY_URL_UPLOAD`, `CLOUDINARY_CLOUD_NAME`, `CLOUDINARY_API_KEY`, `CLOUDINARY_API_SECRET`, `CLOUDINARY_UPLOAD_FOLDER` | Cloudinary credentials |
| `FRONTEND_PORT` | Host port for frontend container (default 80) |

HTTPS redirect is suppressed inside Docker (`DOTNET_RUNNING_IN_CONTAINER=true`).

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

Every `dotnet run` runs idempotent seeders after migrations:
- `LanguageSeeder` — seeds translation keys and their EN/VI values
- `AlbumSeeder`, `ArticleSeeder`, `HeroSlideSeeder`, `SysConfigSeeder`, `ContentTranslationSeeder` — seed default content

When adding new translation keys, add them to `LanguageSeeder.GetEntries()` so they are present on the next startup.

### Pagination

Both sides use the same shape. Backend produces `PaginationItem<T>` (`data`, `totalItems`, `pageNumber`, `pageSize`, `pageCount`); frontend consumes the same interface and feeds it to the shared `<app-pagination>` component.

### File Uploads — Cloudinary Signed Upload Flow

Cloudinary uploads go **directly from the browser to Cloudinary** (not through the backend). The backend only stores metadata. Upload flow:

1. Frontend calls `GET api/extend-service/cloudinary/upload-file-by-signature?type=image|video`
2. Backend (`CloudinaryController`) calls `CloudinaryService.GetSignedUploadResponse()` which computes a SHA-1 signature and returns a `CloudinarySignature` object:
   - `uploadUrl` — `https://api.cloudinary.com/v1_1/{cloudName}/{resourceType}/upload`
   - `apiKey`, `timestamp`, `signature`, `folder`
3. Frontend (`FileCloudinaryService`) POSTs a `FormData` to `uploadUrl` containing: `file` + `api_key` + `timestamp` + `signature` + `folder`. All five fields are required — Cloudinary rejects the upload without them.
4. Cloudinary returns `secure_url` (HTTPS). Always use `secure_url`, never `url` (HTTP).

`CloudinarySignature.cs` lives in `backend/Services/Extends/` (not in `ViewModels/`). `CloudinarySignedParams` interface lives in `font-end/src/app/core/services/file.cloudinary.service.ts`.

Cloudinary config is in `appsettings.json` under `"Cloudinary": { UrlUpload, CloudName, ApiKey, ApiSecret, UploadFolder }`.

Video uploads use resource type `video` — the URL path contains `/video/upload/`. Detect videos via `isVideoUrl()` which checks for `/video/upload/` in the URL. Video thumbnails on the public photography page are generated by replacing `.mp4`/`.webm`/`.mov` with `.jpg` in the Cloudinary URL (auto-thumbnail feature).

### File Metadata

`File.Title` and `File.Description` are **optional** — the `CreateFileRequest` and `UpdateFileRequest` validators do not require them. Batch drag-and-drop upload creates files with empty title/description; users fill them in later via the edit modal in the album admin. Only `Name` and `Url` are required on create.

### Config-driven Social Links

Social media links in the sidebar come from `ConfigService` (`config.social[].ref`). These are **external URLs** (e.g. `https://facebook.com`). Use `[href]="item.ref"` with `target="_blank" rel="noopener noreferrer"` — never `[routerLink]`, which treats external URLs as relative paths and prepends the app origin.

### Angular 21 Zoneless Change Detection

Angular 21 defaults to **zoneless mode** — `ApplicationRef.tick()` is a no-op, plain property assignments in HTTP subscribe callbacks do not update the view. Fixed globally via `font-end/src/app/core/interceptors/zone.interceptor.ts` which calls `queueMicrotask(() => appRef.components[0]?.changeDetectorRef.detectChanges())` after every HTTP response. See `font-end/CLAUDE.md` for full details and the list of failed approaches to avoid.

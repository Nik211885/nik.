# Frontend — Angular 21

## Project Structure

```
font-end/src/app/
├── admin/                  # Admin panel (lazy-loaded, /admin routes)
│   ├── features/           # One component per resource (articles, albums, …)
│   ├── layout/             # AdminLayoutComponent + AdminSidebarComponent
│   ├── services/           # Admin-specific HTTP services (*.admin.service.ts)
│   ├── shared/             # admin-table, admin-confirm-modal, cloudinary-upload
│   └── models/             # admin.model.ts
├── core/
│   ├── auth/               # Auth service, guard, models
│   ├── interceptors/       # authInterceptor, acceptLanguageInterceptor
│   └── services/           # App-wide singletons (LanguageService, ConfigService)
├── features/               # Public page-level components (one per route)
│   ├── home/
│   ├── photography/
│   ├── travel/
│   ├── fashion/
│   ├── about/
│   ├── contact/
│   └── post-detail/
├── layout/
│   └── main-layout/        # Shell layout + sidebar + footer
├── shared/
│   ├── components/         # Reusable UI components
│   ├── models/             # TypeScript interfaces
│   └── pipes/              # Custom pipes
├── app.ts                  # Root component
├── app.routes.ts           # Route definitions
├── app.config.ts           # DI providers + app initializers
└── app.message.ts          # Translation key constants (AdminMessage, ApplicationTitle enums)
```

## Naming Conventions

| Element             | Convention            | Example                            |
|---------------------|-----------------------|------------------------------------|
| Components          | PascalCase + Component| `HomeComponent`, `PostCardComponent` |
| Services            | PascalCase + Service  | `AuthService`, `LanguageService`   |
| Interfaces/models   | PascalCase + Model    | `PostModel`, `UserProfile`         |
| Files               | kebab-case            | `post-card.component.ts`           |
| Constants           | UPPER_SNAKE_CASE      | `TOKEN_KEY = 'access_token'`       |
| Private fields      | camelCase             | `private authService: AuthService` |
| Observables         | `$` suffix            | `currentLanguage$`, `lang$`        |
| BehaviorSubjects    | `Subject` suffix      | `dictionarySubject`                |

## Template Control Flow

Always use Angular 17+ built-in control flow. **Never** use structural directives `*ngIf`, `*ngFor`, `*ngSwitch`.

```html
<!-- ✅ correct -->
@if (show) {
<div>...</div>
}

@if (show) {
<div>...</div>
} @else {
<div>...</div>
}

@for (item of items; track item.id) {
<div>{{ item.name }}</div>
}

@switch (status) {
  @case ('active') { <span>Active</span> }
  @default { <span>Inactive</span> }
}

<!-- ❌ forbidden -->
<div *ngIf="show">...</div>
<div *ngFor="let item of items">...</div>
<div [ngSwitch]="status">...</div>
```

`@for` requires a `track` expression — use a unique field (`track item.id`) or `track $index` when no id exists.

With `@if`/`@for`, `CommonModule` is not needed for control flow, but keep it if the template uses pipes it provides (`AsyncPipe`, `DatePipe`, etc.).

## i18n — Static Text via `app.message.ts`

All user-facing strings (page titles, labels, button text, descriptions) **must** use translation keys, never hardcoded text.

**Step 1** — declare the key in `app.message.ts`:

```typescript
export enum ApplicationTitle {
  LOGIN_TITLE = 'login.title',
  LOGIN_SUBMIT = 'login.submit',
}
```

**Step 2** — use `LanguagePipe` in the template:

```html
<h4>{{ 'login.title' | language }}</h4>
<!-- or with enum -->
<button>{{ ApplicationTitle.LOGIN_SUBMIT | language }}</button>
```

**Step 3** — add the key and its translated value to the backend translation table via the admin `/translations` page (one row per language).

**Step 4** — for TypeScript code, use `LanguageService.translate(key)`:

```typescript
this.langSvc.translate(ApplicationTitle.LOGIN_TITLE)
```

> Admin panel UI also uses `AdminMessage` enum keys + `LanguagePipe`. Add keys to `app.message.ts` under the `AdminMessage` enum and register translations in the `/admin/translations` page.

## i18n Enforcement Rules — Zero Hardcoded Strings

**Every visible string in every template must go through `| language`.** No exceptions.

This applies to:
- Element text content: `<button>`, `<label>`, `<span>`, `<p>`, `<h*>`, `<th>`, `<option>`, `<small>` …
- Attribute values that users see: `placeholder`, `title`, `alt`
- `[title]="AdminMessage.KEY | language"` — use property binding for translated attributes

**What NOT to translate:** format hints in `placeholder` like `"vi, en, fr..."` or `"exception.not_found"` (technical examples), inline SVG fallback text, and pure icon classes.

**Checklist when adding any user-facing text:**

1. Add the key to `app.message.ts` → `AdminMessage` (admin UI) or `ApplicationTitle` (public site)
2. Use `{{ AdminMessage.KEY | language }}` in the template (or `[attr]="AdminMessage.KEY | language"`)
3. Add the key + EN + VI translations to `backend/Data/LanguageSeeder.cs` → `GetEntries()`
4. Restart the backend — `LanguageSeeder` runs on startup and is idempotent

```typescript
// app.message.ts
ALBUMS_EXPAND_ALL = 'admin.albums.expand-all',

// template
{{ AdminMessage.ALBUMS_EXPAND_ALL | language }}

// LanguageSeeder.cs
("admin.albums.expand-all", "Expand all", "Mở rộng tất cả"),
```

**Never** write Vietnamese or English text directly in a template. If a key is missing from the seeder, the pipe shows the raw key string as a fallback — which makes it easy to spot omissions.

## Component Pattern

All components are **standalone**:

```typescript
@Component({
  selector: 'app-post-card',
  standalone: true,
  imports: [CommonModule, RouterLink, LanguagePipe],
  templateUrl: './post-card.component.html',
  styleUrl: './post-card.component.css'
})
export class PostCardComponent { }
```

- Always `standalone: true` — no NgModule
- Import only what the template actually uses
- One component per file with matching `.html` and `.css` files

## Service Pattern

```typescript
@Injectable({ providedIn: 'root' })
export class LanguageService {
  private readonly STORAGE_KEY = 'app_language';
  private readonly DEFAULT_LANG = 'en';

  private currentLang = this.DEFAULT_LANG;
  private currentLanguageSubject = new BehaviorSubject<string>(this.DEFAULT_LANG);
  currentLanguage$ = this.currentLanguageSubject.asObservable();

  constructor(private http: HttpClient) {}
}
```

- Use `providedIn: 'root'` for app-wide singletons
- Expose reactive state as public `Observable` (`.asObservable()`) — never expose `BehaviorSubject` directly
- Keep private `BehaviorSubject` with `Subject` suffix, public `Observable` with `$` suffix

## HTTP Interceptors

Use the **functional interceptor** pattern (not class-based):

```typescript
export const authInterceptor: HttpInterceptorFn = (
  request: HttpRequest<unknown>,
  next: HttpHandlerFn
): Observable<any> => {
  const authService = inject(AuthService);
  // ...
  return next(request);
};
```

- Use `inject()` inside the function body (not constructor injection)
- Register in `app.config.ts` with `provideHttpClient(withInterceptors([authInterceptor]))`

## Custom Headers for Interceptor Control

Two special request headers control the `authInterceptor` behavior:

| Header                    | Effect                                      |
|---------------------------|---------------------------------------------|
| `X-Skip-Auth-Request`     | Skip the entire interceptor (no URL rewrite, no token) |
| `X-Skip-Auth-Bear-Token`  | Skip Bearer token attachment only           |

URLs containing `cloudinary.com` skip the interceptor automatically.
URLs containing `public-api` skip Bearer token automatically.

## Token Refresh Strategy

The auth interceptor handles 401 by:
1. Calling `authService.refreshToken()` (only once, even if multiple requests fail)
2. Queuing other in-flight requests via `refreshTokenSubject$`
3. Retrying all queued requests once the new token arrives
4. Calling `authService.logout()` if refresh fails

## Authentication Storage

| Key             | Storage      | Value           |
|-----------------|--------------|-----------------|
| `access_token`  | localStorage | JWT access token |
| `refresh_token` | localStorage | JWT refresh token |
| `app_language`  | localStorage | Language code (`en`, `vi`) |

## API URL Convention

Base URL is configured per environment:

```typescript
// environment.ts
export const environment = {
  baseApiUrl: 'http://localhost:5055'
};
```

- `authInterceptor` prepends `baseApiUrl` to all relative request URLs
- Use relative paths in service calls: `this.http.get('api/albums')` not full URLs
- Exception: Cloudinary uploads use absolute URLs and bypass the interceptor

## i18n / Translation Pattern

Translation keys are defined in `app.message.ts`:

```typescript
export enum AppMessage {
  WELCOME = 'home.welcome',
  // ...
}
```

Use the `LanguagePipe` in templates:

```html
{{ 'home.welcome' | language }}
```

Use `LanguageService.translate(key)` in TypeScript code.
Use `LanguageService.withLanguage(lang => this.http.get(...))` to auto-reload data on language change.

## Pagination

Services return `PaginationItem<T>` from the backend:

```typescript
interface PaginationItem<T> {
  data: T[];
  totalItems: number;
  pageNumber: number;
  pageSize: number;
  pageCount: number;
}
```

The shared `<app-pagination>` component handles navigation.

## Route Structure

```typescript
// app.routes.ts
export const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      { path: '', component: HomeComponent },
      { path: 'photography', component: PhotographyComponent },
      { path: 'post/:slug', component: PostDetailComponent },
      // ...
    ]
  }
];
```

- Feature routes are children of `MainLayoutComponent`
- Use slug-based routes for content pages (`/post/:slug`, `/album/:slug`)

## Auth Guard

```typescript
// Functional guard
export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  return authService.isAuthenticated() ? true : inject(Router).createUrlTree(['/']);
};
```

## Admin Table Component

`app-admin-table` is the generic data table used across every admin feature. Key inputs/outputs:

```typescript
@Input() columns: TableColumn[]   // column definitions (header, field, sortable, etc.)
@Input() data: any[]              // rows to display
@Input() loading = false          // show spinner overlay
@Input() showActions = true       // show edit / delete action buttons
@Input() hideDelete = false       // hide delete button only
@Input() rowKey = 'id'            // field used as row identity for selection

@Output() editClick = new EventEmitter<any>()     // emits the selected row
@Output() deleteClick = new EventEmitter<any[]>() // emits array of selected rows
```

The component provides built-in column filtering, header-click sorting, pagination, and multi-select with a bulk-action bar. Admin feature components wire it up and handle the emitted events themselves.

## Angular Signals

Only `LoadingService` uses Angular signals (`signal<boolean>`, `signal<string>`) with `.asReadonly()` for the public surface. All other services use RxJS `BehaviorSubject`/`Observable`. There is no use of `computed()` or `effect()` in this codebase.

## Shared Components Inventory

| Component              | Description                          |
|------------------------|--------------------------------------|
| `app-post-card`        | Article card with image, title, tags |
| `app-post-card-compact`| Minimal article card for sidebar     |
| `app-hero-carousel`    | Full-width hero slider               |
| `app-hero-section`     | Static hero banner                   |
| `app-pagination`       | Page navigation controls             |
| `app-tag-cloud`        | Tag list display                     |
| `app-sidebar-list`     | Generic sidebar list                 |
| `app-post-comment`     | Single comment display               |
| `app-post-comment-list`| Threaded comment list                |
| `app-input-comment`    | Comment submission form              |
| `app-photo-masonry-grid`| Masonry photo layout                |
| `app-search-input`     | Search bar component                 |
| `app-loading`          | Spinner overlay                      |
| `app-toast`            | Toast notification                   |

## Custom Pipes

| Pipe         | Usage                            |
|--------------|----------------------------------|
| `language`   | Translate a key: `{{ key \| language }}` |
| `truncate`   | Trim text to N chars             |
| `appDate`    | Format `DateTimeOffset` strings  |

## Comments in TypeScript

The Angular core services (like `LanguageService`, `authInterceptor`) use heavy JSDoc comments to explain the intent of each block. Follow this style for complex service logic:

```typescript
/**
 * Handle 401 Unauthorized error
 *
 * Strategy:
 * - If not refreshing → call refresh token API
 * - If refreshing → queue request until token is ready
 */
function handle401(...) { }
```

For simple components and feature pages, omit comments — code should be self-explanatory.

## Adding a New Feature — Checklist

1. Create `features/{name}/{name}.component.ts` (+ `.html`, `.css`) as standalone component
2. Add route in `app.routes.ts` under `MainLayoutComponent` children
3. Add any shared models to `shared/models/`
4. Create services in `core/services/` if data is shared; keep local if feature-specific
5. Use `LanguagePipe` for all user-facing strings
6. Register new translation keys in `app.message.ts`

## Angular 21 Zoneless Change Detection

Angular 21 bootstraps with **zoneless mode by default** (`provideZonelessChangeDetectionInternal()` is injected automatically before user providers). This means:

- `NgZone` is replaced with `NoopNgZone` — zone.js does **not** trigger Angular CD
- `ApplicationRef.tick()` is a **no-op** in zoneless mode (skips dirty flag setup, loop never runs)
- Plain property assignments in subscribe callbacks (e.g. `this.items = data`) **do not trigger view updates**
- DOM events from Angular template bindings (`(click)="..."`) **do** trigger CD because Angular's renderer wraps them

### The fix — `zone.interceptor.ts`

`font-end/src/app/core/interceptors/zone.interceptor.ts` is a global HTTP interceptor that forces CD after every HTTP response:

```typescript
tap({
  next: () => {
    queueMicrotask(() => {
      appRef.components[0]?.changeDetectorRef.detectChanges();
    });
  }
})
```

**Why `queueMicrotask`:** `tap` runs before the component's `.subscribe()` callback. The microtask defers `detectChanges()` until after the component has assigned `this.data = response`, so the view update picks up the new values.

**Why `detectChanges()` works:** It sets `lView[FLAGS] |= 1024` directly and calls `detectChangesInternal(lView, mode=0)` — a full global tree check that bypasses the dirty-flag guards that block `tick()` in zoneless mode.

### What NOT to do

| Approach | Why it fails |
|---|---|
| `provideZoneChangeDetection()` in providers | Conflicts with Angular's default `provideZonelessChangeDetectionInternal()` → blank page |
| `provideZoneChangeDetection({ eventCoalescing: true })` | Same conflict, worse — coalescing causes bootstrap failure |
| `appRef.tick()` | No-op in zoneless: `if (!this.zonelessEnabled) { this.dirtyFlags \|= 1; }` guard skips setup |
| `NgZone.run()` | `NgZone` is `NoopNgZone` — just calls the function directly, no CD |
| `tap(detectChanges)` synchronously | Runs before component sets data — detects nothing new |

## Production Environment

```typescript
// environment.prod.ts
export const environment = {
  baseApiUrl: ''  // empty string = relative URLs, works behind a reverse proxy
};
```

## Prettier Config

Prettier is configured (`.prettierrc`): 100-char line width, single quotes, Angular parser for `.html` files. Run `npx prettier --write .` to format.

## Notable Third-party Packages

| Package | Purpose |
|---|---|
| `ngx-quill` + `quill` | Rich-text editor (articles) |
| `leaflet` | Interactive maps |
| `chart.js` | Dashboard charts in admin |
| `franc-min` | Language detection |
| `aos` | Animate-on-scroll library |
| `xlsx` | Excel export |
| `wrangler` | Cloudflare Workers CLI |

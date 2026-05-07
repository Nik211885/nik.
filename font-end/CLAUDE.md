# Frontend — Angular 21

## Project Structure

```
font-end/src/app/
├── core/
│   ├── auth/               # Auth service, guard, models
│   ├── interceptors/       # HTTP interceptors
│   └── services/           # App-wide singleton services
├── features/               # Page-level components (one per route)
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
├── app.config.ts           # DI providers
└── app.message.ts          # Translation key constants
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
  baseApiUrl: 'http://localhost:5173'
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

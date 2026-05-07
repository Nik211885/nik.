# Backend — ASP.NET Core 10

## Project Structure

```
backend/
├── Controllers/              # HTTP endpoints, one file per resource
├── Services/
│   ├── Internals/            # Business logic services
│   └── Extends/              # Cross-cutting services (caching, etc.)
├── Data/
│   ├── ApplicationDbContext.cs
│   ├── EntityConfigurations/ # EF Core Fluent API configs, one per entity
│   └── Migrations/
├── Entities/                 # Domain models
├── ViewModels/
│   └── {Feature}/
│       ├── Requests/         # Input DTOs + validators + mappers
│       └── Responses/        # Output DTOs + mappers
├── Exceptions/               # Custom exception types
├── Extensions/               # C# extension members
├── Helpers/                  # Static utility classes
├── Pipes/
│   ├── Filter/               # Action filters (ValidationFilter)
│   └── Middlewares/          # ASP.NET Core middleware
└── ApplicationMessage.cs     # Centralized message/key constants
```

## Naming Conventions

| Element              | Convention        | Example                          |
|----------------------|-------------------|----------------------------------|
| Classes              | PascalCase        | `AlbumController`, `AlbumServices` |
| Async methods        | PascalCase + Async| `CreateAlbumAsync`               |
| Properties           | PascalCase        | `public string Name { get; set; }` |
| Private fields       | `_camelCase`      | `private readonly AlbumServices _albumServices` |
| Namespaces           | `backend.{Layer}` | `backend.Services.Internals`     |
| DB table names       | Plural PascalCase | `"Albums"`, `"Articles"`         |

## Entity Conventions

All entities inherit `BaseEntity`:

```csharp
public abstract class BaseEntity
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
}
```

- IDs are **strings** containing UUIDv7 (not int, not Guid type)
- `CreatedDate` and `UpdatedDate` are `DateTimeOffset` (not `DateTime`)
- Timestamps are always set to `DateTimeOffset.UtcNow`
- Counter fields (`CountRef`, `CountSee`, etc.) default to `0` in EF config

## Entity Configuration

Each entity has a dedicated configuration class in `Data/EntityConfigurations/`:

```csharp
public class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.ToTable("Articles");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasMaxLength(20);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(255);
        builder.Property(x => x.CountCommentRef).HasDefaultValue(0);
        builder.HasOne(x => x.Author).WithMany().HasForeignKey(a => a.AuthorId);
    }
}
```

- Always define table names explicitly with `ToTable()`
- Always define max lengths for string properties
- Set `HasDefaultValue(0)` for counter fields instead of relying on CLR defaults

## ViewModels Pattern

Each feature folder under `ViewModels/{Feature}/` contains:

**Requests/** — Input DTO + FluentValidation validator + mapping extension (all in the same file):

```csharp
// CreateAlbumRequest.cs
public class CreateAlbumRequest
{
    public string Name { get; set; }
    public string Title { get; set; }
}

public class CreateAlbumRequestValidator : AbstractValidator<CreateAlbumRequest>
{
    public CreateAlbumRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
    }
}

public static class CreateAlbumRequestExtensions
{
    extension(CreateAlbumRequest request)
    {
        public Album ToAlbum()
        {
            return new Album { Name = request.Name, Title = request.Title };
        }
    }
}
```

**Responses/** — Output DTO + mapping extensions (all in the same file):

```csharp
// AlbumResponse.cs
public class AlbumResponse { ... }

public static class AlbumResponseExtensions
{
    extension(Album album)
    {
        public AlbumResponse ToAlbumResponse() { ... }
    }
    extension(IQueryable<Album> albums)
    {
        public IQueryable<AlbumResponse> ToAlbumResponses()
        {
            return albums.Select(album => album.ToAlbumResponse());
        }
    }
}
```

## C# Extension Member Syntax

This project uses **C# 14 extension members** (preview feature in .NET 10). The syntax is:

```csharp
public static class SomeExtensions
{
    extension(TargetType instance)
    {
        public ReturnType MethodName() { ... }
    }

    // Generic extensions
    extension<T>(IQueryable<T> queryable)
    {
        public async Task<PaginationItem<T>> PaginationItemAsync(...) { ... }
    }
}
```

This is **not** the classic `this` extension method syntax. Do not convert to classic syntax.

## Controller Pattern

```csharp
[ApiController]
[Route("api/albums")]
public class AlbumController : ControllerBase
{
    private readonly ILogger<AlbumController> _logger;
    private readonly AlbumServices _albumServices;

    public AlbumController(ILogger<AlbumController> logger, AlbumServices albumServices)
    {
        _logger = logger;
        _albumServices = albumServices;
    }

    [HttpPost("create")]
    [ValidationFilter(typeof(CreateAlbumRequest))]
    public async Task<ActionResult> CreateAlbum(CreateAlbumRequest request)
    {
        var result = await _albumServices.CreateAlbumAsync(request);
        return Ok(result);
    }
}
```

- Route: `[Route("api/{resource}")]` — always lowercase, plural
- Create endpoints: `POST /create` with `[ValidationFilter(typeof(RequestType))]`
- All action methods are `async Task<ActionResult>` or `async Task<IActionResult>`
- Inject `ILogger<T>` as first constructor parameter always

### Parameter Binding Rules

| Scenario | Attribute | Example |
|---|---|---|
| POST / PUT body | `[FromBody]` | `[FromBody] CreateAlbumRequest request` |
| GET / DELETE — primitive | `[FromQuery]` | `[FromQuery] bool tree = false` |
| GET / DELETE — object or collection | `[AsParameters]` | `[AsParameters] PaginationRequest request` |
| Path segment | _(none needed)_ | `string id` in route `{id}` |

**Do not use `[FromQuery]` on complex objects** — `[AsParameters]` is the correct binding for objects and collections in GET/DELETE. `[FromQuery]` only applies to individual primitive values.

## Service Pattern

```csharp
public class AlbumServices
{
    private readonly ILogger<AlbumServices> _logger;
    private readonly ApplicationDbContext _dbContext;

    public AlbumServices(ILogger<AlbumServices> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<AlbumResponse> CreateAlbumAsync(CreateAlbumRequest request)
    {
        // 1. Normalize/validate input
        // 2. Check business rules, throw on violation
        // 3. Map request → entity, set computed fields
        // 4. Save to DB
        // 5. Return mapped response
    }
}
```

- Direct DB access via `ApplicationDbContext` (no repository layer)
- Use `AsNoTracking()` for all read-only queries
- Use `ExecuteDeleteAsync()` for bulk deletes (no load then delete)
- Check existence with `AnyAsync()`, not `FirstOrDefaultAsync() != null`
- Use `FirstOrDefaultAsync() ?? throw new NotFoundException()` for required lookups

## Exception Handling

Throw typed exceptions from services; controllers never handle business errors:

```csharp
throw new NotFoundException();                          // 404
throw new NotFoundException(ApplicationMessage.NotFound);
throw new BadRequestException();                        // 400
throw new BadRequestException(ApplicationMessage.ExitsCode);
```

All error messages are keys defined in `ApplicationMessage.cs` (never hardcode strings):

```csharp
public static class ApplicationMessage
{
    public static readonly string NotFound = "exception.not_found";
    public static readonly string BadRequest = "exception.bad_request";
    public static readonly string ExitsCode = "exception.exits_code";
    // validation.*  for FluentValidation messages
}
```

Message values are i18n translation keys, not human-readable strings.

## Validation

- Use `[ValidationFilter(typeof(RequestType))]` on controller actions
- Define `{Request}Validator : AbstractValidator<{Request}>` in the same file as the request class
- Use `ApplicationMessage` constants for `.WithMessage()` in validators
- `ValidationFilter` returns `400 { message, errors: { field: [messages] } }` on failure

## Pagination

Use `PaginationRequest` input and `PaginationItem<T>` output:

```csharp
public async Task<PaginationItem<ReactionResponse>> GetPaginationAsync(PaginationRequest model)
{
    return await _context.Reactions
        .OrderByDescending(x => x.CreatedDate)
        .ToReactionResponses()           // project to DTO first
        .PaginationItemAsync(model);     // extension on IQueryable<T>
}
```

- Always project to response DTO before paginating (`.ToXxxResponses()` then `.PaginationItemAsync()`)
- `PaginationItemAsync` handles defaults: page 1 if `PageNumber <= 0`, size 10 if `PageSize <= 0`

## Slug Generation

Use the `ToSlug()` extension on strings:

```csharp
album.Slug = request.Name.ToSlug();
```

Format: `{6-char-random}/{normalized-name}` — e.g., `ab3x7k/my-album-name`.

## Database Queries — Required Patterns

```csharp
// Read-only queries: always AsNoTracking
var items = await _dbContext.Albums.AsNoTracking().ToListAsync();

// Required entity: ?? throw pattern
var entity = await _dbContext.Albums.FirstOrDefaultAsync(x => x.Id == id)
    ?? throw new NotFoundException();

// Bulk delete: ExecuteDeleteAsync (no load)
await _dbContext.Reactions
    .Where(x => x.ArticleId == id)
    .ExecuteDeleteAsync();

// Existence check
bool exists = await _dbContext.Albums.AnyAsync(x => x.Name == name);
```

## API Endpoint Conventions

| Action      | HTTP Method | Route pattern           |
|-------------|-------------|-------------------------|
| Create      | POST        | `/api/{resource}/create`|
| Update      | PUT         | `/api/{resource}/update`|
| Delete      | DELETE      | `/api/{resource}/delete`|
| Get all     | GET         | `/api/{resource}`       |
| Get by ID   | GET         | `/api/{resource}/{id}`  |
| Get by slug | GET         | `/api/{resource}/slug/{slug}` |

## User Identity

Extract current user ID from JWT claims via the `HttpContext` extension:

```csharp
var userId = _httpContext.HttpContext!.GetUserId();
```

Inject `IHttpContextAccessor _httpContext` when user context is needed.

## Self-referential Entities

Album and Comment use self-referential relationships. Pattern for building trees:

```csharp
// Build a lookup keyed by parent ID
var lookup = all.ToLookup<AlbumResponse, string?>(a => a.AlbumId);
var roots = lookup[null].ToList();   // null key = top-level items
foreach (var root in roots)
    PopulateChildren(root, lookup);  // recursive DFS
```

## Adding a New Feature — Checklist

1. Add entity class in `Entities/` extending `BaseEntity`
2. Add EF config in `Data/EntityConfigurations/` implementing `IEntityTypeConfiguration<T>`
3. Register `DbSet<T>` in `ApplicationDbContext`
4. Create `ViewModels/{Feature}/Requests/{Action}Request.cs` with validator + mapper extension
5. Create `ViewModels/{Feature}/Responses/{Feature}Response.cs` with mapper extensions
6. Create `Services/Internals/{Feature}Services.cs`
7. Register service in `Services/Internals/AddServicesInternalExtensions.cs`
8. Create `Controllers/{Feature}Controller.cs`
9. Add migration: `dotnet ef migrations add {Name}`

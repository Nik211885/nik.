using backend.Data;
using backend.Entities;
using backend.Exceptions;
using backend.Extensions;
using backend.ViewModels.Albums.Requests;
using backend.ViewModels.Albums.Responses;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Internals;

/// <summary>
/// Provides all business operations for photo albums, including hierarchical tree building,
/// file management, and CRUD.
/// </summary>
public class AlbumServices
{
    private readonly ILogger<AlbumServices> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContext;
    private readonly ContentTranslationService _translationService;

    /// <summary>Initialises the service with required dependencies.</summary>
    public AlbumServices(
        ILogger<AlbumServices> logger,
        ApplicationDbContext dbContext,
        IHttpContextAccessor httpContext,
        ContentTranslationService translationService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _httpContext = httpContext;
        _translationService = translationService;
    }

    /// <summary>
    /// Creates a new album. The name is normalised to lowercase and must be unique.
    /// </summary>
    /// <param name="request">Album creation payload.</param>
    /// <returns>The created album response.</returns>
    /// <exception cref="BadRequestException">Thrown when an album with the same name already exists.</exception>
    public async Task<AlbumResponse> CreateAlbumAsync(CreateAlbumRequest request)
    {
        request.Name = request.Name.ToLowerInvariant();

        var existingAlbum = await _dbContext.Albums
            .FirstOrDefaultAsync(a => a.Name == request.Name);

        if (existingAlbum is not null)
            throw new BadRequestException(ApplicationMessage.ExitsCode);

        var album = request.ToAlbum();
        album.Slug = request.Name.ToSlug();
        album.CreatedDate = DateTimeOffset.UtcNow;
        album.UpdatedDate = DateTimeOffset.UtcNow;
        album.CountImageRef = 0;

        _dbContext.Albums.Add(album);
        await _dbContext.SaveChangesAsync();

        return album.ToAlbumResponse();
    }

    /// <summary>
    /// Updates an existing album. Regenerates the slug when the name changes.
    /// </summary>
    /// <param name="request">Update payload.</param>
    /// <returns>The updated album response.</returns>
    /// <exception cref="BadRequestException">Thrown when another album already uses the requested name.</exception>
    /// <exception cref="NotFoundException">Thrown when the album ID does not exist.</exception>
    public async Task<AlbumResponse> UpdateAlbumAsync(UpdateAlbumRequest request)
    {
        request.Name = request.Name.ToLowerInvariant();

        var duplicateName = await _dbContext.Albums
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Name == request.Name);

        if (duplicateName is not null && duplicateName.Id != request.Id)
            throw new BadRequestException(ApplicationMessage.ExitsCode);

        var album = await _dbContext.Albums
            .FirstOrDefaultAsync(a => a.Id == request.Id)
            ?? throw new NotFoundException();

        if (album.Name != request.Name)
            album.Slug = request.Name.ToSlug();

        request.ApplyTo(album);
        album.UpdatedDate = DateTimeOffset.UtcNow;

        _dbContext.Update(album);
        await _dbContext.SaveChangesAsync();

        return album.ToAlbumResponse();
    }

    /// <summary>
    /// Sets or clears the cover image for an album.
    /// </summary>
    /// <param name="request">Album ID and optional file ID to use as cover.</param>
    /// <returns>The updated album response.</returns>
    /// <exception cref="NotFoundException">Thrown when the album ID does not exist.</exception>
    public async Task<AlbumResponse> SetCoverAsync(SetCoverRequest request)
    {
        var album = await _dbContext.Albums
            .FirstOrDefaultAsync(a => a.Id == request.AlbumId)
            ?? throw new NotFoundException();

        album.FileDescriptionId = request.FileId;
        album.UpdatedDate = DateTimeOffset.UtcNow;

        await _dbContext.SaveChangesAsync();

        return await _dbContext.Albums
            .AsNoTracking()
            .Where(a => a.Id == album.Id)
            .Select(a => new AlbumResponse
            {
                Id = a.Id,
                Name = a.Name,
                Title = a.Title,
                Description = a.Description,
                Slug = a.Slug,
                CountImageRef = a.CountImageRef,
                FileDescriptionId = a.FileDescriptionId,
                CoverUrl = a.File != null ? a.File.Url : null,
                OrderIndex = a.OrderIndex,
                AlbumId = a.AlbumId,
                CreatedDate = a.CreatedDate,
                UpdatedDate = a.UpdatedDate
            })
            .FirstAsync();
    }

    /// <summary>
    /// Deletes one or more albums by ID and removes their translations.
    /// Detaches child albums (sets their parent to null) and removes album-file records before deleting.
    /// </summary>
    /// <param name="ids">IDs of albums to delete.</param>
    public async Task DeleteAlbumAsync(List<string> ids)
    {
        await _dbContext.Albums
            .Where(a => a.AlbumId != null && ids.Contains(a.AlbumId))
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.AlbumId, (string?)null));

        await _dbContext.AlbumFiles.Where(af => ids.Contains(af.AlbumId)).ExecuteDeleteAsync();
        await _dbContext.Albums.Where(a => ids.Contains(a.Id)).ExecuteDeleteAsync();
        await _translationService.DeleteByEntityAsync(EntityType.Album, ids);
    }

    /// <summary>
    /// Adds files to an album, skipping any file IDs already associated.
    /// Updates <see cref="Album.CountImageRef"/> to reflect the new total.
    /// </summary>
    /// <param name="request">Album ID and list of file IDs to add.</param>
    /// <returns>The newly added album-file records.</returns>
    /// <exception cref="NotFoundException">Thrown when the album ID does not exist.</exception>
    public async Task<IReadOnlyCollection<AlbumFileResponse>> AddFilesToAlbumAsync(AddFilesToAlbumRequest request)
    {
        var album = await _dbContext.Albums
            .FirstOrDefaultAsync(a => a.Id == request.AlbumId)
            ?? throw new NotFoundException();

        var existingFileIds = await _dbContext.AlbumFiles
            .AsNoTracking()
            .Where(af => af.AlbumId == request.AlbumId)
            .Select(af => af.FileId)
            .ToListAsync();

        var newFileIds = request.FileIds
            .Except(existingFileIds)
            .ToList();

        if (newFileIds.Count == 0)
            return [];

        var albumFiles = newFileIds.Select(fileId => new AlbumFile
        {
            AlbumId = request.AlbumId,
            FileId = fileId
        }).ToList();

        _dbContext.AlbumFiles.AddRange(albumFiles);

        album.CountImageRef = await _dbContext.AlbumFiles
            .CountAsync(af => af.AlbumId == request.AlbumId) + newFileIds.Count;

        await _dbContext.SaveChangesAsync();

        return await _dbContext.AlbumFiles
            .AsNoTracking()
            .Where(af => af.AlbumId == request.AlbumId && newFileIds.Contains(af.FileId))
            .ToAlbumFileResponses()
            .ToListAsync();
    }

    /// <summary>
    /// Removes files from an album and decrements <see cref="Album.CountImageRef"/>
    /// by the number of records actually deleted.
    /// </summary>
    /// <param name="request">Album ID and file IDs to remove.</param>
    /// <exception cref="NotFoundException">Thrown when the album ID does not exist.</exception>
    public async Task RemoveFilesFromAlbumAsync(RemoveFilesFromAlbumRequest request)
    {
        var album = await _dbContext.Albums
            .FirstOrDefaultAsync(a => a.Id == request.AlbumId)
            ?? throw new NotFoundException();

        var deleted = await _dbContext.AlbumFiles
            .Where(af => af.AlbumId == request.AlbumId && request.FileIds.Contains(af.FileId))
            .ExecuteDeleteAsync();

        album.CountImageRef = Math.Max(0, album.CountImageRef - deleted);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Returns all files belonging to the specified album.
    /// </summary>
    /// <param name="albumId">ID of the album.</param>
    /// <returns>Read-only collection of album-file responses.</returns>
    /// <exception cref="NotFoundException">Thrown when the album ID does not exist.</exception>
    public async Task<IReadOnlyCollection<AlbumFileResponse>> GetAlbumFilesAsync(string albumId)
    {
        if (!await _dbContext.Albums.AsNoTracking().AnyAsync(a => a.Id == albumId))
            throw new NotFoundException();

        return await _dbContext.AlbumFiles
            .AsNoTracking()
            .Where(af => af.AlbumId == albumId)
            .ToAlbumFileResponses()
            .ToListAsync();
    }

    /// <summary>
    /// Returns a single album by ID. When <paramref name="tree"/> is <see langword="true"/>,
    /// the <c>Children</c> hierarchy is populated recursively.
    /// Translates text fields when the request is unauthenticated and the language is not <c>vi</c>.
    /// </summary>
    public async Task<AlbumResponse?> GetAlbumByIdAsync(string id, bool tree = false)
    {
        var album = await _dbContext.Albums
            .ToAlbumResponses()
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);

        if (album is null) return null;

        var ctx = _httpContext.HttpContext!;
        var lang = ctx.GetLanguage();
        bool translate = lang != "vi";

        if (translate)
        {
            var t = await _translationService.GetAsync(EntityType.Album, id, lang);
            ApplyTranslations(album, t);
        }

        if (tree)
        {
            var lookup = await BuildLookupAsync(translate ? lang : null);
            PopulateChildren(album, lookup);
        }

        return album;
    }

    /// <summary>
    /// Returns a single album by slug. When <paramref name="tree"/> is <see langword="true"/>,
    /// the <c>Children</c> hierarchy is populated recursively.
    /// Translates text fields when the request is unauthenticated and the language is not <c>vi</c>.
    /// </summary>
    public async Task<AlbumResponse?> GetAlbumBySlugAsync(string slug, bool tree = false)
    {
        var album = await _dbContext.Albums
            .ToAlbumResponses()
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Slug == slug);

        if (album is null) return null;

        var ctx = _httpContext.HttpContext!;
        var lang = ctx.GetLanguage();
        bool translate = lang != "vi";

        if (translate)
        {
            var t = await _translationService.GetAsync(EntityType.Album, album.Id, lang);
            ApplyTranslations(album, t);
        }

        if (tree)
        {
            var lookup = await BuildLookupAsync(translate ? lang : null);
            PopulateChildren(album, lookup);
        }

        return album;
    }

    /// <summary>
    /// Returns all root-level albums (those with no parent).
    /// When <paramref name="tree"/> is <see langword="true"/>, children are populated recursively.
    /// Translates text fields when the request is unauthenticated and the language is not <c>vi</c>.
    /// </summary>
    public async Task<IReadOnlyCollection<AlbumResponse>> GetAlbumParentAsync(bool tree = false)
    {
        var ctx = _httpContext.HttpContext!;
        var lang = ctx.GetLanguage();
        bool translate = lang != "vi";

        if (!tree)
        {
            var list = await _dbContext.Albums
                .Where(a => a.AlbumId == null)
                .ToAlbumResponses()
                .AsNoTracking()
                .ToListAsync();

            if (translate && list.Count > 0)
            {
                var batch = await _translationService.GetBatchAsync(
                    EntityType.Album, list.Select(a => a.Id), lang);
                foreach (var a in list)
                    if (batch.TryGetValue(a.Id, out var t)) ApplyTranslations(a, t);
            }

            return list;
        }

        var lookup = await BuildLookupAsync(translate ? lang : null);
        var parents = lookup[null].ToList();

        foreach (var parent in parents)
            PopulateChildren(parent, lookup);

        return parents;
    }

    /// <summary>
    /// Returns direct children of the specified parent album.
    /// When <paramref name="tree"/> is <see langword="true"/>, their descendants are also populated.
    /// Translates text fields when the request is unauthenticated and the language is not <c>vi</c>.
    /// </summary>
    public async Task<IReadOnlyCollection<AlbumResponse>> GetAlbumChildrenAsync(
        string parentId, bool tree = false)
    {
        var ctx = _httpContext.HttpContext!;
        var lang = ctx.GetLanguage();
        bool translate = lang != "vi";

        if (!tree)
        {
            var list = await _dbContext.Albums
                .Where(a => a.AlbumId == parentId)
                .ToAlbumResponses()
                .AsNoTracking()
                .ToListAsync();

            if (translate && list.Count > 0)
            {
                var batch = await _translationService.GetBatchAsync(
                    EntityType.Album, list.Select(a => a.Id), lang);
                foreach (var a in list)
                    if (batch.TryGetValue(a.Id, out var t)) ApplyTranslations(a, t);
            }

            return list;
        }

        var lookup = await BuildLookupAsync(translate ? lang : null);
        var children = lookup[parentId].ToList();

        foreach (var child in children)
            PopulateChildren(child, lookup);

        return children;
    }

    /// <summary>
    /// Returns all albums as a flat list, ordered by creation date descending.
    /// Translates text fields when the request is unauthenticated and the language is not <c>vi</c>.
    /// </summary>
    public async Task<IReadOnlyCollection<AlbumResponse>> GetAllAsync()
    {
        var list = await _dbContext.Albums
            .OrderByDescending(a => a.CreatedDate)
            .ToAlbumResponses()
            .AsNoTracking()
            .ToListAsync();

        var ctx = _httpContext.HttpContext!;
        var lang = ctx.GetLanguage();
        if (lang != "vi" && list.Count > 0)
        {
            var batch = await _translationService.GetBatchAsync(
                EntityType.Album, list.Select(a => a.Id), lang);
            foreach (var album in list)
                if (batch.TryGetValue(album.Id, out var t)) ApplyTranslations(album, t);
        }

        return list;
    }

    /// <summary>
    /// Returns the full album hierarchy as a list of root albums with all descendants nested.
    /// Translates text fields when the request is unauthenticated and the language is not <c>vi</c>.
    /// </summary>
    public async Task<IReadOnlyCollection<AlbumResponse>> BuildAlbumTreeAsync()
    {
        var ctx = _httpContext.HttpContext!;
        var lang = ctx.GetLanguage();
        bool translate = lang != "vi";

        var lookup = await BuildLookupAsync(translate ? lang : null);
        var roots = lookup[null].ToList();

        foreach (var root in roots)
            PopulateChildren(root, lookup);

        return roots;
    }

    // ── Private helpers ───────────────────────────────────────────────────

    /// <summary>
    /// Loads all albums in a single query and groups them by parent ID.
    /// Applies translations when <paramref name="langToTranslate"/> is provided.
    /// The <see langword="null"/> key contains root-level albums.
    /// </summary>
    private async Task<ILookup<string?, AlbumResponse>> BuildLookupAsync(string? langToTranslate = null)
    {
        var all = await _dbContext.Albums
            .ToAlbumResponses()
            .AsNoTracking()
            .ToListAsync();

        if (langToTranslate is not null)
        {
            var batch = await _translationService.GetBatchAsync(
                EntityType.Album, all.Select(a => a.Id), langToTranslate);
            foreach (var album in all)
                if (batch.TryGetValue(album.Id, out var t)) ApplyTranslations(album, t);
        }

        return all.ToLookup<AlbumResponse, string?>(a => a.AlbumId);
    }

    /// <summary>
    /// Recursively attaches child albums to <paramref name="album"/> using a pre-built lookup.
    /// Terminates when there are no children for the current node.
    /// </summary>
    private static void PopulateChildren(
        AlbumResponse album,
        ILookup<string?, AlbumResponse> lookup)
    {
        var children = lookup[album.Id].ToList();
        if (children.Count == 0) return;

        album.Children = children;

        foreach (var child in children)
            PopulateChildren(child, lookup);
    }

    private static void ApplyTranslations(AlbumResponse r, Dictionary<string, string> t)
    {
        if (t.TryGetValue("title", out var v)) r.Title = v;
        if (t.TryGetValue("description", out v)) r.Description = v;
    }
}

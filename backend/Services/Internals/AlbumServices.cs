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

    /// <summary>Initialises the service with required dependencies.</summary>
    public AlbumServices(ILogger<AlbumServices> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
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
    /// Deletes one or more albums by ID. Cascades to child albums and album-file records
    /// as configured in the database.
    /// </summary>
    /// <param name="ids">IDs of albums to delete.</param>
    public async Task DeleteAlbumAsync(List<string> ids)
    {
        await _dbContext.Albums
            .Where(a => ids.Contains(a.Id))
            .ExecuteDeleteAsync();
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
    /// </summary>
    public async Task<AlbumResponse?> GetAlbumByIdAsync(string id, bool tree = false)
    {
        var album = await _dbContext.Albums
            .ToAlbumResponses()
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);

        if (album is not null && tree)
        {
            var lookup = await BuildLookupAsync();
            PopulateChildren(album, lookup);
        }

        return album;
    }

    /// <summary>
    /// Returns a single album by slug. When <paramref name="tree"/> is <see langword="true"/>,
    /// the <c>Children</c> hierarchy is populated recursively.
    /// </summary>
    public async Task<AlbumResponse?> GetAlbumBySlugAsync(string slug, bool tree = false)
    {
        var album = await _dbContext.Albums
            .ToAlbumResponses()
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Slug == slug);

        if (album is not null && tree)
        {
            var lookup = await BuildLookupAsync();
            PopulateChildren(album, lookup);
        }

        return album;
    }

    /// <summary>
    /// Returns all root-level albums (those with no parent).
    /// When <paramref name="tree"/> is <see langword="true"/>, children are populated recursively.
    /// </summary>
    public async Task<IReadOnlyCollection<AlbumResponse>> GetAlbumParentAsync(bool tree = false)
    {
        if (!tree)
        {
            return await _dbContext.Albums
                .Where(a => a.AlbumId == null)
                .ToAlbumResponses()
                .AsNoTracking()
                .ToListAsync();
        }

        var lookup = await BuildLookupAsync();
        var parents = lookup[null].ToList();

        foreach (var parent in parents)
            PopulateChildren(parent, lookup);

        return parents;
    }

    /// <summary>
    /// Returns direct children of the specified parent album.
    /// When <paramref name="tree"/> is <see langword="true"/>, their descendants are also populated.
    /// </summary>
    public async Task<IReadOnlyCollection<AlbumResponse>> GetAlbumChildrenAsync(
        string parentId, bool tree = false)
    {
        if (!tree)
        {
            return await _dbContext.Albums
                .Where(a => a.AlbumId == parentId)
                .ToAlbumResponses()
                .AsNoTracking()
                .ToListAsync();
        }

        var lookup = await BuildLookupAsync();
        var children = lookup[parentId].ToList();

        foreach (var child in children)
            PopulateChildren(child, lookup);

        return children;
    }

    /// <summary>
    /// Returns the full album hierarchy as a list of root albums with all descendants nested.
    /// </summary>
    public async Task<IReadOnlyCollection<AlbumResponse>> BuildAlbumTreeAsync()
    {
        var lookup = await BuildLookupAsync();
        var roots = lookup[null].ToList();

        foreach (var root in roots)
            PopulateChildren(root, lookup);

        return roots;
    }

    // ── Private helpers ───────────────────────────────────────────────────

    /// <summary>
    /// Loads all albums in a single query and groups them by parent ID.
    /// The <see langword="null"/> key contains root-level albums.
    /// </summary>
    private async Task<ILookup<string?, AlbumResponse>> BuildLookupAsync()
    {
        var all = await _dbContext.Albums
            .ToAlbumResponses()
            .AsNoTracking()
            .ToListAsync();

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
}

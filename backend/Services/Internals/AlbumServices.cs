using backend.Data;
using backend.Exceptions;
using backend.Extensions;
using backend.ViewModels.Albums.Requests;
using backend.ViewModels.Albums.Responses;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Internals;

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
        request.Name = request.Name.ToLowerInvariant();

        var existingAlbum = await _dbContext.Albums
            .FirstOrDefaultAsync(a => a.Name == request.Name);

        if (existingAlbum is not null)
            throw new BadRequestException();

        var album = request.ToAlbum();
        album.Slug = request.Name.ToSlug();
        album.CreatedDate = DateTimeOffset.UtcNow;
        album.UpdatedDate = DateTimeOffset.UtcNow;
        album.CountImageRef = 0;

        _dbContext.Albums.Add(album);
        await _dbContext.SaveChangesAsync();

        return album.ToAlbumResponse();
    }

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

    public async Task<IReadOnlyCollection<AlbumResponse>> BuildAlbumTreeAsync()
    {
        var lookup = await BuildLookupAsync();
        var roots = lookup[null].ToList();

        foreach (var root in roots)
            PopulateChildren(root, lookup);

        return roots;
    }

    private async Task<ILookup<string?, AlbumResponse>> BuildLookupAsync()
    {
        var all = await _dbContext.Albums
            .ToAlbumResponses()
            .AsNoTracking()
            .ToListAsync();

        return all.ToLookup<AlbumResponse, string?>(a => a.AlbumId);
    }

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

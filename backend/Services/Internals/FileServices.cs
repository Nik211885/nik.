using backend.Data;
using backend.Exceptions;
using backend.ViewModels;
using backend.ViewModels.Files.Requests;
using backend.ViewModels.Files.Responses;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Internals;

/// <summary>
/// Provides CRUD operations for file metadata records.
/// Binary content is stored on Cloudinary; only metadata is persisted here.
/// </summary>
public class FileServices
{
    private readonly ILogger<FileServices> _logger;
    private readonly ApplicationDbContext _dbContext;

    /// <summary>Initialises the service with required dependencies.</summary>
    public FileServices(ILogger<FileServices> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    /// <summary>
    /// Registers a new file record. The Cloudinary URL must be unique across all files.
    /// </summary>
    /// <param name="request">File metadata including the Cloudinary URL.</param>
    /// <returns>The created file response.</returns>
    /// <exception cref="BadRequestException">Thrown when a file with the same URL already exists.</exception>
    public async Task<FileResponse> CreateFileAsync(CreateFileRequest request)
    {
        var existing = await _dbContext.files
            .AsNoTracking()
            .AnyAsync(f => f.Url == request.Url);

        if (existing)
            throw new BadRequestException(ApplicationMessage.ExitsCode);

        var file = request.ToFile();

        _dbContext.files.Add(file);
        await _dbContext.SaveChangesAsync();

        return file.ToFileResponse();
    }

    /// <summary>
    /// Updates a file's display metadata (name, title, description).
    /// The Cloudinary URL cannot be changed after creation.
    /// </summary>
    /// <param name="request">Update payload.</param>
    /// <returns>The updated file response.</returns>
    /// <exception cref="NotFoundException">Thrown when the file ID does not exist.</exception>
    public async Task<FileResponse> UpdateFileAsync(UpdateFileRequest request)
    {
        var file = await _dbContext.files
            .FirstOrDefaultAsync(f => f.Id == request.Id)
            ?? throw new NotFoundException();

        request.ApplyTo(file);

        _dbContext.Update(file);
        await _dbContext.SaveChangesAsync();

        return file.ToFileResponse();
    }

    /// <summary>
    /// Deletes one or more file records by ID.
    /// Does not remove the binary content from Cloudinary.
    /// </summary>
    /// <param name="ids">IDs of files to delete.</param>
    public async Task DeleteFileAsync(List<string> ids)
    {
        await _dbContext.files
            .Where(f => ids.Contains(f.Id))
            .ExecuteDeleteAsync();
    }

    /// <summary>
    /// Returns a single file by ID, or <see langword="null"/> if not found.
    /// </summary>
    /// <param name="id">File ID.</param>
    public async Task<FileResponse?> GetFileByIdAsync(string id)
    {
        return await _dbContext.files
            .AsNoTracking()
            .Where(f => f.Id == id)
            .ToFileResponses()
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Returns a paginated list of all files ordered by ID descending (newest first).
    /// </summary>
    /// <param name="request">Pagination parameters.</param>
    public async Task<PaginationItem<FileResponse>> GetPaginationFileAsync(PaginationRequest request)
    {
        return await _dbContext.files
            .AsNoTracking()
            .OrderByDescending(f => f.Id)
            .ToFileResponses()
            .PaginationItemAsync(request);
    }
}

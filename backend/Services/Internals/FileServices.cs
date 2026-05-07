using backend.Data;
using backend.Exceptions;
using backend.ViewModels;
using backend.ViewModels.Files.Requests;
using backend.ViewModels.Files.Responses;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Internals;

public class FileServices
{
    private readonly ILogger<FileServices> _logger;
    private readonly ApplicationDbContext _dbContext;

    public FileServices(ILogger<FileServices> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

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

    public async Task DeleteFileAsync(List<string> ids)
    {
        await _dbContext.files
            .Where(f => ids.Contains(f.Id))
            .ExecuteDeleteAsync();
    }

    public async Task<FileResponse?> GetFileByIdAsync(string id)
    {
        return await _dbContext.files
            .AsNoTracking()
            .Where(f => f.Id == id)
            .ToFileResponses()
            .FirstOrDefaultAsync();
    }

    public async Task<PaginationItem<FileResponse>> GetPaginationFileAsync(PaginationRequest request)
    {
        return await _dbContext.files
            .AsNoTracking()
            .OrderByDescending(f => f.Id)
            .ToFileResponses()
            .PaginationItemAsync(request);
    }
}

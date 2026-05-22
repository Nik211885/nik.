using backend.Data;
using backend.Entities;
using backend.Exceptions;
using backend.Extensions;
using backend.Services.Extends;
using backend.ViewModels;
using backend.ViewModels.WallMessages.Requests;
using backend.ViewModels.WallMessages.Responses;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Internals;

/// <summary>Business logic for wall message submissions and moderation.</summary>
public class WallMessageServices
{
    private readonly ILogger<WallMessageServices> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly ClaudeModeration _moderation;

    /// <summary>Initialises the service with required dependencies.</summary>
    public WallMessageServices(
        ILogger<WallMessageServices> logger,
        ApplicationDbContext dbContext,
        ClaudeModeration moderation)
    {
        _logger = logger;
        _dbContext = dbContext;
        _moderation = moderation;
    }

    /// <summary>
    /// Creates a new wall message and runs Claude moderation to set initial status.
    /// </summary>
    /// <param name="request">Submission payload.</param>
    /// <param name="ipAddress">Submitter IP for audit trail.</param>
    /// <returns>The saved message with its moderation status.</returns>
    public async Task<WallMessageResponse> CreateAsync(CreateWallMessageRequest request, string? ipAddress)
    {
        var status = await _moderation.ModerateAsync(request.Name, request.Message, request.Source);

        var entity = new WallMessage
        {
            Name        = request.Name.Trim(),
            Message     = request.Message.Trim(),
            Source      = string.IsNullOrWhiteSpace(request.Source) ? null : request.Source.Trim(),
            Status      = status,
            IpAddress   = ipAddress,
            CreatedDate = DateTimeOffset.UtcNow,
        };

        _dbContext.WallMessages.Add(entity);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("WallMessage {Id} from {Name} → {Status}", entity.Id, entity.Name, status);

        return ToResponse(entity);
    }

    /// <summary>Returns all approved wall messages, sorted by most-reacted then newest.</summary>
    public async Task<List<WallMessageResponse>> GetApprovedAsync()
    {
        return await _dbContext.WallMessages
            .AsNoTracking()
            .Where(x => x.Status == WallMessageStatus.Approved)
            .OrderByDescending(x => x.ReactionCount)
            .ThenByDescending(x => x.CreatedDate)
            .Select(x => new WallMessageResponse
            {
                Id            = x.Id,
                Name          = x.Name,
                Message       = x.Message,
                Source        = x.Source,
                Status        = x.Status.ToString(),
                CreatedDate   = x.CreatedDate,
                ReactionCount = x.ReactionCount,
            })
            .ToListAsync();
    }

    /// <summary>Toggles a reaction from a device on a wall message.</summary>
    /// <param name="id">Wall message ID.</param>
    /// <param name="deviceId">Visitor device UUID from localStorage.</param>
    /// <returns>Updated reaction count and whether the device is now reacted.</returns>
    /// <exception cref="NotFoundException">Thrown when no approved message with the given ID exists.</exception>
    public async Task<ReactWallMessageResponse> ReactAsync(string id, string deviceId)
    {
        var message = await _dbContext.WallMessages
            .FirstOrDefaultAsync(x => x.Id == id && x.Status == WallMessageStatus.Approved)
            ?? throw new NotFoundException();

        var existing = await _dbContext.WallMessageReactions
            .FirstOrDefaultAsync(x => x.WallMessageId == id && x.DeviceId == deviceId);

        if (existing is not null)
        {
            _dbContext.WallMessageReactions.Remove(existing);
            message.ReactionCount = Math.Max(0, message.ReactionCount - 1);
            await _dbContext.SaveChangesAsync();
            return new ReactWallMessageResponse { ReactionCount = message.ReactionCount, Reacted = false };
        }

        _dbContext.WallMessageReactions.Add(new WallMessageReaction
        {
            WallMessageId = id,
            DeviceId      = deviceId,
            CreatedDate   = DateTimeOffset.UtcNow,
        });
        message.ReactionCount++;
        await _dbContext.SaveChangesAsync();
        return new ReactWallMessageResponse { ReactionCount = message.ReactionCount, Reacted = true };
    }

    /// <summary>Returns a paginated list of messages for admin, optionally filtered by status.</summary>
    /// <param name="request">Pagination parameters.</param>
    /// <param name="status">Optional status filter.</param>
    public async Task<PaginationItem<AdminWallMessageResponse>> GetPaginationAsync(
        PaginationRequest request, WallMessageStatus? status = null)
    {
        var query = _dbContext.WallMessages.AsNoTracking();

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        return await query
            .OrderByDescending(x => x.CreatedDate)
            .Select(x => new AdminWallMessageResponse
            {
                Id          = x.Id,
                Name        = x.Name,
                Message     = x.Message,
                Source      = x.Source,
                Status      = x.Status.ToString(),
                IpAddress   = x.IpAddress,
                CreatedDate = x.CreatedDate,
            })
            .PaginationItemAsync(request);
    }

    /// <summary>Updates the moderation status of a wall message.</summary>
    /// <param name="id">Message ID.</param>
    /// <param name="status">New status to apply.</param>
    /// <exception cref="NotFoundException">Thrown when no message with the given ID exists.</exception>
    public async Task<AdminWallMessageResponse> UpdateStatusAsync(string id, WallMessageStatus status)
    {
        var entity = await _dbContext.WallMessages.FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException();

        entity.Status = status;
        await _dbContext.SaveChangesAsync();

        return new AdminWallMessageResponse
        {
            Id          = entity.Id,
            Name        = entity.Name,
            Message     = entity.Message,
            Status      = entity.Status.ToString(),
            IpAddress   = entity.IpAddress,
            CreatedDate = entity.CreatedDate,
        };
    }

    /// <summary>Updates the moderation status of multiple wall messages in one operation.</summary>
    /// <param name="ids">IDs of messages to update.</param>
    /// <param name="status">Status to apply to all selected messages.</param>
    public async Task BulkUpdateStatusAsync(List<string> ids, WallMessageStatus status)
    {
        await _dbContext.WallMessages
            .Where(x => ids.Contains(x.Id))
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.Status, status));
    }

    /// <summary>Deletes one or more wall messages by ID.</summary>
    /// <param name="ids">IDs of messages to delete.</param>
    public async Task DeleteAsync(List<string> ids)
    {
        await _dbContext.WallMessages
            .Where(x => ids.Contains(x.Id))
            .ExecuteDeleteAsync();
    }

    private static WallMessageResponse ToResponse(WallMessage x) => new()
    {
        Id            = x.Id,
        Name          = x.Name,
        Message       = x.Message,
        Source        = x.Source,
        Status        = x.Status.ToString(),
        CreatedDate   = x.CreatedDate,
        ReactionCount = x.ReactionCount,
    };
}

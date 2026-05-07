using System.Net.Mime;
using backend.Data;
using backend.Entities;
using backend.Exceptions;
using backend.Services;
using backend.ViewModels.Configs.Requests;
using backend.ViewModels.Configs.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Internals;

/// <summary>Provides CRUD operations for system configuration key-value entries.</summary>
public class SysConfigServices
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<SysConfigServices> _logger;

    /// <summary>Initialises the service with required dependencies.</summary>
    public SysConfigServices(ApplicationDbContext dbContext, ILogger<SysConfigServices> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>Returns all configuration entries.</summary>
    public async Task<IReadOnlyCollection<ConfigResponse>> GetConfigsAsync()
    {
        var result = await _dbContext
            .SysConfigs.Select(x => new ConfigResponse()
            {
                Id = x.Id,
                Key = x.Key,
                Value = x.Value
            })
            .AsNoTracking()
            .ToListAsync();
        return result;
    }

    /// <summary>
    /// Creates a new configuration entry. The key is normalised to lowercase and must be unique.
    /// </summary>
    /// <param name="request">Configuration payload.</param>
    /// <exception cref="BadRequestException">Thrown when the key already exists.</exception>
    public async Task CreateConfigAsync(CreateConfigRequest request)
    {
        request.Key = request.Key.ToLowerInvariant();
        var configExitsKey = await _dbContext.SysConfigs
            .Where(x => x.Key == request.Key).AsNoTracking().AnyAsync();
        if (configExitsKey)
        {
            throw new BadRequestException(ApplicationMessage.ExitsCode);
        }

        _dbContext.SysConfigs.Add(new SysConfig() { Key = request.Key, Value = request.Value });
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Updates the key and value of a specific configuration entry.
    /// The new key must not be used by a different entry.
    /// </summary>
    /// <param name="id">ID of the entry to update.</param>
    /// <param name="request">New key and value.</param>
    /// <exception cref="NotFoundException">Thrown when the entry ID does not exist.</exception>
    /// <exception cref="BadRequestException">Thrown when the new key is already used by another entry.</exception>
    public async Task UpdateConfigSpecificByIdAsync(string id, CreateConfigRequest request)
    {
        request.Key = request.Key.ToLowerInvariant();
        var configById = await _dbContext.SysConfigs.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (configById is null)
        {
            throw new NotFoundException(ApplicationMessage.NotFound);
        }

        var configKeyExits = await _dbContext.SysConfigs
            .Where(x => x.Key == request.Key).AsNoTracking().FirstOrDefaultAsync();
        if (configKeyExits is not null && configById.Id != configKeyExits.Id)
        {
            throw new BadRequestException(ApplicationMessage.ExitsCode);
        }

        configById.Key = request.Key;
        configById.Value = request.Value;

        _dbContext.SysConfigs.Update(configById);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>Returns a single configuration entry by ID, or <see langword="null"/> if not found.</summary>
    /// <param name="id">Configuration entry ID.</param>
    public async Task<ConfigResponse?> GetConfigByIdAsync(string id)
    {
        var config = await _dbContext.SysConfigs.Where(x => x.Id == id)
            .Select(x=>new ConfigResponse()
            {
                Id = x.Id,
                Key = x.Key,
                Value = x.Value
            })
            .FirstOrDefaultAsync();
        return config;
    }

    /// <summary>Deletes one or more configuration entries by ID.</summary>
    /// <param name="ids">IDs of entries to delete.</param>
    public async Task DeleteConfigByIdsAsync(List<string> ids)
    {
        await _dbContext.SysConfigs.Where(x => ids.Contains(x.Id))
            .ExecuteDeleteAsync();
    }
}

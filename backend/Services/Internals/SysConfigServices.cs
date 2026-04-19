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

public class SysConfigServices
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<SysConfigServices> _logger;

    public SysConfigServices(ApplicationDbContext dbContext, ILogger<SysConfigServices> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

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
    
    public async Task DeleteConfigByIdsAsync(List<string> ids)
    {
        await _dbContext.SysConfigs.Where(x => ids.Contains(x.Id))
            .ExecuteDeleteAsync();
    }
    
}

using backend.Data;
using backend.Entities;
using backend.Exceptions;
using backend.ViewModels.Languages.Requests;
using backend.ViewModels.Languages.Responses;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class LanguageServices
{
    private readonly ILogger<LanguageServices> _logger;
    private readonly ApplicationDbContext _dbContext;

    public LanguageServices(ILogger<LanguageServices> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<TranslateLanguageResponse?> GetTranslates(string lang)
    {
        Language? language = await _dbContext.Languages
            .Where(x => x.Code == lang.ToLower()).FirstOrDefaultAsync();
        if (language is null)
        {
            return null;
        }

        List<TranslateResponse> translates = await _dbContext.Translates
            .Where(x => x.LanguageId == language.Id)
            .Include(x => x.CodeLanguage)
            .Select(x => new TranslateResponse() { Code = x.CodeLanguage.Code, Value = x.Value })
            .ToListAsync();

        var response = new TranslateLanguageResponse() { Language = language.Code, Translations = translates };
        return response;
    }

    public async Task CreateCodeLanguageAsync(List<string> codes)
    {
        var normalizedCodes = codes
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim().ToLower())
            .Distinct()
            .ToList();

        var existsCodes = await _dbContext.CodeLanguages
            .Where(x => normalizedCodes.Contains(x.Code))
            .Select(x => x.Code)
            .ToListAsync();

        var notExistsCodes = normalizedCodes
            .Except(existsCodes)
            .ToList();

        if (notExistsCodes.Any())
        {
            _dbContext.CodeLanguages.AddRange(
                notExistsCodes.Select(x => new CodeLanguage
                {
                    Code = x
                })
            );

            await _dbContext.SaveChangesAsync(); 
        }
    }

    public async Task UpdateCodeLanguageAsync(UpdateCodeLanguage request)
    {
        // find by id
        var codeLanguage = await _dbContext.CodeLanguages.Where(x=>x.Id == request.Id).FirstOrDefaultAsync();
        if (codeLanguage is null)
        {
            throw new NotFoundException();
        }
        // find code language has exit in database
        var codeLanguageExits =
            await _dbContext.CodeLanguages.Where(x => x.Code.ToLower() == request.Code).FirstOrDefaultAsync();
        if (codeLanguageExits is not null && codeLanguageExits.Id != codeLanguage.Id)
        {
            throw new BadRequestException(ApplicationMessage.ExitsCode);
        }

        codeLanguage.Code = request.Code.ToLower();
        _dbContext.CodeLanguages.Update(codeLanguage);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteCodeLanguageAsync(List<string> ids)
    {
        await _dbContext.CodeLanguages
            .Where(x=>ids.Contains(x.Id))
            .ExecuteDeleteAsync();
    }

    public async Task CreateLanguageAsync(LanguageRequest request)
    {
        var languageExitCode = await _dbContext.Languages
            .Where(x => x.Code == request.Code.ToLower()).FirstOrDefaultAsync();
        if (languageExitCode is not null)
        {
            throw new BadRequestException(ApplicationMessage.ExitsCode);
        }

        _dbContext.Languages.Add(new Language { Code = request.Code.ToLower(), Name = request.Name });
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateLanguageAsync(string id, LanguageRequest request)
    {
        var languageUpdate = await _dbContext.Languages.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (languageUpdate is null)
        {
            throw new NotFoundException();
        }
        var languageExitCode = await _dbContext.Languages
            .Where(x => x.Code == request.Code.ToLower()).FirstOrDefaultAsync();
        if (languageExitCode is not null && languageExitCode.Id != languageUpdate.Id)
        {
            throw new BadRequestException(ApplicationMessage.ExitsCode);
        }

        languageUpdate.Code = request.Code.ToLower();
        languageUpdate.Name = request.Name;
        _dbContext.Languages.Update(languageUpdate);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteLanguageAsync(string id)
    {
        await _dbContext.Languages
            .Where(x => x.Id == id)
            .ExecuteDeleteAsync();
    }

    public async Task<IReadOnlyCollection<LanguageResponse>> GetListLanguageAsync()
    {
        var result =await _dbContext.Languages
            .Select(x=>new LanguageResponse()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name
            }).ToListAsync();
        return result;
    }
}

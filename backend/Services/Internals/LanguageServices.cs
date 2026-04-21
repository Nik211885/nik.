using backend.Data;
using backend.Entities;
using backend.Exceptions;
using backend.Services;
using backend.ViewModels.Languages.Requests;
using backend.ViewModels.Languages.Responses;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Internals;

public class LanguageServices
{
    private readonly ILogger<LanguageServices> _logger;
    private readonly ApplicationDbContext _dbContext;

    public LanguageServices(ILogger<LanguageServices> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<TranslateLanguageResponse?> GetTranslatesAsync(string lang)
    {
        Language? language = await _dbContext.Languages
            .Where(x => x.Code.Equals(lang, StringComparison.InvariantCultureIgnoreCase)).AsNoTracking().FirstOrDefaultAsync();
        if (language is null)
        {
            return null;
        }

        List<TranslateResponse> translates = await _dbContext.Translates
            .Where(x => x.LanguageId == language.Id)
            .Include(x => x.CodeLanguage)
            .Select(x => new TranslateResponse() { Code = x.CodeLanguage.Code, Value = x.Value })
            .AsNoTracking()
            .ToListAsync();

        var response = new TranslateLanguageResponse() { Language = language.Code, Translations = translates };
        return response;
    }

    public async Task CreateCodeLanguageAsync(List<string> codes)
    {
        var normalizedCodes = codes
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim().ToLowerInvariant())
            .Distinct()
            .ToList();

        var existsCodes = await _dbContext.CodeLanguages
            .Where(x => normalizedCodes.Contains(x.Code))
            .Select(x => x.Code)
            .AsNoTracking()
            .ToListAsync();

        var notExistsCodes = normalizedCodes
            .Except(existsCodes)
            .ToList();

        if (notExistsCodes.Count != 0)
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
        request.Code = request.Code.ToLowerInvariant();
        var codeLanguage = await _dbContext.CodeLanguages.Where(x => x.Id == request.Id).FirstOrDefaultAsync() 
        ?? throw new NotFoundException();
        // find code language has exit in database
        var codeLanguageExits =
            await _dbContext.CodeLanguages.Where(x => x.Code == request.Code)
                .AsNoTracking().FirstOrDefaultAsync();
        if (codeLanguageExits is not null && codeLanguageExits.Id != codeLanguage.Id)
        {
            throw new BadRequestException(ApplicationMessage.ExitsCode);
        }

        codeLanguage.Code = request.Code;
        _dbContext.CodeLanguages.Update(codeLanguage);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteCodeLanguageAsync(List<string> ids)
    {
        _ = await _dbContext.CodeLanguages
            .Where(x => ids.Contains(x.Id))
            .ExecuteDeleteAsync();
    }

    public async Task<IReadOnlyCollection<CodeLanguageResponse>> GetCodeLanguagesAsync()
    {
        var result = await _dbContext.CodeLanguages
            .Select(x=> new CodeLanguageResponse(){Id = x.Id,  Code = x.Code})
            .AsNoTracking()
            .ToListAsync();
        return result;
    }

    public async Task CreateLanguageAsync(LanguageRequest request)
    {
        request.Code = request.Code.ToLowerInvariant();
        var languageExitCode = await _dbContext.Languages
            .Where(x => x.Code == request.Code).AsNoTracking().FirstOrDefaultAsync();
        if (languageExitCode is not null)
        {
            throw new BadRequestException(ApplicationMessage.ExitsCode);
        }

        _dbContext.Languages.Add(new Language { Code = request.Code, Name = request.Name });
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateLanguageAsync(string id, LanguageRequest request)
    {
        request.Code = request.Code.ToLowerInvariant();
        var languageUpdate = await _dbContext.Languages.Where(x => x.Id == id).FirstOrDefaultAsync() 
        ?? throw new NotFoundException();
        var languageExitCode = await _dbContext.Languages
            .Where(x => x.Code == request.Code).AsNoTracking().FirstOrDefaultAsync();
        if (languageExitCode is not null && languageExitCode.Id != languageUpdate.Id)
        {
            throw new BadRequestException(ApplicationMessage.ExitsCode);
        }

        languageUpdate.Code = request.Code;
        languageUpdate.Name = request.Name;
        _dbContext.Languages.Update(languageUpdate);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteLanguageAsync(string id)
    {
        _ = await _dbContext.Languages
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
            }).AsNoTracking().ToListAsync();
        return result;
    }

    public async Task DefinedCodeLanguageAsync(TranslateCodeLanguageRequest request)
    {
        request.CodeDefined = request.CodeDefined.ToLowerInvariant();
        var codeLanguage = await _dbContext.CodeLanguages.Where(x => x.Code == request.CodeDefined)
            .AsNoTracking().FirstOrDefaultAsync() ?? throw new NotFoundException();
        var translateGroupByCode = request.Translates
            .DistinctBy(x => x.Language).ToList();
        var languages = await _dbContext.Languages
            .Where(x=>translateGroupByCode.Select(y=>y.Language).Contains(x.Code))
            .AsNoTracking()
            .ToListAsync();

        var translateInstances = new List<Translate>();
        await _dbContext.Translates.Where(x => x.CodeId == codeLanguage.Id)
            .Where(x => translateGroupByCode.Select(y => y.Language).Contains(x.LanguageId))
            .ExecuteDeleteAsync();
        foreach (var lang in languages)
        {
            var req = translateGroupByCode.FirstOrDefault(x => x.Language == lang.Code);
            if (req is null)
            {
                continue;
            }

            translateInstances.Add(new Translate()
            {
                CodeId = codeLanguage.Id,
                LanguageId = lang.Id,
                Value = req.Value
            });
        }
        _dbContext.Translates.AddRange(translateInstances);
        await _dbContext.SaveChangesAsync();
    }
}

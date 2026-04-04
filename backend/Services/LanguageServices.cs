using backend.Data;
using backend.Entities;
using backend.ViewModels.Responses;
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
}

using backend.Data;
using backend.Entities;
using backend.Exceptions;
using backend.Services;
using backend.ViewModels.Languages.Requests;
using backend.ViewModels.Languages.Responses;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Internals;

/// <summary>
/// Provides operations for managing i18n languages, translation code keys,
/// and per-language translation values.
/// </summary>
public class LanguageServices
{
    private readonly ILogger<LanguageServices> _logger;
    private readonly ApplicationDbContext _dbContext;

    /// <summary>Initialises the service with required dependencies.</summary>
    public LanguageServices(ILogger<LanguageServices> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    /// <summary>
    /// Returns all translations for the specified language code, or <see langword="null"/>
    /// when the language is not registered.
    /// </summary>
    /// <param name="lang">BCP-47 language code (e.g. <c>vi</c>, <c>en</c>).</param>
    public async Task<TranslateLanguageResponse?> GetTranslatesAsync(string lang)
    {
        var langLower = lang.ToLowerInvariant();
        Language? language = await _dbContext.Languages
            .Where(x => x.Code == langLower).AsNoTracking().FirstOrDefaultAsync();
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

    /// <summary>
    /// Registers new translation code keys. Already-existing codes are silently skipped.
    /// Codes are normalised to lowercase and deduplicated before insert.
    /// </summary>
    /// <param name="codes">List of raw code strings to register.</param>
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

    /// <summary>
    /// Updates an existing translation code key. The new code must be unique.
    /// </summary>
    /// <param name="request">Update payload containing the entry ID and new code value.</param>
    /// <exception cref="NotFoundException">Thrown when the code language ID does not exist.</exception>
    /// <exception cref="BadRequestException">Thrown when the new code is already used by another entry.</exception>
    public async Task UpdateCodeLanguageAsync(UpdateCodeLanguage request)
    {
        request.Code = request.Code.ToLowerInvariant();
        var codeLanguage = await _dbContext.CodeLanguages.Where(x => x.Id == request.Id).FirstOrDefaultAsync()
        ?? throw new NotFoundException();
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

    /// <summary>Deletes one or more translation code keys by ID.</summary>
    /// <param name="ids">IDs of code language entries to delete.</param>
    public async Task DeleteCodeLanguageAsync(List<string> ids)
    {
        _ = await _dbContext.CodeLanguages
            .Where(x => ids.Contains(x.Id))
            .ExecuteDeleteAsync();
    }

    /// <summary>Returns all registered translation code keys.</summary>
    public async Task<IReadOnlyCollection<CodeLanguageResponse>> GetCodeLanguagesAsync()
    {
        var result = await _dbContext.CodeLanguages
            .Select(x=> new CodeLanguageResponse(){Id = x.Id,  Code = x.Code})
            .AsNoTracking()
            .ToListAsync();
        return result;
    }

    /// <summary>
    /// Registers a new language. The code is normalised to lowercase and must be unique.
    /// </summary>
    /// <param name="request">Language creation payload.</param>
    /// <exception cref="BadRequestException">Thrown when the code already exists.</exception>
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

    /// <summary>
    /// Updates an existing language's code and name. The new code must be unique.
    /// </summary>
    /// <param name="id">ID of the language to update.</param>
    /// <param name="request">New code and name.</param>
    /// <exception cref="NotFoundException">Thrown when the language ID does not exist.</exception>
    /// <exception cref="BadRequestException">Thrown when the new code is already used by another language.</exception>
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

    /// <summary>Deletes a language by ID.</summary>
    /// <param name="id">ID of the language to delete.</param>
    public async Task DeleteLanguageAsync(string id)
    {
        _ = await _dbContext.Languages
            .Where(x => x.Id == id)
            .ExecuteDeleteAsync();
    }

    /// <summary>Returns all registered languages.</summary>
    public async Task<IReadOnlyCollection<LanguageResponse>> GetListLanguageAsync()
    {
        var result = await _dbContext.Languages
            .Select(x=>new LanguageResponse()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Icon = x.Icon
            }).AsNoTracking().ToListAsync();
        return result;
    }

    /// <summary>
    /// Upserts translation values for a given code key across multiple languages.
    /// Existing translations for the same code-language pairs are replaced.
    /// </summary>
    /// <param name="request">Code key and per-language value list.</param>
    /// <exception cref="NotFoundException">Thrown when the code key does not exist.</exception>
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

    // ─── Admin CRUD for Languages ─────────────────────────────────────────────

    /// <summary>
    /// Creates a new language and returns it. The code is normalised to lowercase and must be unique.
    /// </summary>
    /// <param name="request">Language creation payload.</param>
    /// <returns>The created <see cref="LanguageResponse"/>.</returns>
    /// <exception cref="BadRequestException">Thrown when the code already exists.</exception>
    public async Task<LanguageResponse> CreateLanguageAdminAsync(LanguageRequest request)
    {
        request.Code = request.Code.ToLowerInvariant();
        var exists = await _dbContext.Languages.AnyAsync(x => x.Code == request.Code);
        if (exists)
        {
            throw new BadRequestException(ApplicationMessage.ExitsCode);
        }

        var entity = new Language { Code = request.Code, Name = request.Name, Icon = request.Icon };
        _dbContext.Languages.Add(entity);
        await _dbContext.SaveChangesAsync();

        return new LanguageResponse { Id = entity.Id, Code = entity.Code, Name = entity.Name, Icon = entity.Icon };
    }

    /// <summary>
    /// Updates an existing language's code and name. The new code must be unique.
    /// </summary>
    /// <param name="request">Update payload with ID, new code and new name.</param>
    /// <returns>The updated <see cref="LanguageResponse"/>.</returns>
    /// <exception cref="NotFoundException">Thrown when the language ID does not exist.</exception>
    /// <exception cref="BadRequestException">Thrown when the new code is already used by another language.</exception>
    public async Task<LanguageResponse> UpdateLanguageAdminAsync(UpdateLanguageAdminRequest request)
    {
        request.Code = request.Code.ToLowerInvariant();
        var language = await _dbContext.Languages.FirstOrDefaultAsync(x => x.Id == request.Id)
            ?? throw new NotFoundException();

        var codeConflict = await _dbContext.Languages
            .Where(x => x.Code == request.Code)
            .AsNoTracking()
            .FirstOrDefaultAsync();
        if (codeConflict is not null && codeConflict.Id != language.Id)
        {
            throw new BadRequestException(ApplicationMessage.ExitsCode);
        }

        language.Code = request.Code;
        language.Name = request.Name;
        language.Icon = request.Icon;
        _dbContext.Languages.Update(language);
        await _dbContext.SaveChangesAsync();

        return new LanguageResponse { Id = language.Id, Code = language.Code, Name = language.Name, Icon = language.Icon };
    }

    /// <summary>Deletes one or more languages by ID.</summary>
    /// <param name="ids">IDs of languages to delete.</param>
    public async Task DeleteLanguagesAsync(List<string> ids)
    {
        await _dbContext.Languages.Where(x => ids.Contains(x.Id)).ExecuteDeleteAsync();
    }

    // ─── Admin CRUD for Code Keys ─────────────────────────────────────────────

    /// <summary>
    /// Creates a single translation code key. The code is normalised to lowercase and must be unique.
    /// </summary>
    /// <param name="code">Raw code string to register.</param>
    /// <returns>The created <see cref="CodeLanguageResponse"/>.</returns>
    /// <exception cref="BadRequestException">Thrown when the code already exists.</exception>
    public async Task<CodeLanguageResponse> CreateSingleCodeKeyAsync(string code)
    {
        var normalized = code.Trim().ToLowerInvariant();
        var exists = await _dbContext.CodeLanguages.AnyAsync(x => x.Code == normalized);
        if (exists)
        {
            throw new BadRequestException(ApplicationMessage.ExitsCode);
        }

        var entity = new CodeLanguage { Code = normalized };
        _dbContext.CodeLanguages.Add(entity);
        await _dbContext.SaveChangesAsync();

        return new CodeLanguageResponse { Id = entity.Id, Code = entity.Code };
    }

    // ─── Admin CRUD for Translates ────────────────────────────────────────────

    /// <summary>Returns all translation entries as a flat list.</summary>
    /// <returns>A list of <see cref="TranslateItemResponse"/> objects.</returns>
    public async Task<List<TranslateItemResponse>> GetAllTranslatesAsync()
    {
        return await _dbContext.Translates
            .AsNoTracking()
            .Select(t => new TranslateItemResponse
            {
                Id = t.Id,
                CodeId = t.CodeId,
                LanguageId = t.LanguageId,
                Value = t.Value
            })
            .ToListAsync();
    }

    /// <summary>Creates a single translation entry.</summary>
    /// <param name="request">Translation creation payload.</param>
    /// <returns>The created <see cref="TranslateItemResponse"/>.</returns>
    public async Task<TranslateItemResponse> CreateTranslateAsync(CreateTranslateAdminRequest request)
    {
        var entity = new Translate
        {
            CodeId = request.CodeId,
            LanguageId = request.LanguageId,
            Value = request.Value
        };
        _dbContext.Translates.Add(entity);
        await _dbContext.SaveChangesAsync();

        return new TranslateItemResponse
        {
            Id = entity.Id,
            CodeId = entity.CodeId,
            LanguageId = entity.LanguageId,
            Value = entity.Value
        };
    }

    /// <summary>Updates the value of an existing translation entry.</summary>
    /// <param name="request">Update payload with entry ID and new value.</param>
    /// <returns>The updated <see cref="TranslateItemResponse"/>.</returns>
    /// <exception cref="NotFoundException">Thrown when the entry ID does not exist.</exception>
    public async Task<TranslateItemResponse> UpdateTranslateAsync(UpdateTranslateAdminRequest request)
    {
        var entity = await _dbContext.Translates.FirstOrDefaultAsync(x => x.Id == request.Id)
            ?? throw new NotFoundException();

        entity.Value = request.Value;
        _dbContext.Translates.Update(entity);
        await _dbContext.SaveChangesAsync();

        return new TranslateItemResponse
        {
            Id = entity.Id,
            CodeId = entity.CodeId,
            LanguageId = entity.LanguageId,
            Value = entity.Value
        };
    }

    /// <summary>Deletes one or more translation entries by ID.</summary>
    /// <param name="ids">IDs of translation entries to delete.</param>
    public async Task DeleteTranslatesAsync(List<string> ids)
    {
        await _dbContext.Translates.Where(x => ids.Contains(x.Id)).ExecuteDeleteAsync();
    }
}

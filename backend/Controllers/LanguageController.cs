using backend.Services;
using backend.Services.Internals;
using backend.ViewModels.Languages.Requests;
using backend.ViewModels.Languages.Responses;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

/// <summary>
/// Endpoints for managing UI languages, translation code keys, and per-language translation values.
/// </summary>
[ApiController]
[Route("api/language")]
public class LanguageController : ControllerBase
{
    private readonly ILogger<LanguageController> _logger;
    private readonly LanguageServices _languageServices;

    /// <summary>Initialises the controller with required dependencies.</summary>
    public LanguageController(ILogger<LanguageController> logger, LanguageServices languageServices)
    {
        _logger = logger;
        _languageServices = languageServices;
    }

    /// <summary>Returns all translations for the specified language code, or 404 if the language is not registered.</summary>
    [HttpGet("")]
    public async Task<ActionResult<TranslateLanguageResponse>> GetTranslateLanguage([FromQuery] string lang)
    {
        TranslateLanguageResponse? response = await _languageServices.GetTranslatesAsync(lang);
        if (response is null)
        {
            return NotFound();
        }
        return Ok(response);
    }

    /// <summary>Registers new translation code keys. Already-existing codes are silently skipped.</summary>
    [HttpPost("create-code-language")]
    public async Task<IActionResult> CreateCodeLanguage([FromBody] string[] lang)
    {
        await _languageServices.CreateCodeLanguageAsync(lang.ToList());
        return NoContent();
    }

    /// <summary>Updates an existing translation code key.</summary>
    [HttpPost("update-code-language")]
    public async Task<IActionResult> UpdateCodeLanguage([FromBody] UpdateCodeLanguage request)
    {
        await _languageServices.UpdateCodeLanguageAsync(request);
        return NoContent();
    }

    /// <summary>Deletes one or more translation code keys by ID.</summary>
    [HttpDelete("delete-code-lanuages")]
    public async Task<IActionResult> DeleteCodeLanguages([FromBody] string[] ids)
    {
        await _languageServices.DeleteCodeLanguageAsync(ids.ToList());
        return NoContent();
    }

    /// <summary>Registers a new UI language.</summary>
    [HttpPost("create-language")]
    public async Task<IActionResult> CreateLanguageAsync([FromBody] LanguageRequest request)
    {
        await _languageServices.CreateLanguageAsync(request);
        return NoContent();
    }

    /// <summary>Updates an existing language's code and display name.</summary>
    [HttpPut("update-language")]
    public async Task<IActionResult> UpdateLanguage([FromQuery] string id, LanguageRequest request)
    {
        await _languageServices.UpdateLanguageAsync(id, request);
        return NoContent();
    }

    /// <summary>Deletes a language by ID.</summary>
    [HttpDelete("delete-language")]
    public async Task<IActionResult> DeleteLanguage([FromQuery] string id)
    {
        await _languageServices.DeleteLanguageAsync(id);
        return NoContent();
    }

    /// <summary>Returns all registered UI languages.</summary>
    [HttpGet("list-language")]
    public async Task<IActionResult> GetListLanguage()
    {
        var result = await _languageServices.GetListLanguageAsync();
        return Ok(result);
    }

    /// <summary>Returns all registered translation code keys.</summary>
    [HttpGet("code-languages")]
    public async Task<ActionResult<IReadOnlyCollection<CodeLanguageResponse>>> GetCodeLanguages()
    {
        var result = await _languageServices.GetCodeLanguagesAsync();
        return Ok(result);
    }

    /// <summary>Upserts translation values for a given code key across multiple languages.</summary>
    [HttpPost("defined-code")]
    public async Task<IActionResult> DefinedCodeLanguage([FromBody] TranslateCodeLanguageRequest request)
    {
        await _languageServices.DefinedCodeLanguageAsync(request);
        return Ok();
    }
}

using backend.Services.Internals;
using backend.ViewModels.Languages.Requests;
using backend.ViewModels.Languages.Responses;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

/// <summary>Admin CRUD endpoints for registered UI languages.</summary>
[ApiController]
[Route("api/languages")]
public class LanguagesController : ControllerBase
{
    private readonly ILogger<LanguagesController> _logger;
    private readonly LanguageServices _languageServices;

    /// <summary>Initialises the controller with required dependencies.</summary>
    public LanguagesController(ILogger<LanguagesController> logger, LanguageServices languageServices)
    {
        _logger = logger;
        _languageServices = languageServices;
    }

    /// <summary>Returns all registered UI languages.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<LanguageResponse>>> GetAll()
    {
        var result = await _languageServices.GetListLanguageAsync();
        return Ok(result);
    }

    /// <summary>
    /// Returns the translation dictionary for the requested language as a flat key-value map.
    /// Used by the frontend <c>LanguageService</c> to populate its i18n dictionary.
    /// </summary>
    /// <param name="lang">BCP-47 language code (e.g. <c>en</c>, <c>vi</c>).</param>
    [HttpGet("dictionary")]
    public async Task<ActionResult<Dictionary<string, string>>> GetDictionary([FromQuery] string lang)
    {
        var result = await _languageServices.GetTranslatesAsync(lang);
        if (result is null) return NotFound();
        return Ok(result.Translations.ToDictionary(t => t.Code, t => t.Value));
    }

    /// <summary>Creates a new language and returns it.</summary>
    [HttpPost("create")]
    public async Task<ActionResult<LanguageResponse>> Create([FromBody] LanguageRequest request)
    {
        var result = await _languageServices.CreateLanguageAdminAsync(request);
        return Ok(result);
    }

    /// <summary>Updates an existing language and returns the updated record.</summary>
    [HttpPut("update")]
    public async Task<ActionResult<LanguageResponse>> Update([FromBody] UpdateLanguageAdminRequest request)
    {
        var result = await _languageServices.UpdateLanguageAdminAsync(request);
        return Ok(result);
    }

    /// <summary>Deletes one or more languages by ID.</summary>
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([AsParameters] List<string> ids)
    {
        await _languageServices.DeleteLanguagesAsync(ids);
        return NoContent();
    }
}

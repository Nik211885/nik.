using backend.Services.Internals;
using backend.ViewModels.Languages.Requests;
using backend.ViewModels.Languages.Responses;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

/// <summary>Admin CRUD endpoints for translation code keys.</summary>
[ApiController]
[Route("api/code-languages")]
public class CodeLanguagesController : ControllerBase
{
    private readonly ILogger<CodeLanguagesController> _logger;
    private readonly LanguageServices _languageServices;

    /// <summary>Initialises the controller with required dependencies.</summary>
    public CodeLanguagesController(ILogger<CodeLanguagesController> logger, LanguageServices languageServices)
    {
        _logger = logger;
        _languageServices = languageServices;
    }

    /// <summary>Returns all registered translation code keys.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<CodeLanguageResponse>>> GetAll()
    {
        var result = await _languageServices.GetCodeLanguagesAsync();
        return Ok(result);
    }

    /// <summary>Creates a single translation code key and returns it.</summary>
    [HttpPost("create")]
    public async Task<ActionResult<CodeLanguageResponse>> Create([FromBody] CreateCodeKeyRequest request)
    {
        var result = await _languageServices.CreateSingleCodeKeyAsync(request.Code);
        return Ok(result);
    }

    /// <summary>Deletes one or more translation code keys by ID.</summary>
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromQuery] List<string> ids)
    {
        await _languageServices.DeleteCodeLanguageAsync(ids);
        return NoContent();
    }
}

using backend.Services.Internals;
using backend.ViewModels.Languages.Requests;
using backend.ViewModels.Languages.Responses;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

/// <summary>Admin CRUD endpoints for translation entries.</summary>
[ApiController]
[Route("api/translates")]
public class TranslatesController : ControllerBase
{
    private readonly ILogger<TranslatesController> _logger;
    private readonly LanguageServices _languageServices;

    /// <summary>Initialises the controller with required dependencies.</summary>
    public TranslatesController(ILogger<TranslatesController> logger, LanguageServices languageServices)
    {
        _logger = logger;
        _languageServices = languageServices;
    }

    /// <summary>Returns all translation entries as a flat list.</summary>
    [HttpGet]
    public async Task<ActionResult<List<TranslateItemResponse>>> GetAll()
    {
        var result = await _languageServices.GetAllTranslatesAsync();
        return Ok(result);
    }

    /// <summary>Creates a single translation entry and returns it.</summary>
    [HttpPost("create")]
    public async Task<ActionResult<TranslateItemResponse>> Create([FromBody] CreateTranslateAdminRequest request)
    {
        var result = await _languageServices.CreateTranslateAsync(request);
        return Ok(result);
    }

    /// <summary>Updates a translation entry's value and returns the updated record.</summary>
    [HttpPut("update")]
    public async Task<ActionResult<TranslateItemResponse>> Update([FromBody] UpdateTranslateAdminRequest request)
    {
        var result = await _languageServices.UpdateTranslateAsync(request);
        return Ok(result);
    }

    /// <summary>Deletes one or more translation entries by ID.</summary>
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([AsParameters] List<string> ids)
    {
        await _languageServices.DeleteTranslatesAsync(ids);
        return NoContent();
    }
}

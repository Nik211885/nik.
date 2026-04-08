using backend.Services;
using backend.ViewModels.Languages.Requests;
using backend.ViewModels.Languages.Responses;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/language")]
public class LanguageController : ControllerBase
{
    private readonly ILogger<LanguageController> _logger;
    private readonly LanguageServices _languageServices;

    public LanguageController(ILogger<LanguageController> logger, LanguageServices languageServices)
    {
        _logger = logger;
        _languageServices = languageServices;
    }

    [HttpGet("")]
    public async Task<ActionResult<TranslateLanguageResponse>> GetTranslateLanguage([FromQuery] string lang)
    {
        TranslateLanguageResponse? response = await _languageServices.GetTranslates(lang);
        if (response is null)
        {
            return NotFound();
        }
        return Ok(response);
    }

    [HttpPost("create-code-language")]
    public async Task<IActionResult> CreateCodeLanguage([FromBody] string[] lang)
    {
        await _languageServices.CreateCodeLanguageAsync(lang.ToList());
        return NoContent();
    }

    [HttpPost("update-code-language")]
    public async Task<IActionResult> UpdateCodeLanguage([FromBody] UpdateCodeLanguage request)
    {
        await _languageServices.UpdateCodeLanguageAsync(request);
        return NoContent();
    }

    [HttpDelete("delete-code-lanuages")]
    public async Task<IActionResult> DeleteCodeLanguages([FromBody] string[] ids)
    {
        await _languageServices.DeleteCodeLanguageAsync(ids.ToList());
        return NoContent();
    }

    [HttpPost("create-language")]
    public async Task<IActionResult> CreateLanguageAsync([FromBody] LanguageRequest request)
    {
        await _languageServices.CreateLanguageAsync(request);
        return NoContent();
    }

    [HttpPut("update-language")]
    public async Task<IActionResult> UpdateLanguage([FromQuery] string id, LanguageRequest request)
    {
        await _languageServices.UpdateLanguageAsync(id, request);
        return NoContent();
    }

    [HttpDelete("delete-language")]
    public async Task<IActionResult> DeleteLanguage([FromQuery] string id)
    {
        await _languageServices.DeleteLanguageAsync(id);
        return NoContent();
    }

    [HttpGet("list-language")]
    public async Task<IActionResult> GetListLanguage()
    {
        var result = await _languageServices.GetListLanguageAsync();
        return Ok(result);
    }
}

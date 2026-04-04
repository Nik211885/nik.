using backend.Services;
using backend.ViewModels.Responses;
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
}

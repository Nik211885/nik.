using backend.Services.Internals;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

/// <summary>Provides on-demand text translation via LibreTranslate.</summary>
[ApiController]
[Route("api/auto-translate")]
public class AutoTranslateController : ControllerBase
{
    private readonly ILogger<AutoTranslateController> _logger;
    private readonly AutoTranslateService _translateService;

    /// <summary>Initialises the controller with required dependencies.</summary>
    public AutoTranslateController(ILogger<AutoTranslateController> logger, AutoTranslateService translateService)
    {
        _logger = logger;
        _translateService = translateService;
    }

    /// <summary>Translates the given text to the target language.</summary>
    /// <param name="text">Text to translate.</param>
    /// <param name="to">Target language code (e.g. <c>vi</c>, <c>en</c>).</param>
    /// <returns>Object with <c>translatedText</c> field.</returns>
    [HttpGet]
    public async Task<ActionResult> Translate([FromQuery] string text, [FromQuery] string to)
    {
        if (string.IsNullOrWhiteSpace(text)) return Ok(new { translatedText = text ?? "" });
        if (string.IsNullOrWhiteSpace(to))   return BadRequest("to is required");

        var translated = await _translateService.TranslateAsync(text, to);
        return Ok(new { translatedText = translated });
    }
}

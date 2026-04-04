using backend.Services;
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
}

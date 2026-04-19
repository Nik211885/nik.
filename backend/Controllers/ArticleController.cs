using backend.Services;
using backend.Services.Internals;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/articles")]
public class ArticleController : ControllerBase
{
    private readonly ILogger<ArticleController> _logger;
    private readonly ArticleServices _articleServices;

    public ArticleController(ILogger<ArticleController> logger, ArticleServices articleServices)
    {
        _logger = logger;
        _articleServices = articleServices;
    }
}

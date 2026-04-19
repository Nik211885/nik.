using backend.Services;
using backend.Services.Internals;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
    private readonly ILogger<CategoryController> _logger;
    private readonly CategoryServices _articleServices;

    public CategoryController(ILogger<CategoryController> logger, CategoryServices articleServices)
    {
        _logger = logger;
        _articleServices = articleServices;
    }
}

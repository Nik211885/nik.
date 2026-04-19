using backend.Services;
using backend.Services.Internals;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/comments")]
public class CommentController : ControllerBase
{
    private readonly ILogger<CommentController> _logger;
    private readonly CommentServices _commentServices;

    public CommentController(ILogger<CommentController> logger, CommentServices commentServices)
    {
        _logger = logger;
        _commentServices = commentServices;
    }
}

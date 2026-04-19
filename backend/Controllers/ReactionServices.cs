using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/reactions")]
public class ReactionServices
{
    private readonly ILogger<ReactionServices> _logger;
    private readonly ReactionServices _reactionServices;

    public ReactionServices(ILogger<ReactionServices> logger, ReactionServices reactionServices)
    {
        _logger = logger;
        _reactionServices = reactionServices;
    }
}

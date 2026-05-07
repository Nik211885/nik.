using backend.Services.Internals;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

/// <summary>Endpoints for user management operations.</summary>
[ApiController]
[Route("api/users")]
public class UserController
{
    private readonly ILogger<UserController> _logger;
    private readonly UserServices _userServices;

    /// <summary>Initialises the controller with required dependencies.</summary>
    public UserController(ILogger<UserController> logger, UserServices userServices)
    {
        _logger = logger;
        _userServices = userServices;
    }
}

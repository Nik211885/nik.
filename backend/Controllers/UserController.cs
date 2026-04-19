using backend.Services.Internals;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/users")]
public class UserController
{
    private readonly ILogger<UserController> _logger;
    private readonly UserServices _userServices;

    public UserController(ILogger<UserController> logger, UserServices userServices)
    {
        _logger = logger;
        _userServices = userServices;
    }
}

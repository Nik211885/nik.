using backend.Pipes.Filter;
using backend.Services.Internals;
using backend.ViewModels.Users.Requests;
using backend.ViewModels.Users.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

/// <summary>Endpoints for user management operations.</summary>
[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly UserServices _userServices;

    /// <summary>Initialises the controller with required dependencies.</summary>
    public UserController(ILogger<UserController> logger, UserServices userServices)
    {
        _logger = logger;
        _userServices = userServices;
    }

    /// <summary>Returns all users.</summary>
    /// <returns>A list of <see cref="UserResponse"/> objects.</returns>
    [HttpGet]
    public async Task<ActionResult<List<UserResponse>>> GetUsers()
    {
        var result = await _userServices.GetUsersAsync();
        return Ok(result);
    }

    /// <summary>Returns a single user by ID.</summary>
    /// <param name="id">The user ID.</param>
    /// <returns>The matching <see cref="UserResponse"/>.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> GetUser(string id)
    {
        var result = await _userServices.GetUserAsync(id);
        return Ok(result);
    }

    /// <summary>Updates the authenticated user's profile.</summary>
    /// <param name="request">Updated profile data.</param>
    /// <returns>The updated <see cref="UserResponse"/>.</returns>
    [HttpPut("update")]
    [Authorize]
    [ValidationFilter(typeof(UpdateUserRequest))]
    public async Task<ActionResult<UserResponse>> UpdateUser([FromBody] UpdateUserRequest request)
    {
        var result = await _userServices.UpdateUserAsync(request);
        return Ok(result);
    }

    /// <summary>Updates a specific user's profile by ID (admin use).</summary>
    /// <param name="id">The target user ID.</param>
    /// <param name="request">Updated profile data.</param>
    /// <returns>The updated <see cref="UserResponse"/>.</returns>
    [HttpPut("update/{id}")]
    [ValidationFilter(typeof(UpdateUserRequest))]
    public async Task<ActionResult<UserResponse>> UpdateUserById(string id, [FromBody] UpdateUserRequest request)
    {
        var result = await _userServices.UpdateUserByIdAsync(id, request);
        return Ok(result);
    }
}

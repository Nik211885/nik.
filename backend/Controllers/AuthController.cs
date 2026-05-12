using backend.Pipes.Filter;
using backend.Services.Internals;
using backend.ViewModels.Auth.Requests;
using backend.ViewModels.Auth.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

/// <summary>Endpoints for authentication: login, register, logout, token refresh, and profile.</summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly AuthServices _authServices;

    /// <summary>Initialises the controller with required dependencies.</summary>
    public AuthController(ILogger<AuthController> logger, AuthServices authServices)
    {
        _logger = logger;
        _authServices = authServices;
    }

    /// <summary>Authenticates a user and returns a JWT access + refresh token pair.</summary>
    /// <param name="request">Login credentials.</param>
    /// <returns>A <see cref="TokenResponse"/> containing the token pair.</returns>
    [HttpPost("login")]
    [ValidationFilter(typeof(LoginRequest))]
    public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginRequest request)
    {
        var result = await _authServices.LoginAsync(request);
        return Ok(result);
    }

    /// <summary>Creates a new user account and returns a token pair.</summary>
    /// <param name="request">Registration payload.</param>
    /// <returns>A <see cref="TokenResponse"/> for the newly created account.</returns>
    [HttpPost("register")]
    [ValidationFilter(typeof(RegisterRequest))]
    public async Task<ActionResult<TokenResponse>> Register([FromBody] RegisterRequest request)
    {
        var result = await _authServices.RegisterAsync(request);
        return Ok(result);
    }

    /// <summary>Revokes the current user's refresh token, effectively logging them out.</summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> Logout()
    {
        await _authServices.LogoutAsync();
        return Ok();
    }

    /// <summary>Exchanges a valid refresh token for a new access + refresh token pair.</summary>
    /// <param name="request">The refresh token to exchange.</param>
    /// <returns>A new <see cref="TokenResponse"/>.</returns>
    [HttpPost("refresh-token")]
    [ValidationFilter(typeof(RefreshTokenRequest))]
    public async Task<ActionResult<TokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var result = await _authServices.RefreshTokenAsync(request);
        return Ok(result);
    }

    /// <summary>Returns the profile of the currently authenticated user.</summary>
    /// <returns>The <see cref="UserProfileResponse"/> for the caller.</returns>
    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult<UserProfileResponse>> GetProfile()
    {
        var result = await _authServices.GetProfileAsync();
        return Ok(result);
    }
}

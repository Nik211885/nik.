using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using backend.Data;
using backend.Entities;
using backend.Exceptions;
using backend.Extensions;
using backend.ViewModels.Auth.Requests;
using backend.ViewModels.Auth.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace backend.Services.Internals;

/// <summary>Provides authentication operations: login, register, logout, and token refresh.</summary>
public class AuthServices
{
    private readonly ILogger<AuthServices> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IHttpContextAccessor _httpContext;
    private readonly IConfiguration _configuration;

    /// <summary>Initialises the service with required dependencies.</summary>
    public AuthServices(
        ILogger<AuthServices> logger,
        ApplicationDbContext dbContext,
        IPasswordHasher<User> passwordHasher,
        IHttpContextAccessor httpContext,
        IConfiguration configuration)
    {
        _logger = logger;
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _httpContext = httpContext;
        _configuration = configuration;
    }

    /// <summary>
    /// Validates credentials and returns a new JWT access + refresh token pair.
    /// </summary>
    /// <param name="request">Login credentials.</param>
    /// <returns>A new <see cref="TokenResponse"/> on success.</returns>
    /// <exception cref="BadRequestException">Thrown when the credentials are invalid.</exception>
    public async Task<TokenResponse> LoginAsync(LoginRequest request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u =>
            u.Email == request.EmailOrUserName || u.UserName == request.EmailOrUserName)
            ?? throw new BadRequestException(ApplicationMessage.InvalidCredentials);

        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);
        if (result == PasswordVerificationResult.Failed)
            throw new BadRequestException(ApplicationMessage.InvalidCredentials);

        return await IssueTokensAsync(user);
    }

    /// <summary>
    /// Creates a new user account and returns a token pair.
    /// </summary>
    /// <param name="request">Registration payload.</param>
    /// <returns>A new <see cref="TokenResponse"/> for the created user.</returns>
    /// <exception cref="BadRequestException">Thrown when the email or username is already taken.</exception>
    public async Task<TokenResponse> RegisterAsync(RegisterRequest request)
    {
        var exists = await _dbContext.Users.AnyAsync(u =>
            u.Email == request.Email || u.UserName == request.UserName);
        if (exists) throw new BadRequestException(ApplicationMessage.ExitsCode);

        var user = request.ToUser();
        user.Password = _passwordHasher.HashPassword(user, request.Password);
        user.Slug = request.UserName.ToSlug();
        user.CreatedDate = DateTimeOffset.UtcNow;
        user.UpdatedDate = DateTimeOffset.UtcNow;

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return await IssueTokensAsync(user);
    }

    /// <summary>
    /// Revokes the refresh token for the currently authenticated user.
    /// </summary>
    public async Task LogoutAsync()
    {
        var userId = _httpContext.HttpContext!.GetUserId();
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null) return;

        user.RefreshToken = null;
        user.RefreshTokenExpiresAt = null;
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Validates a refresh token and issues a new token pair.
    /// </summary>
    /// <param name="request">The refresh token to exchange.</param>
    /// <returns>A new <see cref="TokenResponse"/>.</returns>
    /// <exception cref="BadRequestException">Thrown when the token is invalid or expired.</exception>
    public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u =>
            u.RefreshToken == request.RefreshToken)
            ?? throw new BadRequestException(ApplicationMessage.InvalidToken);

        if (user.RefreshTokenExpiresAt < DateTimeOffset.UtcNow)
            throw new BadRequestException(ApplicationMessage.InvalidToken);

        return await IssueTokensAsync(user);
    }

    /// <summary>
    /// Returns the profile of the currently authenticated user.
    /// </summary>
    /// <returns>The <see cref="UserProfileResponse"/> for the current user.</returns>
    /// <exception cref="NotFoundException">Thrown when the user ID from the JWT no longer exists.</exception>
    public async Task<UserProfileResponse> GetProfileAsync()
    {
        var userId = _httpContext.HttpContext!.GetUserId();
        var user = await _dbContext.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new NotFoundException();

        return new UserProfileResponse
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            Roles = []
        };
    }

    private async Task<TokenResponse> IssueTokensAsync(User user)
    {
        var refreshExpirationDays = int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"]!);

        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiresAt = DateTimeOffset.UtcNow.AddDays(refreshExpirationDays);
        await _dbContext.SaveChangesAsync();

        return new TokenResponse
        {
            AccessToken = GenerateAccessToken(user),
            RefreshToken = user.RefreshToken
        };
    }

    private string GenerateAccessToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expirationMinutes = int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"]!);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Name, user.UserName)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
}

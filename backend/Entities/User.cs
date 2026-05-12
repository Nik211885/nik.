namespace backend.Entities;

/// <summary>
/// Represents the portfolio owner or any authenticated user in the system.
/// </summary>
public class User : BaseEntity
{
    /// <summary>Display name used throughout the UI.</summary>
    public string UserName { get; set; }

    /// <summary>Optional email address for contact or authentication.</summary>
    public string? Email { get; set; }

    /// <summary>Optional phone number for contact.</summary>
    public string? Phone { get; set; }

    /// <summary>Hashed password for local authentication.</summary>
    public string Password { get; set; }

    /// <summary>Short biography displayed on the about/profile page.</summary>
    public string Bio { get; set; }

    /// <summary>UTC timestamp when the user account was created.</summary>
    public DateTimeOffset CreatedDate { get; set; }

    /// <summary>UTC timestamp of the last profile modification.</summary>
    public DateTimeOffset UpdatedDate { get; set; }

    /// <summary>
    /// URL-friendly slug used to build the public profile URL.
    /// Generated via <see cref="Extensions.StringExtensions.ToSlug"/>.
    /// </summary>
    public string Slug { get; set; }

    /// <summary>Opaque refresh token stored server-side for token rotation validation.</summary>
    public string? RefreshToken { get; set; }

    /// <summary>UTC expiry of <see cref="RefreshToken"/>. <see langword="null"/> when logged out.</summary>
    public DateTimeOffset? RefreshTokenExpiresAt { get; set; }
}

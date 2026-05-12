using FluentValidation;

namespace backend.ViewModels.Auth.Requests;

/// <summary>Request model for exchanging a refresh token for a new token pair.</summary>
public class RefreshTokenRequest
{
    /// <summary>The opaque refresh token previously issued by the server.</summary>
    public string RefreshToken { get; set; }
}

/// <summary>FluentValidation rules for <see cref="RefreshTokenRequest"/>.</summary>
public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    /// <inheritdoc/>
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}

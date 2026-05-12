using FluentValidation;

namespace backend.ViewModels.Auth.Requests;

/// <summary>Request model for authenticating an existing user.</summary>
public class LoginRequest
{
    /// <summary>The user's email address or username.</summary>
    public string EmailOrUserName { get; set; }

    /// <summary>The user's plain-text password.</summary>
    public string Password { get; set; }
}

/// <summary>FluentValidation rules for <see cref="LoginRequest"/>.</summary>
public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    /// <inheritdoc/>
    public LoginRequestValidator()
    {
        RuleFor(x => x.EmailOrUserName).NotEmpty();
        RuleFor(x => x.Password).NotEmpty().WithMessage(ApplicationMessage.PasswordIsRequired);
    }
}

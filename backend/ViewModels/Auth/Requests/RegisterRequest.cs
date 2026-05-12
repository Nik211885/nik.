using backend.Entities;
using FluentValidation;

namespace backend.ViewModels.Auth.Requests;

/// <summary>Request model for registering a new user account.</summary>
public class RegisterRequest
{
    /// <summary>The user's email address.</summary>
    public string Email { get; set; }

    /// <summary>The desired display name / login handle.</summary>
    public string UserName { get; set; }

    /// <summary>The plain-text password to hash and store.</summary>
    public string Password { get; set; }

    /// <summary>Must match <see cref="Password"/> exactly.</summary>
    public string ConfirmPassword { get; set; }
}

/// <summary>FluentValidation rules for <see cref="RegisterRequest"/>.</summary>
public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    /// <inheritdoc/>
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage(ApplicationMessage.EmailIsRequired).EmailAddress();
        RuleFor(x => x.UserName).NotEmpty().WithMessage(ApplicationMessage.UserNameIsRequired);
        RuleFor(x => x.Password).NotEmpty().WithMessage(ApplicationMessage.PasswordIsRequired).MinimumLength(6);
        RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage(ApplicationMessage.PasswordNotMatch);
    }
}

/// <summary>Mapping extensions for <see cref="RegisterRequest"/>.</summary>
public static class RegisterRequestExtensions
{
    extension(RegisterRequest request)
    {
        /// <summary>
        /// Maps the request to a new <see cref="User"/> entity.
        /// Does not set <c>Password</c>, <c>Slug</c>, or timestamps — the service sets those.
        /// </summary>
        public User ToUser()
        {
            return new User
            {
                UserName = request.UserName,
                Email = request.Email,
                Bio = string.Empty
            };
        }
    }
}

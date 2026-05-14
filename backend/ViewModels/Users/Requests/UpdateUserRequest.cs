using FluentValidation;

namespace backend.ViewModels.Users.Requests;

/// <summary>Request model for updating the authenticated user's profile.</summary>
public class UpdateUserRequest
{
    /// <summary>New display name.</summary>
    public string UserName { get; set; }

    /// <summary>New email address. <see langword="null"/> leaves the existing value unchanged.</summary>
    public string? Email { get; set; }

    /// <summary>New phone number. <see langword="null"/> leaves the existing value unchanged.</summary>
    public string? Phone { get; set; }

    /// <summary>Profile picture URL. <see langword="null"/> leaves the existing value unchanged.</summary>
    public string? Avatar { get; set; }

    /// <summary>Updated short biography.</summary>
    public string Bio { get; set; }
}

/// <summary>FluentValidation rules for <see cref="UpdateUserRequest"/>.</summary>
public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    /// <inheritdoc/>
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage(ApplicationMessage.UserNameIsRequired);
        RuleFor(x => x.Bio).NotEmpty();
    }
}

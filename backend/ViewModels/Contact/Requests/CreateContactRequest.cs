using FluentValidation;

namespace backend.ViewModels.Contact.Requests;

/// <summary>Request model for submitting a contact form.</summary>
public class CreateContactRequest
{
    /// <summary>Full name of the sender.</summary>
    public string Name { get; set; }

    /// <summary>Email address of the sender.</summary>
    public string Email { get; set; }

    /// <summary>Subject of the enquiry.</summary>
    public string Subject { get; set; }

    /// <summary>Body of the message.</summary>
    public string Message { get; set; }
}

/// <summary>FluentValidation rules for <see cref="CreateContactRequest"/>.</summary>
public class CreateContactRequestValidator : AbstractValidator<CreateContactRequest>
{
    /// <inheritdoc/>
    public CreateContactRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(255);
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Message).NotEmpty().MaximumLength(2000);
    }
}

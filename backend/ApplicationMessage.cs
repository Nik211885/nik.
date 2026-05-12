namespace backend;

/// <summary>
/// Centralised repository of i18n message keys used for API error responses
/// and FluentValidation messages. All values are translation keys resolved
/// at runtime by the <c>TranslatedHandlingMiddleware</c>.
/// </summary>
public static class ApplicationMessage
{
    // ── Exception keys ────────────────────────────────────────────────────

    /// <summary>Key for 404 Not Found responses.</summary>
    public static readonly string NotFound = "exception.not_found";

    /// <summary>Key for generic 400 Bad Request responses.</summary>
    public static readonly string BadRequest = "exception.bad_request";

    /// <summary>Key used when a resource with the same unique identifier already exists.</summary>
    public static readonly string ExitsCode = "exception.exits_code";

    // ── Validation keys ───────────────────────────────────────────────────

    /// <summary>Key for "Name is required" validation failures.</summary>
    public static readonly string NameIsRequired = "validation.name_is_required";

    /// <summary>Key for "Title is required" validation failures.</summary>
    public static readonly string TitleIsRequired = "validation.title_is_required";

    /// <summary>Key for "Description is required" validation failures.</summary>
    public static readonly string DescriptionIsRequired = "validation.description_is_required";

    /// <summary>Key for "Image is required" validation failures.</summary>
    public static readonly string ImageIsRequired = "validation.image_is_required";

    /// <summary>Key for invalid <see cref="Entities.ReactionType"/> enum value failures.</summary>
    public static readonly string ReactionTypeIsInvalid = "validation.reaction_type_is_invalid";

    /// <summary>Key for "UserName is required" validation failures.</summary>
    public static readonly string UserNameIsRequired = "validation.username_is_required";

    /// <summary>Key for "Email is required" validation failures.</summary>
    public static readonly string EmailIsRequired = "validation.email_is_required";

    /// <summary>Key for "Password is required" validation failures.</summary>
    public static readonly string PasswordIsRequired = "validation.password_is_required";

    /// <summary>Key for "Password does not match" validation failures.</summary>
    public static readonly string PasswordNotMatch = "validation.password_not_match";

    /// <summary>Key returned when email/username or password is incorrect.</summary>
    public static readonly string InvalidCredentials = "exception.invalid_credentials";

    /// <summary>Key returned when a refresh token is invalid or expired.</summary>
    public static readonly string InvalidToken = "exception.invalid_token";
}

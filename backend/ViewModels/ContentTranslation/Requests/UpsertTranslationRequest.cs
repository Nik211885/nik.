using backend.Exceptions;
using FluentValidation;

namespace backend.ViewModels.ContentTranslation.Requests;

/// <summary>Payload for creating or updating one or more translated fields on a content entity.</summary>
public class UpsertTranslationRequest
{
    /// <summary>Logical entity type (e.g. <c>"article"</c>, <c>"category"</c>).</summary>
    public string EntityType { get; set; }

    /// <summary>ID of the entity being translated.</summary>
    public string EntityId { get; set; }

    /// <summary>BCP-47 base language code of the translation (e.g. <c>"en"</c>).</summary>
    public string LangCode { get; set; }

    /// <summary>
    /// Field values to upsert, keyed by field name (e.g. <c>"title"</c>).
    /// Empty-string values are skipped and not persisted.
    /// </summary>
    public Dictionary<string, string> Fields { get; set; }
}

/// <summary>Validates <see cref="UpsertTranslationRequest"/>.</summary>
public class UpsertTranslationRequestValidator : AbstractValidator<UpsertTranslationRequest>
{
    /// <inheritdoc/>
    public UpsertTranslationRequestValidator()
    {
        RuleFor(x => x.EntityType).NotEmpty().WithMessage(ApplicationMessage.BadRequest);
        RuleFor(x => x.EntityId).NotEmpty().WithMessage(ApplicationMessage.BadRequest);
        RuleFor(x => x.LangCode).NotEmpty().WithMessage(ApplicationMessage.BadRequest);
        RuleFor(x => x.Fields).NotNull()
            .Must(f => f != null && f.Count > 0)
            .WithMessage(ApplicationMessage.BadRequest);
    }
}

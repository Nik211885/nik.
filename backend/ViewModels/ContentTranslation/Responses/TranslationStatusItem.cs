namespace backend.ViewModels.ContentTranslation.Responses;

/// <summary>Summary of an entity's translation status for the admin translation list page.</summary>
public class TranslationStatusItem
{
    /// <summary>ID of the entity.</summary>
    public string EntityId { get; set; }

    /// <summary>Logical entity type (e.g. <c>"article"</c>).</summary>
    public string EntityType { get; set; }

    /// <summary>Vietnamese source title used to identify the entity in the admin list.</summary>
    public string SourceTitle { get; set; }

    /// <summary><see langword="true"/> when at least one field has been translated for the requested language.</summary>
    public bool IsTranslated { get; set; }
}

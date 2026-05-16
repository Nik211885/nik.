namespace backend.ViewModels.ContentTranslation.Responses;

/// <summary>All translated field values for a single entity, keyed by field name.</summary>
public class EntityTranslationResponse
{
    /// <summary>
    /// Translated field values keyed by field name (e.g. <c>"title"</c>, <c>"description"</c>).
    /// Missing fields are represented as empty strings — never <see langword="null"/>.
    /// </summary>
    public Dictionary<string, string> Fields { get; set; } = new();
}

namespace backend.Entities;

/// <summary>Stores a translated value for a single field of any translatable entity.</summary>
public class ContentTranslation
{
    /// <summary>Primary key (UUIDv7).</summary>
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    /// <summary>Logical type of the owning entity (e.g. "article", "category").</summary>
    public string EntityType { get; set; }

    /// <summary>ID of the owning entity.</summary>
    public string EntityId { get; set; }

    /// <summary>Name of the translated field (e.g. "title", "description", "content").</summary>
    public string Field { get; set; }

    /// <summary>BCP-47 base language code of the translation (e.g. "en", "fr").</summary>
    public string LangCode { get; set; }

    /// <summary>Translated text value; never <see langword="null"/> or empty.</summary>
    public string Value { get; set; }
}

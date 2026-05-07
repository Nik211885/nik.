namespace backend.Entities;

/// <summary>
/// Represents a single translated string: the resolved value of a
/// <see cref="CodeLanguage"/> key in a specific <see cref="Language"/>.
/// </summary>
public class Translate : BaseEntity
{
    /// <summary>Foreign key to the translation key (<see cref="CodeLanguage"/>).</summary>
    public string CodeId { get; set; }

    /// <summary>Foreign key to the target <see cref="Language"/>.</summary>
    public string LanguageId { get; set; }

    /// <summary>The translated string value for this key/language combination.</summary>
    public string Value { get; set; }

    /// <summary>Navigation property for the translation key.</summary>
    public CodeLanguage CodeLanguage { get; set; }

    /// <summary>Navigation property for the language this value belongs to.</summary>
    public Language Language { get; set; }
}

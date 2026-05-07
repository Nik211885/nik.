namespace backend.Entities;

/// <summary>
/// Represents a translation key (code) that maps to translated values across
/// multiple <see cref="Language"/> instances via <see cref="Translate"/> records.
/// </summary>
public class CodeLanguage : BaseEntity
{
    /// <summary>
    /// Unique dot-separated translation key, stored lowercase
    /// (e.g. <c>exception.not_found</c>, <c>home.welcome</c>).
    /// </summary>
    public string Code { get; set; }

    /// <summary>All translated values for this key across all languages.</summary>
    public ICollection<Translate> Translates { get; set; }
}

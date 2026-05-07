namespace backend.Entities;

/// <summary>
/// Represents a supported UI language (e.g. English, Vietnamese).
/// Holds the locale <see cref="Code"/> and a collection of translation strings.
/// </summary>
public class Language : BaseEntity
{
    /// <summary>IETF language tag, stored lowercase (e.g. <c>en</c>, <c>vi</c>).</summary>
    public string Code { get; set; }

    /// <summary>Human-readable display name of the language (e.g. "English").</summary>
    public string Name { get; set; }

    /// <summary>All translation strings that belong to this language.</summary>
    public ICollection<Translate> Translates { get; set; }
}

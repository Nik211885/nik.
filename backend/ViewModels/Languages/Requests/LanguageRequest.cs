namespace backend.ViewModels.Languages.Requests;

/// <summary>Request model for creating or updating a supported language.</summary>
public class LanguageRequest
{
    /// <summary>IETF language tag (e.g. <c>en</c>, <c>vi</c>). Stored lowercase.</summary>
    public string Code { get; set; }

    /// <summary>Human-readable display name of the language (e.g. "English").</summary>
    public string Name { get; set; }

    /// <summary>Flag emoji or short icon string (e.g. <c>🇬🇧</c>). Optional.</summary>
    public string? Icon { get; set; }
}

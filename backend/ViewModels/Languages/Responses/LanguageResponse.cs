namespace backend.ViewModels.Languages.Responses;

/// <summary>Language record response returned by the language list endpoint.</summary>
public class LanguageResponse
{
    /// <summary>Language ID.</summary>
    public string Id { get; set; }

    /// <summary>IETF language tag (e.g. <c>en</c>, <c>vi</c>).</summary>
    public string Code { get; set; }

    /// <summary>Human-readable display name (e.g. "English").</summary>
    public string Name { get; set; }

    /// <summary>Flag emoji for display in language switchers (e.g. <c>🇬🇧</c>).</summary>
    public string? Icon { get; set; }
}

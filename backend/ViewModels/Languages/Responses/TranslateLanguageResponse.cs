namespace backend.ViewModels.Languages.Responses;

/// <summary>
/// Full translation dictionary for a specific language.
/// Returned by <c>GET /api/language?lang={code}</c> and consumed by the Angular
/// <c>LanguageService</c> to populate the client-side translation dictionary.
/// </summary>
public class TranslateLanguageResponse
{
    /// <summary>Language code of this dictionary (e.g. <c>en</c>).</summary>
    public string Language { get; set; }

    /// <summary>All key-value translation pairs for this language.</summary>
    public IReadOnlyCollection<TranslateResponse> Translations { get; set; }
}

/// <summary>A single translation key-value pair within <see cref="TranslateLanguageResponse"/>.</summary>
public class TranslateResponse
{
    /// <summary>Translation key code (e.g. <c>home.welcome</c>).</summary>
    public string Code { get; set; }

    /// <summary>Translated string value for this key.</summary>
    public string Value { get; set; }
}

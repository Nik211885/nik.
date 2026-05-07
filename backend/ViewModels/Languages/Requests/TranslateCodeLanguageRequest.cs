namespace backend.ViewModels.Languages.Requests;

/// <summary>
/// Request model for defining or overwriting translations for a specific code key
/// across one or more languages.
/// </summary>
public class TranslateCodeLanguageRequest
{
    /// <summary>
    /// The translation key code to define values for (e.g. <c>home.welcome</c>).
    /// Stored lowercase.
    /// </summary>
    public string CodeDefined { get; set; }

    /// <summary>
    /// List of language-value pairs. Existing translations for included languages
    /// are replaced; languages not listed are left untouched.
    /// </summary>
    public List<TranslateRequest> Translates { get; set; }
}

/// <summary>A single language-to-value translation pair within <see cref="TranslateCodeLanguageRequest"/>.</summary>
public class TranslateRequest
{
    /// <summary>Language code (e.g. <c>en</c>, <c>vi</c>).</summary>
    public string Language { get; set; }

    /// <summary>Translated string value for this language.</summary>
    public string Value { get; set; }
}

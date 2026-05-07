namespace backend.ViewModels.Languages.Responses;

/// <summary>Translation key code response returned by the code-language list endpoint.</summary>
public class CodeLanguageResponse
{
    /// <summary>CodeLanguage ID.</summary>
    public string Id { get; set; }

    /// <summary>Dot-separated translation key code (e.g. <c>exception.not_found</c>).</summary>
    public string Code { get; set; }
}

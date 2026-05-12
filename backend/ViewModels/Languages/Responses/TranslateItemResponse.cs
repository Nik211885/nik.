namespace backend.ViewModels.Languages.Responses;

/// <summary>Flat translation entry returned by the admin translations API.</summary>
public class TranslateItemResponse
{
    /// <summary>Translation entry ID.</summary>
    public string Id { get; set; }

    /// <summary>Foreign key to the <see cref="backend.Entities.CodeLanguage"/>.</summary>
    public string CodeId { get; set; }

    /// <summary>Foreign key to the <see cref="backend.Entities.Language"/>.</summary>
    public string LanguageId { get; set; }

    /// <summary>Translated string value.</summary>
    public string Value { get; set; }
}

namespace backend.ViewModels.Languages.Requests;

/// <summary>Request model for creating a single translation entry in the admin panel.</summary>
public class CreateTranslateAdminRequest
{
    /// <summary>ID of the <see cref="backend.Entities.CodeLanguage"/> this translation belongs to.</summary>
    public string CodeId { get; set; }

    /// <summary>ID of the <see cref="backend.Entities.Language"/> this translation is for.</summary>
    public string LanguageId { get; set; }

    /// <summary>Translated string value.</summary>
    public string Value { get; set; }
}

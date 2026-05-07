namespace backend.ViewModels.Languages.Requests;

/// <summary>Request model for renaming an existing translation key code.</summary>
public class UpdateCodeLanguage
{
    /// <summary>ID of the <see cref="backend.Entities.CodeLanguage"/> to update.</summary>
    public string Id { get; set; }

    /// <summary>New dot-separated translation key code. Stored lowercase.</summary>
    public string Code { get; set; }
}

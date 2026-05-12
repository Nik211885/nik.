namespace backend.ViewModels.Languages.Requests;

/// <summary>Request model for updating a translation entry's value in the admin panel.</summary>
public class UpdateTranslateAdminRequest
{
    /// <summary>ID of the <see cref="backend.Entities.Translate"/> entry to update.</summary>
    public string Id { get; set; }

    /// <summary>New translated string value.</summary>
    public string Value { get; set; }
}

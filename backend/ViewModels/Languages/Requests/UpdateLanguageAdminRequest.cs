namespace backend.ViewModels.Languages.Requests;

/// <summary>Request model for updating an existing language record in the admin panel.</summary>
public class UpdateLanguageAdminRequest
{
    /// <summary>ID of the language entry to update.</summary>
    public string Id { get; set; }

    /// <summary>New IETF language tag (e.g. <c>en</c>, <c>vi</c>). Stored lowercase.</summary>
    public string Code { get; set; }

    /// <summary>New human-readable display name (e.g. "English").</summary>
    public string Name { get; set; }
}

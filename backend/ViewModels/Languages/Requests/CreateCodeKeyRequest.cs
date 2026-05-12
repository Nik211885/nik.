namespace backend.ViewModels.Languages.Requests;

/// <summary>Request model for creating a single translation code key.</summary>
public class CreateCodeKeyRequest
{
    /// <summary>Dot-separated translation key code (e.g. <c>home.welcome</c>). Stored lowercase.</summary>
    public string Code { get; set; }
}

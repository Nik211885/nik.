namespace backend.Services.Extends;

/// <summary>Signed upload payload returned to the browser for direct Cloudinary uploads.</summary>
public class CloudinarySignature
{
    /// <summary>The Cloudinary upload endpoint URL (includes cloud name and resource type).</summary>
    public string UploadUrl { get; init; } = string.Empty;

    /// <summary>Cloudinary API key; must be sent as a form field alongside the file.</summary>
    public string ApiKey { get; init; } = string.Empty;

    /// <summary>Unix timestamp used in signature generation; must be sent as a form field.</summary>
    public string Timestamp { get; init; } = string.Empty;

    /// <summary>SHA-1 HMAC of the signed parameters; must be sent as a form field.</summary>
    public string Signature { get; init; } = string.Empty;

    /// <summary>Upload folder; must be sent as a form field because it was included in the signed params.</summary>
    public string Folder { get; init; } = string.Empty;
}

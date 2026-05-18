using System.Security.Cryptography;
using System.Text;

namespace backend.Services.Extends;

/// <summary>Generates signed Cloudinary upload URLs for browser-direct uploads.</summary>
public class CloudinaryService
{
    private readonly string _urlUpload;
    private readonly string _cloudName;
    private readonly string _apiKey;
    private readonly string _apiSecret;
    private readonly string _uploadFolder;

    /// <summary>Initialises the service with Cloudinary configuration from appsettings.</summary>
    /// <param name="configuration">Application configuration.</param>
    public CloudinaryService(IConfiguration configuration)
    {
        var s = configuration.GetSection("Cloudinary");
        _urlUpload    = s["UrlUpload"]    ?? throw new InvalidOperationException("Cloudinary:UrlUpload missing");
        _cloudName    = s["CloudName"]    ?? throw new InvalidOperationException("Cloudinary:CloudName missing");
        _apiKey       = s["ApiKey"]       ?? throw new InvalidOperationException("Cloudinary:ApiKey missing");
        _apiSecret    = s["ApiSecret"]    ?? throw new InvalidOperationException("Cloudinary:ApiSecret missing");
        _uploadFolder = s["UploadFolder"] ?? "static";
    }

    /// <summary>
    /// Returns a signed upload payload for browser-direct uploads.
    /// All parameters must be sent as multipart form fields (not query string) per Cloudinary spec.
    /// </summary>
    /// <param name="resourceType"><c>image</c> or <c>video</c>.</param>
    /// <returns>Upload URL and the form fields to include alongside the file.</returns>
    public CloudinarySignature GetSignedUploadResponse(string resourceType = "image")
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        var parameters = new SortedDictionary<string, string>
        {
            ["folder"]    = _uploadFolder,
            ["timestamp"] = timestamp,
        };

        var stringToSign = string.Join("&", parameters.Select(kv => $"{kv.Key}={kv.Value}")) + _apiSecret;
        var signature    = ComputeSha1(stringToSign);

        return new CloudinarySignature
        {
            UploadUrl  = $"{_urlUpload}/{_cloudName}/{resourceType}/upload",
            ApiKey     = _apiKey,
            Timestamp  = timestamp,
            Signature  = signature,
            Folder     = _uploadFolder,
        };
    }

    private static string ComputeSha1(string input)
    {
        var bytes = SHA1.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLower();
    }
}

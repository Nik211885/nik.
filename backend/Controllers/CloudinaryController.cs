using backend.Services.Extends;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

/// <summary>Provides signed Cloudinary upload URLs for browser-direct file uploads.</summary>
[ApiController]
[Route("api/extend-service/cloudinary")]
public class CloudinaryController : ControllerBase
{
    private readonly ILogger<CloudinaryController> _logger;
    private readonly CloudinaryService _cloudinaryService;

    /// <summary>Initialises the controller with required dependencies.</summary>
    public CloudinaryController(ILogger<CloudinaryController> logger, CloudinaryService cloudinaryService)
    {
        _logger = logger;
        _cloudinaryService = cloudinaryService;
    }

    /// <summary>
    /// Returns a signed Cloudinary upload payload for browser-direct uploads.
    /// The browser posts the file together with the returned fields directly to <c>UploadUrl</c>.
    /// </summary>
    /// <param name="type">Resource type: <c>image</c> (default) or <c>video</c>.</param>
    /// <returns>Signed upload URL and the required form fields.</returns>
    [HttpGet("upload-file-by-signature")]
    public IActionResult GetUploadSignature([FromQuery] string type = "image")
    {
        var resourceType = type == "video" ? "video" : "image";
        var payload = _cloudinaryService.GetSignedUploadResponse(resourceType);
        _logger.LogInformation("Generated {ResourceType} upload signature", resourceType);
        return Ok(payload);
    }
}

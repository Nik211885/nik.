using backend.Pipes.Filter;
using backend.Services.Internals;
using backend.ViewModels;
using backend.ViewModels.ContentTranslation.Requests;
using backend.ViewModels.ContentTranslation.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

/// <summary>Endpoints for reading, writing, and monitoring content translations.</summary>
[ApiController]
[Route("api/content-translations")]
public class ContentTranslationController : ControllerBase
{
    private readonly ILogger<ContentTranslationController> _logger;
    private readonly ContentTranslationService _translationService;

    /// <summary>Initialises the controller with required dependencies.</summary>
    public ContentTranslationController(
        ILogger<ContentTranslationController> logger,
        ContentTranslationService translationService)
    {
        _logger = logger;
        _translationService = translationService;
    }

    /// <summary>
    /// Returns a paginated list of entities with their translation status for the admin UI.
    /// Supports filtering by entity type, language, and translated/untranslated state.
    /// </summary>
    [HttpGet("status")]
    public async Task<ActionResult<PaginationItem<TranslationStatusItem>>> GetStatus(
        [FromQuery] string entityType,
        [FromQuery] string langCode,
        [FromQuery] bool? translated,
        [FromQuery] PaginationRequest request)
    {
        var result = await _translationService.GetStatusListAsync(entityType, langCode, translated, request);
        return Ok(result);
    }

    /// <summary>
    /// Returns all translated fields for a single entity in the requested language.
    /// Returns an empty <c>Fields</c> dictionary when no translations exist yet.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<EntityTranslationResponse>> GetEntityTranslation(
        [FromQuery] string entityType,
        [FromQuery] string entityId,
        [FromQuery] string langCode)
    {
        var fields = await _translationService.GetAsync(entityType, entityId, langCode);
        return Ok(new EntityTranslationResponse { Fields = fields });
    }

    /// <summary>
    /// Returns the original (Vietnamese) field values for an entity so the translation editor
    /// can display them alongside the translation input fields.
    /// </summary>
    [HttpGet("source/{entityType}/{entityId}")]
    public async Task<ActionResult<EntityTranslationResponse>> GetSource(string entityType, string entityId)
    {
        var fields = await _translationService.GetSourceAsync(entityType, entityId);
        return Ok(new EntityTranslationResponse { Fields = fields });
    }

    /// <summary>
    /// Creates or updates translated field values for an entity.
    /// Fields with empty or whitespace values are silently skipped.
    /// </summary>
    [HttpPost("upsert")]
    [Authorize]
    [ValidationFilter(typeof(UpsertTranslationRequest))]
    public async Task<ActionResult> Upsert([FromBody] UpsertTranslationRequest request)
    {
        await _translationService.UpsertAsync(
            request.EntityType, request.EntityId, request.LangCode, request.Fields);
        return Ok();
    }
}

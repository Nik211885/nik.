using backend.Services;
using backend.Services.Internals;

namespace backend.Pipes.Middlewares;

/// <summary>
/// Middleware that reads the <c>lang</c> query parameter on every request and loads
/// the corresponding translation dictionary. Translations are available for downstream
/// response-shaping logic (e.g. translating i18n message keys in error responses).
/// </summary>
public class TranslatedHandlingMiddleware : IMiddleware
{
    private readonly ILogger<TranslatedHandlingMiddleware> _logger;
    private readonly LanguageServices _languageServices;

    /// <summary>Initialises the middleware with required dependencies.</summary>
    public TranslatedHandlingMiddleware(ILogger<TranslatedHandlingMiddleware> logger, LanguageServices languageServices)
    {
        _logger = logger;
        _languageServices = languageServices;
    }

    /// <inheritdoc/>
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var lang = context.Request.Query["lang"].FirstOrDefault() ?? "vi";
        var translatedLanguage = await _languageServices.GetTranslatesAsync(lang);
        var translate = translatedLanguage?.Translations ?? [];
        await next(context);
    }
}

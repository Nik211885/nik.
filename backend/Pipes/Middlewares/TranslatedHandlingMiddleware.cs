using backend.Services;
using backend.Services.Internals;

namespace backend.Pipes.Middlewares;

public class TranslatedHandlingMiddleware : IMiddleware
{
    private readonly ILogger<TranslatedHandlingMiddleware> _logger;
    private readonly LanguageServices _languageServices;

    public TranslatedHandlingMiddleware(ILogger<TranslatedHandlingMiddleware> logger, LanguageServices languageServices)
    {
        _logger = logger;
        _languageServices = languageServices;
    }
    // you can use cache in services and defined format for response to translate key in response
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var lang = context.Request.Query["lang"].FirstOrDefault() ?? "vi";
        var translatedLanguage = await _languageServices.GetTranslatesAsync(lang);
        var translate = translatedLanguage?.Translations ?? [];
        await next(context);
    }
}

using System.Text.Json.Serialization;

namespace backend.Services.Internals;

/// <summary>Translates free-form text via the MyMemory public API.</summary>
public class AutoTranslateService(IHttpClientFactory httpFactory, ILogger<AutoTranslateService> logger)
{
    /// <summary>Translates text to the target language; returns original on failure.</summary>
    /// <param name="text">Text to translate.</param>
    /// <param name="to">Target language code, e.g. <c>vi</c>.</param>
    /// <param name="from">Source language code.</param>
    /// <returns>Translated text, or original text if the service is unavailable.</returns>
    public async Task<string> TranslateAsync(string text, string to, string from = "vi")
    {
        try
        {
            var client = httpFactory.CreateClient("mymemory");
            var encoded = Uri.EscapeDataString(text);
            var response = await client.GetAsync($"/get?q={encoded}&langpair={from}|{to}");

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<MyMemoryResponse>();
            return result?.ResponseData?.TranslatedText ?? text;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Auto-translate failed (text: {Snippet})", text[..Math.Min(50, text.Length)]);
            return text;
        }
    }
}

file class MyMemoryResponse
{
    [JsonPropertyName("responseData")]
    public MyMemoryData? ResponseData { get; set; }
}

file class MyMemoryData
{
    [JsonPropertyName("translatedText")]
    public string? TranslatedText { get; set; }
}

namespace backend.ViewModels.Languages.Responses;

public class TranslateLanguageResponse
{
    public string Language { get; set; }
    public IReadOnlyCollection<TranslateResponse> Translations { get; set; }
}

public class TranslateResponse
{
    public string Code { get; set; }
    public string Value { get; set; }
}

namespace backend.ViewModels.Languages.Requests;

public class TranslateCodeLanguageRequest
{
    public string CodeDefined { get; set; }
    public List<TranslateRequest>  Translates { get; set; }
}

public class TranslateRequest
{
    public string Language { get; set; }
    public string Value { get; set; }
}

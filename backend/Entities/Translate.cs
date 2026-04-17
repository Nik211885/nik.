namespace backend.Entities;

public class Translate : BaseEntity
{
    public string CodeId { get; set; }
    public string LanguageId { get; set; }
    public string Value { get; set; }
    
    public CodeLanguage CodeLanguage { get; set; }
    public Language Language { get; set; }
}

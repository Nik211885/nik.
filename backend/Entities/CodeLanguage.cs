namespace backend.Entities;

public class CodeLanguage
{
    public string Id { get; set; }
    public string Code { get; set; }
    
    public ICollection<Translate> Translates { get; set; }
}

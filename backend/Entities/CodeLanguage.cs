namespace backend.Entities;

public class CodeLanguage : BaseEntity
{
    public string Code { get; set; }
    public ICollection<Translate> Translates { get; set; }
}

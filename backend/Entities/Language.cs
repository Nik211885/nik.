namespace backend.Entities;

public class Language : BaseEntity
{
    public string Code { get; set; }
    public string Name { get; set; }
    
    public ICollection<Translate> Translates { get; set; }
}

namespace backend.Entities;

public class Language
{
    public string Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    
    public ICollection<Translate> Translates { get; set; }
}

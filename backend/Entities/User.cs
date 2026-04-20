namespace backend.Entities;

public class User : BaseEntity
{
    public string UserName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string Password { get; set; }
    public string Bio { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public string Slug { get; set; }
}

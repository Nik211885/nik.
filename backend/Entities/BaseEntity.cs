namespace backend.Entities;

public abstract class BaseEntity
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
}

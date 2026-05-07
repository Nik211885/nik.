namespace backend.Entities;

/// <summary>
/// Base class for all domain entities. Provides a UUIDv7-based string identifier
/// that is sortable by creation time.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Unique identifier generated as a UUIDv7 string. Sortable by insertion order.
    /// </summary>
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
}

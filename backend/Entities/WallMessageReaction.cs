namespace backend.Entities;

/// <summary>An anonymous visitor reaction on a wall message, identified by device.</summary>
public class WallMessageReaction : BaseEntity
{
    /// <summary>ID of the wall message being reacted to.</summary>
    public string WallMessageId { get; set; }

    /// <summary>Browser-generated UUID stored in the visitor's localStorage.</summary>
    public string DeviceId { get; set; }

    /// <summary>UTC timestamp when the reaction was created.</summary>
    public DateTimeOffset CreatedDate { get; set; }

    /// <summary>Navigation property to the parent wall message.</summary>
    public WallMessage WallMessage { get; set; }
}

namespace backend.Entities;

/// <summary>A short message left by a visitor on the public wall page.</summary>
public class WallMessage : BaseEntity
{
    /// <summary>Display name provided by the visitor.</summary>
    public string Name { get; set; }

    /// <summary>Short message body (max 300 chars).</summary>
    public string Message { get; set; }

    /// <summary>Moderation status assigned by Claude or an admin.</summary>
    public WallMessageStatus Status { get; set; }

    /// <summary>Optional attribution — the source or author the visitor is quoting.</summary>
    public string? Source { get; set; }

    /// <summary>IP address of the submitter, stored for audit purposes.</summary>
    public string? IpAddress { get; set; }

    /// <summary>UTC timestamp when the message was submitted.</summary>
    public DateTimeOffset CreatedDate { get; set; }
}

/// <summary>Moderation state of a wall message.</summary>
public enum WallMessageStatus
{
    /// <summary>Awaiting admin review.</summary>
    Pending = 0,

    /// <summary>Cleared by auto-moderation or admin — visible on the public wall.</summary>
    Approved = 1,

    /// <summary>Rejected by auto-moderation or admin — not shown publicly.</summary>
    Rejected = 2,
}

namespace backend.Entities;

/// <summary>
/// Stores a contact form submission from the public site.
/// </summary>
public class Contact : BaseEntity
{
    /// <summary>Full name supplied by the visitor.</summary>
    public string Name { get; set; }

    /// <summary>Email address supplied by the visitor.</summary>
    public string Email { get; set; }

    /// <summary>Subject line of the enquiry.</summary>
    public string Subject { get; set; }

    /// <summary>Body of the message.</summary>
    public string Message { get; set; }

    /// <summary>UTC timestamp when the submission was received.</summary>
    public DateTimeOffset CreatedDate { get; set; }

    /// <summary>Indicates whether an admin has opened/read this submission.</summary>
    public bool IsRead { get; set; }
}

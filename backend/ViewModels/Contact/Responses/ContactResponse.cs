namespace backend.ViewModels.Contact.Responses;

/// <summary>Read model for a contact form submission.</summary>
public class ContactResponse
{
    /// <summary>Unique identifier of the submission.</summary>
    public string Id { get; set; }

    /// <summary>Full name of the sender.</summary>
    public string Name { get; set; }

    /// <summary>Email address of the sender.</summary>
    public string Email { get; set; }

    /// <summary>Subject of the enquiry.</summary>
    public string Subject { get; set; }

    /// <summary>Body of the message.</summary>
    public string Message { get; set; }

    /// <summary>UTC timestamp when the submission was received.</summary>
    public DateTimeOffset CreatedDate { get; set; }

    /// <summary>Whether an admin has read this submission.</summary>
    public bool IsRead { get; set; }
}

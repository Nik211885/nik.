namespace backend.ViewModels.Auth.Responses;

/// <summary>Authenticated user's profile returned by <c>GET /api/auth/profile</c>.</summary>
public class UserProfileResponse
{
    /// <summary>User ID.</summary>
    public string Id { get; set; }

    /// <summary>User's email address.</summary>
    public string? Email { get; set; }

    /// <summary>Display name / login handle.</summary>
    public string UserName { get; set; }

    /// <summary>Assigned roles. Empty list for regular users.</summary>
    public List<string> Roles { get; set; } = [];
}

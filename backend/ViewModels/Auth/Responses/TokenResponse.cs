namespace backend.ViewModels.Auth.Responses;

/// <summary>Token pair returned on successful login, register, or token refresh.</summary>
public class TokenResponse
{
    /// <summary>Short-lived JWT access token used to authenticate API requests.</summary>
    public string AccessToken { get; set; }

    /// <summary>Long-lived opaque token used to obtain a new access token.</summary>
    public string RefreshToken { get; set; }
}

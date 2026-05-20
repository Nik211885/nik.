namespace backend.Exceptions;

/// <summary>
/// Thrown when a request carries an Authorization header whose token is absent, expired, or invalid.
/// Translated to HTTP 401 Unauthorized by the exception-handling middleware.
/// </summary>
/// <param name="mss">Optional i18n message key to override the default <see cref="ApplicationMessage.Unauthorized"/> key.</param>
public sealed class UnauthorizedException(string? mss = null)
    : Exception(mss ?? ApplicationMessage.Unauthorized);

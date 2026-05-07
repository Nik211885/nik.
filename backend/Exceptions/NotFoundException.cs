namespace backend.Exceptions;

/// <summary>
/// Thrown when a requested resource cannot be found in the database.
/// Translated to HTTP 404 Not Found by the exception-handling middleware.
/// </summary>
/// <param name="mss">
/// Optional i18n message key to override the default <see cref="ApplicationMessage.NotFound"/> key.
/// </param>
public sealed class NotFoundException(string? mss = null)
    : Exception(mss ?? ApplicationMessage.NotFound);

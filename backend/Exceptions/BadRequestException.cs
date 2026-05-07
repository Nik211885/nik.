namespace backend.Exceptions;
using backend;

/// <summary>
/// Thrown when a request violates a business rule (e.g. duplicate name, invalid state).
/// Translated to HTTP 400 Bad Request by the exception-handling middleware.
/// </summary>
/// <param name="mss">
/// Optional i18n message key to override the default <see cref="ApplicationMessage.BadRequest"/> key.
/// </param>
public sealed class BadRequestException(string? mss = null)
    : Exception(mss ?? ApplicationMessage.BadRequest);

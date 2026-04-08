namespace backend.Exceptions;
using backend;

public sealed class BadRequestException(string? mss = null)
    : Exception(mss ?? ApplicationMessage.BadRequest);

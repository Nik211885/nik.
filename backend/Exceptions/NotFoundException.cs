namespace backend.Exceptions;

public sealed class NotFoundException(string? mss  = null) 
    : Exception(mss ?? ApplicationMessage.NotFound);

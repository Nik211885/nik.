using System.Security.Claims;

namespace backend.Extensions;

/// <summary>
/// Extension members for <see cref="HttpContext"/> providing JWT claim helpers.
/// </summary>
public static class HttpContextExtensions
{
    extension(HttpContext context)
    {
        /// <summary>
        /// Extracts the authenticated user's ID from the JWT
        /// <see cref="ClaimTypes.NameIdentifier"/> claim.
        /// Returns <see cref="Guid.Empty"/> as a string when the claim is absent
        /// (i.e. the request is unauthenticated).
        /// </summary>
        /// <returns>The user ID string, or <c>"00000000-0000-0000-0000-000000000000"</c>.</returns>
        public string GetUserId()
        {
            return context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                ?? Guid.Empty.ToString();
        }
    }
}

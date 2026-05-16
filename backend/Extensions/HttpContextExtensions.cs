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

        /// <summary>
        /// Derives the base BCP-47 language code from the request's <c>Accept-Language</c> header.
        /// Falls back to <c>"vi"</c> when the header is absent or empty.
        /// </summary>
        /// <returns>Lowercase two-letter language code, e.g. <c>"vi"</c> or <c>"en"</c>.</returns>
        public string GetLanguage()
        {
            var raw = context.Request.Headers["Accept-Language"]
                             .FirstOrDefault()?.Split(',')[0].Trim() ?? "vi";
            return raw.Split('-')[0].ToLowerInvariant();
        }

        /// <summary>
        /// Returns <see langword="true"/> when the current request carries a valid authenticated identity
        /// (i.e. the user is logged in as an admin).
        /// </summary>
        public bool IsAdmin() => context.User.Identity?.IsAuthenticated ?? false;
    }
}

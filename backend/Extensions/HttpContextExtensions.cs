using System.Security.Claims;

namespace backend.Extensions;
public static class HttpContextExtensions
{
    extension(HttpContext context)
    {
        public string GetUserId()
        {
            return context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value 
                ?? Guid.Empty.ToString();
        }
    }
}

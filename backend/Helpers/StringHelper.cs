using System.Security.Cryptography;

namespace backend.Helpers;

/// <summary>
/// Static utility methods for string generation.
/// </summary>
public static class StringHelper
{
    /// <summary>
    /// Generates a cryptographically random Base64 string derived from the
    /// specified number of random bytes.
    /// </summary>
    /// <param name="bytes">Number of random bytes to generate before Base64 encoding.</param>
    /// <returns>Base64-encoded string of the random bytes.</returns>
    public static string GeneratorRandomStringBase64(int bytes)
    {
        var rand = new byte[bytes];
        using (var random = RandomNumberGenerator.Create())
        {
            random.GetBytes(rand);
        }
        return Convert.ToBase64String(rand);
    }
}

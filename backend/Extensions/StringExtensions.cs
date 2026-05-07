using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using backend.Helpers;

namespace backend.Extensions;

/// <summary>
/// Extension members for <see cref="string"/> providing slug generation utilities.
/// </summary>
public static class StringExtensions
{
    extension(string str)
    {
        /// <summary>
        /// Converts a string to a URL-friendly slug in the format
        /// <c>{6-char-random}/{normalised-text}</c>.
        /// <para>
        /// The random prefix ensures uniqueness even for identical titles while
        /// keeping the human-readable segment for SEO purposes.
        /// </para>
        /// </summary>
        /// <returns>
        /// A slug string such as <c>ab3x7k/my-article-title</c>.
        /// </returns>
        public string ToSlug()
        {
            string first = StringHelper.GeneratorRandomStringBase64(6)
                .Replace("/", "")
                .Replace("+", "")
                .TrimEnd('=').ToLowerInvariant();

            var normalized = str.Normalize(NormalizationForm.FormD);
            var chars = normalized.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);
            string second = new string(chars.ToArray());

            second = second.Normalize(NormalizationForm.FormC).ToLowerInvariant();
            second = Regex.Replace(second, @"[^a-z0-9\s-]", "");
            second = Regex.Replace(second, @"\s+", "-").Trim('-');
            return $"{first}/{second}";
        }
    }
}

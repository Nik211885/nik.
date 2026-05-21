using System.Net.Http.Json;
using System.Text.Json;
using backend.Entities;

namespace backend.Services.Extends;

/// <summary>Calls the Claude API to auto-moderate user-submitted wall messages.</summary>
public class ClaudeModeration
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ClaudeModeration> _logger;
    private readonly IConfiguration _configuration;

    /// <summary>Initialises the service with required dependencies.</summary>
    public ClaudeModeration(
        IHttpClientFactory httpClientFactory,
        ILogger<ClaudeModeration> logger,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Sends the name and message to Claude for moderation.
    /// Returns <see cref="WallMessageStatus.Pending"/> when the API key is missing or the call fails.
    /// </summary>
    /// <param name="name">Visitor display name.</param>
    /// <param name="message">Message body to evaluate.</param>
    /// <returns>The moderation verdict.</returns>
    public async Task<WallMessageStatus> ModerateAsync(string name, string message, string? source = null)
    {
        var apiKey = _configuration["Anthropic:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogWarning("Anthropic API key not configured — wall message defaulting to Pending.");
            return WallMessageStatus.Pending;
        }

        try
        {
            var client = _httpClientFactory.CreateClient("claude");
            client.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", apiKey);
            client.DefaultRequestHeaders.TryAddWithoutValidation("anthropic-version", "2023-06-01");

            var prompt = $"""
                Bạn là người kiểm duyệt nội dung cho một blog cá nhân tiếng Việt.
                Đánh giá tin nhắn ngắn từ khách thăm và phân loại thành MỘT trong ba từ:
                - approved: Bình thường, thân thiện, trung lập, hoặc tiếng lóng nhẹ nhàng
                - rejected: Thù địch, kích động, nội dung 18+, spam, hoặc xúc phạm nghiêm trọng
                - pending: Ranh giới không rõ ràng, cần người duyệt

                Chỉ trả về đúng một trong ba từ trên, không thêm gì khác.

                Tên: {name}
                Tin nhắn: {message}
                {(source is not null ? $"Trích dẫn từ: {source}" : "")}
                """;

            var requestBody = new
            {
                model = "claude-haiku-4-5-20251001",
                max_tokens = 10,
                messages = new[] { new { role = "user", content = prompt } },
            };

            var response = await client.PostAsJsonAsync("/v1/messages", requestBody);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Claude API returned {Status} — defaulting to Pending.", response.StatusCode);
                return WallMessageStatus.Pending;
            }

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            var verdict = json.GetProperty("content")[0].GetProperty("text").GetString()?.Trim().ToLowerInvariant();

            return verdict switch
            {
                "approved" => WallMessageStatus.Approved,
                "rejected" => WallMessageStatus.Rejected,
                _          => WallMessageStatus.Pending,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Claude moderation failed — defaulting to Pending.");
            return WallMessageStatus.Pending;
        }
    }
}

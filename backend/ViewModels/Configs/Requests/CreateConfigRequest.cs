using System.Text.Json;

namespace backend.ViewModels.Configs.Requests;

/// <summary>Request model for creating or updating a system configuration entry.</summary>
public class CreateConfigRequest
{
    /// <summary>Unique lowercase dot-separated config key (e.g. <c>site.title</c>).</summary>
    public string Key { get; set; }

    /// <summary>Arbitrary JSON value to store. Supports any valid JSON shape.</summary>
    public JsonDocument Value { get; set; }
}

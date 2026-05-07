using System.Text.Json;

namespace backend.ViewModels.Configs.Responses;

/// <summary>System configuration entry response returned by config endpoints.</summary>
public class ConfigResponse
{
    /// <summary>Config entry ID.</summary>
    public string Id { get; set; }

    /// <summary>Unique lowercase config key (e.g. <c>site.title</c>).</summary>
    public string Key { get; set; }

    /// <summary>The stored JSON value.</summary>
    public JsonDocument Value { get; set; }
}

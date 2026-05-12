using System.Text.Json;

namespace backend.ViewModels.Configs.Requests;

/// <summary>Request model for updating an existing system configuration entry by ID.</summary>
public class UpdateConfigRequest
{
    /// <summary>ID of the configuration entry to update.</summary>
    public string Id { get; set; }

    /// <summary>New unique lowercase dot-separated config key (e.g. <c>site.title</c>).</summary>
    public string Key { get; set; }

    /// <summary>New arbitrary JSON value to store.</summary>
    public JsonDocument Value { get; set; }
}

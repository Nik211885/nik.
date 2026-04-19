using System.Text.Json;

namespace backend.ViewModels.Configs.Responses;

public class ConfigResponse
{
    public string Id { get; set; }
    public string Key { get; set; }
    public JsonDocument Value  { get; set; }
}

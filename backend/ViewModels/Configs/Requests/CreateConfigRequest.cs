using System.Text.Json;

namespace backend.ViewModels.Configs.Requests;

public class CreateConfigRequest
{
    public string Key { get; set; }
    public JsonDocument Value { get; set; }
}

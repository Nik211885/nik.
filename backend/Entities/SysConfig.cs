using System.Text.Json;

namespace backend.Entities;

public class SysConfig : BaseEntity
{
    public string Key { get; set; }
    public Dictionary<string, JsonElement> Value { get; set; }
}

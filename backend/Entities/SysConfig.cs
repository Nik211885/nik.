using System.Text.Json;

namespace backend.Entities;

public class SysConfig : BaseEntity
{
    public string Key { get; set; }
    public JsonDocument Value { get; set; }
    public T? GetValue<T>() => Value.Deserialize<T>();
}

using System.Text.Json;

namespace backend.Entities;

/// <summary>
/// Represents a generic key-value system configuration entry.
/// The value is stored as a <see cref="JsonDocument"/> to support arbitrary JSON shapes.
/// </summary>
public class SysConfig : BaseEntity
{
    /// <summary>Unique lowercase dot-separated config key (e.g. <c>site.title</c>).</summary>
    public string Key { get; set; }

    /// <summary>Arbitrary JSON value associated with the key.</summary>
    public JsonDocument Value { get; set; }

    /// <summary>
    /// Deserialises the JSON <see cref="Value"/> into the specified type.
    /// Returns <see langword="null"/> if deserialisation fails or the value is absent.
    /// </summary>
    /// <typeparam name="T">Target type to deserialise into.</typeparam>
    /// <returns>Deserialised value, or <see langword="null"/>.</returns>
    public T? GetValue<T>() => Value.Deserialize<T>();
}

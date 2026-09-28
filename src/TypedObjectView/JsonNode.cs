using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ObjectViews;

internal sealed class JsonNode(JsonElement value, JsonSerializerOptions options) : IViewNode
{
    public TResult Get<TResult>(MemberPath path)
    {
        var element = Traverse(path);

        try
        {
            return element.Deserialize<TResult>(options)!;
        }
        catch (Exception exception) when (exception is JsonException or NotSupportedException)
        {
            throw new ObjectViewException(
                $"JSON value at '{path.DisplayName}' cannot be read as '{typeof(TResult)}'.",
                exception);
        }
    }

    public IViewNode GetObject(MemberPath path)
    {
        var element = Traverse(path);
        if (element.ValueKind == JsonValueKind.Null)
        {
            throw new ObjectViewException($"Object at '{path.DisplayName}' is null.");
        }

        if (element.ValueKind != JsonValueKind.Object)
        {
            throw new ObjectViewException($"JSON value at '{path.DisplayName}' is not an object.");
        }

        return new JsonNode(element, options);
    }

    public IEnumerable<IViewNode> GetArray(MemberPath path)
    {
        var element = Traverse(path);
        if (element.ValueKind == JsonValueKind.Null)
        {
            throw new ObjectViewException($"Array at '{path.DisplayName}' is null.");
        }

        if (element.ValueKind != JsonValueKind.Array)
        {
            throw new ObjectViewException($"JSON value at '{path.DisplayName}' is not an array.");
        }

        return Enumerate(element, options, path.DisplayName);
    }

    private JsonElement Traverse(MemberPath path)
    {
        var current = value;

        foreach (var member in path.Members)
        {
            if (current.ValueKind != JsonValueKind.Object)
            {
                throw new ObjectViewException(
                    $"Cannot read '{path.DisplayName}': the parent of '{member.Name}' is not a JSON object.");
            }

            var jsonName = GetJsonName(member, options);
            if (!TryGetProperty(current, jsonName, options.PropertyNameCaseInsensitive, out current))
            {
                throw new ObjectViewException(
                    $"JSON property '{jsonName}' for '{path.DisplayName}' was not found.");
            }
        }

        return current;
    }

    private static string GetJsonName(MemberInfo member, JsonSerializerOptions options)
    {
        var attribute = member.GetCustomAttribute<JsonPropertyNameAttribute>();
        return attribute?.Name
            ?? options.PropertyNamingPolicy?.ConvertName(member.Name)
            ?? member.Name;
    }

    private static bool TryGetProperty(
        JsonElement element,
        string name,
        bool caseInsensitive,
        out JsonElement result)
    {
        if (element.TryGetProperty(name, out result))
        {
            return true;
        }

        if (caseInsensitive)
        {
            foreach (var property in element.EnumerateObject())
            {
                if (string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    result = property.Value;
                    return true;
                }
            }
        }

        result = default;
        return false;
    }

    private static IEnumerable<IViewNode> Enumerate(
        JsonElement array,
        JsonSerializerOptions serializerOptions,
        string path)
    {
        foreach (var item in array.EnumerateArray())
        {
            if (item.ValueKind == JsonValueKind.Null)
            {
                throw new ObjectViewException($"Array at '{path}' contains a null item.");
            }

            yield return new JsonNode(item, serializerOptions);
        }
    }
}

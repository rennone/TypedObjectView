using System.Text.Json;

namespace ObjectViews;

/// <summary>Creates object views backed by CLR instances or JSON.</summary>
public static class ObjectView
{
    /// <summary>Creates a view over an existing CLR instance.</summary>
    public static IObjectView<T> FromObject<T>(T value)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(value);
        return new TypedObjectView<T>(new ClrNode(value));
    }

    /// <summary>
    /// Creates a view over a <see cref="JsonElement"/>. The caller must keep the
    /// element's owning <see cref="JsonDocument"/> alive while using the view.
    /// </summary>
    public static IObjectView<T> FromJsonElement<T>(
        JsonElement value,
        JsonSerializerOptions? options = null)
        => new TypedObjectView<T>(new JsonNode(value, options ?? JsonSerializerOptions.Default));

    /// <summary>Parses JSON and creates a view whose backing data is independently owned.</summary>
    public static IObjectView<T> FromJson<T>(
        string json,
        JsonSerializerOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(json);
        return FromOwnedJson<T>(json, options);
    }

    /// <summary>
    /// Creates a view over a database JSON column or another deferred JSON source.
    /// The loader is invoked at most once, on the first data access.
    /// </summary>
    public static IObjectView<T> FromJsonColumn<T>(
        Func<string> loadJson,
        JsonSerializerOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(loadJson);
        return new TypedObjectView<T>(
            new DeferredNode(() => CreateOwnedJsonNode(loadJson(), options)));
    }

    /// <summary>
    /// Asynchronously loads a database JSON column and creates a view over it.
    /// Use this at the asynchronous data-access boundary; subsequent view access is synchronous.
    /// </summary>
    public static async ValueTask<IObjectView<T>> FromJsonColumnAsync<T>(
        Func<CancellationToken, ValueTask<string>> loadJson,
        JsonSerializerOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(loadJson);
        var json = await loadJson(cancellationToken).ConfigureAwait(false);
        return FromOwnedJson<T>(json, options);
    }

    private static IObjectView<T> FromOwnedJson<T>(string json, JsonSerializerOptions? options)
        => new TypedObjectView<T>(CreateOwnedJsonNode(json, options));

    private static JsonNode CreateOwnedJsonNode(string json, JsonSerializerOptions? options)
    {
        ArgumentNullException.ThrowIfNull(json);

        try
        {
            using var document = JsonDocument.Parse(json);
            return new JsonNode(document.RootElement.Clone(), options ?? JsonSerializerOptions.Default);
        }
        catch (JsonException exception)
        {
            throw new ObjectViewException("The JSON source could not be parsed.", exception);
        }
    }
}

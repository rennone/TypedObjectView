using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ObjectViews;

var data = new MyData(
    new Size(1024, 512),
    10,
    "main.png",
    [new Lod("small.png", new Size(256, 128)), new Lod("tiny.png", new Size(64, 32))],
    ["linear", "repeat"]);

var expected = "Size 1024,512" + Environment.NewLine
    + "small.png : 256, 128" + Environment.NewLine
    + "tiny.png : 64, 32" + Environment.NewLine;

Equal(expected, GetDescription(ObjectView.FromObject(data)), "CLR view");

var webOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
var json = JsonSerializer.Serialize(data, webOptions);
Equal(expected, GetDescription(ObjectView.FromJson<MyData>(json, webOptions)), "JSON text view");

using (var document = JsonDocument.Parse(json))
{
    Equal(
        expected,
        GetDescription(ObjectView.FromJsonElement<MyData>(document.RootElement, webOptions)),
        "JsonElement view");
}

var loadCount = 0;
var databaseView = ObjectView.FromJsonColumn<MyData>(() =>
{
    loadCount++;
    return json;
}, webOptions);
Equal(0, loadCount, "deferred database load");
Equal(expected, GetDescription(databaseView), "database JSON column view");
Equal(1, loadCount, "single database load");

var asyncView = await ObjectView.FromJsonColumnAsync<MyData>(
    _ => ValueTask.FromResult(json),
    webOptions);
Equal(expected, GetDescription(asyncView), "async database JSON column view");

var renamedJson = """
    {"dimensions":{"width":3,"height":4}}
    """;
var renamed = ObjectView.FromJson<RenamedData>(renamedJson, webOptions);
Equal(3, renamed.Get(x => x.TextureSize.Width), "JsonPropertyName path");

Throws<ObjectViewException>(
    () => ObjectView.FromJson<MyData>("{}").Get(x => x.Priority),
    "missing JSON member");
Throws<ObjectViewException>(
    () => ObjectView.FromObject(data).Get(x => x.Priority + 1),
    "computed selector rejection");

Console.WriteLine("All TypedObjectView tests passed.");

static string GetDescription(IObjectView<MyData> data)
{
    var builder = new StringBuilder();
    var size = data.Object(x => x.TextureSize);
    builder.AppendLine($"Size {size.Get(x => x.Width)},{size.Get(x => x.Height)}");

    foreach (var lod in data.Array(x => x.Lods))
    {
        builder.AppendLine(
            $"{lod.Get(x => x.Path)} : "
            + $"{lod.Get(x => x.TextureSize.Width)}, {lod.Get(x => x.TextureSize.Height)}");
    }

    return builder.ToString();
}

static void Equal<T>(T expectedValue, T actualValue, string name)
{
    if (!EqualityComparer<T>.Default.Equals(expectedValue, actualValue))
    {
        throw new InvalidOperationException(
            $"{name} failed. Expected '{expectedValue}', actual '{actualValue}'.");
    }
}

static void Throws<TException>(Action action, string name)
    where TException : Exception
{
    try
    {
        action();
    }
    catch (TException)
    {
        return;
    }

    throw new InvalidOperationException($"{name} failed: {typeof(TException).Name} was not thrown.");
}

internal sealed record MyData(
    Size TextureSize,
    int Priority,
    string Path,
    Lod[] Lods,
    string[] OtherSettings);

internal sealed record Size(int Width, int Height);

internal sealed record Lod(string Path, Size TextureSize);

internal sealed record RenamedData(
    [property: JsonPropertyName("dimensions")] Size TextureSize);

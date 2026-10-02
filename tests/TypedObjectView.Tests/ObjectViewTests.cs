using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ObjectViews;

namespace TypedObjectView.Tests;

public sealed class ObjectViewTests
{
    [Fact]
    public void FromObject_ExposesNestedObjectAndArrayValues()
    {
        var view = ObjectView.FromObject(CreateData());

        Assert.Equal(ExpectedDescription, GetDescription(view));
    }

    [Fact]
    public void FromJson_ExposesNestedObjectAndArrayValues()
    {
        var options = CreateWebOptions();
        var json = JsonSerializer.Serialize(CreateData(), options);

        var view = ObjectView.FromJson<MyData>(json, options);

        Assert.Equal(ExpectedDescription, GetDescription(view));
    }

    [Fact]
    public void FromJsonElement_ExposesNestedObjectAndArrayValues()
    {
        var options = CreateWebOptions();
        var json = JsonSerializer.Serialize(CreateData(), options);
        using var document = JsonDocument.Parse(json);

        var view = ObjectView.FromJsonElement<MyData>(document.RootElement, options);

        Assert.Equal(ExpectedDescription, GetDescription(view));
    }

    [Fact]
    public void FromJsonColumn_DefersAndCachesLoading()
    {
        var options = CreateWebOptions();
        var json = JsonSerializer.Serialize(CreateData(), options);
        var loadCount = 0;
        var view = ObjectView.FromJsonColumn<MyData>(() =>
        {
            loadCount++;
            return json;
        }, options);

        Assert.Equal(0, loadCount);
        Assert.Equal(ExpectedDescription, GetDescription(view));
        Assert.Equal(1, loadCount);
    }

    [Fact]
    public async Task FromJsonColumnAsync_LoadsAndExposesValues()
    {
        var options = CreateWebOptions();
        var json = JsonSerializer.Serialize(CreateData(), options);

        var view = await ObjectView.FromJsonColumnAsync<MyData>(
            _ => ValueTask.FromResult(json),
            options,
            TestContext.Current.CancellationToken);

        Assert.Equal(ExpectedDescription, GetDescription(view));
    }

    [Fact]
    public void FromJson_UsesJsonPropertyNameAttribute()
    {
        const string json = """
            {"dimensions":{"width":3,"height":4}}
            """;
        var view = ObjectView.FromJson<RenamedData>(json, CreateWebOptions());

        Assert.Equal(3, view.Get(x => x.TextureSize.Width));
    }

    [Fact]
    public void Get_WhenJsonMemberIsMissing_ThrowsObjectViewException()
    {
        Assert.Throws<ObjectViewException>(
            () => ObjectView.FromJson<MyData>("{}").Get(x => x.Priority));
    }

    [Fact]
    public void Get_WhenSelectorIsComputed_ThrowsObjectViewException()
    {
        var view = ObjectView.FromObject(CreateData());

        Assert.Throws<ObjectViewException>(() => view.Get(x => x.Priority + 1));
    }

    private static MyData CreateData() => new(
        new Size(1024, 512),
        10,
        "main.png",
        [new Lod("small.png", new Size(256, 128)), new Lod("tiny.png", new Size(64, 32))],
        ["linear", "repeat"]);

    private static JsonSerializerOptions CreateWebOptions() =>
        new(JsonSerializerDefaults.Web);

    private static string ExpectedDescription =>
        "Size 1024,512" + Environment.NewLine
        + "small.png : 256, 128" + Environment.NewLine
        + "tiny.png : 64, 32" + Environment.NewLine;

    private static string GetDescription(IObjectView<MyData> data)
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

    private sealed record MyData(
        Size TextureSize,
        int Priority,
        string Path,
        Lod[] Lods,
        string[] OtherSettings);

    private sealed record Size(int Width, int Height);

    private sealed record Lod(string Path, Size TextureSize);

    private sealed record RenamedData(
        [property: JsonPropertyName("dimensions")] Size TextureSize);
}

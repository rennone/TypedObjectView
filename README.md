# TypedObjectView

`TypedObjectView` は、同じ型情報を持つ CLR インスタンス、`JsonElement`、DB の JSON カラムを、
`IObjectView<T>` という共通の読み取り専用 API で扱うための小さな .NET ライブラリです。

JSON ビューはオブジェクト全体を `T` にデシリアライズしません。アクセスされた値だけを読み取り、
`Object` と `Array` は JSON の該当部分を参照する子ビューを返します。

## 基本的な使い方

```csharp
using ObjectViews;

record MyData(Size TextureSize, int Priority, string Path, Lod[] Lods, string[] OtherSettings);
record Size(int Width, int Height);
record Lod(string Path, Size TextureSize);

string GetDescription(IObjectView<MyData> data)
{
    StringBuilder sb = new();
    var size = data.Object(x => x.TextureSize);
    sb.AppendLine($"Size {size.Get(x => x.Width)},{size.Get(x => x.Height)}");

    foreach (var lod in data.Array(x => x.Lods))
    {
        sb.AppendLine(
            $"{lod.Get(x => x.Path)} : " +
            $"{lod.Get(x => x.TextureSize.Width)}, {lod.Get(x => x.TextureSize.Height)}");
    }

    return sb.ToString();
}
```

バックエンドだけを差し替えて同じ関数に渡せます。

```csharp
IObjectView<MyData> clr = ObjectView.FromObject(myData);
IObjectView<MyData> element = ObjectView.FromJsonElement<MyData>(jsonElement, jsonOptions);
IObjectView<MyData> json = ObjectView.FromJson<MyData>(jsonText, jsonOptions);

// 最初の Get/Object/Array まで loadJsonColumn は呼ばれず、その後は結果が再利用されます。
IObjectView<MyData> database = ObjectView.FromJsonColumn<MyData>(
    loadJsonColumn,
    jsonOptions);

// DB API が非同期なら、データアクセス境界で一度 await します。
IObjectView<MyData> asyncDatabase = await ObjectView.FromJsonColumnAsync<MyData>(
    cancellationToken => loadJsonColumnAsync(cancellationToken),
    jsonOptions,
    cancellationToken);
```

`JsonSerializerOptions.PropertyNamingPolicy`、`PropertyNameCaseInsensitive`、
`JsonPropertyNameAttribute` が JSON のプロパティ解決に反映されます。

## API の制約

- セレクターは `x => x.Property` や `x => x.Child.Property` のような、引数を起点とするプロパティ／フィールドのパスに限定されます。
- `x => x.Priority + 1` やメソッド呼び出しは `ObjectViewException` になります。
- `FromJsonElement` はコピーを作らないため、利用中は元の `JsonDocument` を破棄しないでください。独立した寿命が必要なら `FromJson` を利用してください。
- DB 固有の接続や ORM には依存しません。JSON カラムを返すデリゲートを渡すことで、ADO.NET、Dapper、EF Core などから利用できます。

## ビルドとテスト

```text
dotnet build TypedObjectView.sln
dotnet run --project tests/TypedObjectView.Tests/TypedObjectView.Tests.csproj --framework net9.0 --no-build
dotnet run --project tests/TypedObjectView.Tests/TypedObjectView.Tests.csproj --framework net9.0-windows --no-build
```

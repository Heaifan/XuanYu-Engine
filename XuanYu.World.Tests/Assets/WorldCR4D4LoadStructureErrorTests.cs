using XuanYu.Editor.SceneDocument;

namespace XuanYu.World.Tests.Assets;

// D4：结构错误事务（拆分自 LoadTransactionTests，5+100）。
// 非法 JSON / Schema / 引用 / 资产记录错误 → 加载失败，原场景不变。
public sealed class WorldCR4D4LoadStructureErrorTests : IDisposable
{
    readonly ScenePersistenceEnv _env = new();

    [Fact]
    public async Task Invalid_json_keeps_current_state()
    {
        var scenePath = _env.NewScenePath();
        await File.WriteAllTextAsync(scenePath, "{ not json");

        var loaded = await _env.LoadAsync(scenePath);

        Assert.False(loaded.Succeeded);
        Assert.Equal("BrokenJson", loaded.ErrorCode);
    }

    [Fact]
    public async Task Unsupported_schema_rejected()
    {
        var scenePath = _env.NewScenePath();
        await File.WriteAllTextAsync(scenePath,
            """{"format":"XuanYuScene","schemaVersion":99,"scene":{"id":"x","name":"y"},"entities":[]}""");

        var loaded = await _env.LoadAsync(scenePath);

        Assert.False(loaded.Succeeded);
        Assert.Equal("UnsupportedSchema", loaded.ErrorCode);
    }

    [Fact]
    public async Task Static_model_without_asset_id_rejected()
    {
        var scenePath = _env.NewScenePath();
        await File.WriteAllTextAsync(scenePath,
            """{"format":"XuanYuScene","schemaVersion":3,"scene":{"id":"x","name":"y"},"entities":[{"id":1,"name":"m","entityType":"StaticModel","parentId":null,"siblingOrder":0,"position":{"x":0,"y":0,"z":0},"rotation":{"x":0,"y":0,"z":0},"scale":{"x":1,"y":1,"z":1}}],"assets":[]}""");

        var loaded = await _env.LoadAsync(scenePath);

        Assert.False(loaded.Succeeded);
        Assert.Equal("MissingEntityAssetId", loaded.ErrorCode);
    }

    [Fact]
    public async Task Duplicate_asset_id_rejected()
    {
        var scenePath = _env.NewScenePath();
        var assetJson = """{"assetId":"asset_00000000000000000000000000000001","kind":"ModelGltf","relativePath":"models/x/source.glb","displayName":"a.glb","importerVersion":1}""";
        var doc = "{\"format\":\"XuanYuScene\",\"schemaVersion\":3,\"scene\":{\"id\":\"x\",\"name\":\"y\"},\"entities\":[],\"assets\":["
            + assetJson + "," + assetJson + "]}";
        await File.WriteAllTextAsync(scenePath, doc);

        var loaded = await _env.LoadAsync(scenePath);

        Assert.False(loaded.Succeeded);
        Assert.Equal("DuplicateAssetId", loaded.ErrorCode);
    }

    public void Dispose() => _env.Dispose();
}

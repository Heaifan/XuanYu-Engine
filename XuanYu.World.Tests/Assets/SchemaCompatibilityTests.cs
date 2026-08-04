using XuanYu.Editor.SceneDocument;
using XuanYu.Editor.Assets;

namespace XuanYu.World.Tests.Assets;

public sealed class SchemaCompatibilityTests : IDisposable
{
    readonly ScenePersistenceEnv _env = new();
    [Fact]
    public async Task V1_scene_loads_without_assets()
    {
        var scenePath = _env.NewScenePath();
        await File.WriteAllTextAsync(scenePath,
            """{"format":"XuanYuScene","schemaVersion":1,"scene":{"id":"x","name":"y"},"entities":[{"id":1,"name":"t","entityType":null,"parentId":null,"siblingOrder":0,"position":{"x":0,"y":0,"z":0},"rotation":{"x":0,"y":0,"z":0},"scale":{"x":1,"y":1,"z":1}}]}""");
        var loaded = await _env.LoadAsync(scenePath);

        Assert.True(loaded.Succeeded);
        Assert.Single(loaded.Value!.Entities);
        Assert.Null(loaded.Value.Snapshot.Assets);
    }

    [Fact]
    public async Task V2_scene_loads_without_assets()
    {
        var scenePath = _env.NewScenePath();
        await File.WriteAllTextAsync(scenePath,
            """{"format":"XuanYuScene","schemaVersion":2,"scene":{"id":"x","name":"y"},"entities":[{"id":1,"name":"c","entityType":"Cube","parentId":null,"siblingOrder":0,"position":{"x":0,"y":0,"z":0},"rotation":{"x":0,"y":0,"z":0},"scale":{"x":1,"y":1,"z":1}}]}""");

        var loaded = await _env.LoadAsync(scenePath);

        Assert.True(loaded.Succeeded);
        Assert.Equal(WorldEntityTypes.Cube, loaded.Value!.Entities[0].Type);
    }

    [Fact]
    public async Task V3_empty_assets_scene_loads()
    {
        var scenePath = _env.NewScenePath();
        await File.WriteAllTextAsync(scenePath,
            """{"format":"XuanYuScene","schemaVersion":3,"scene":{"id":"x","name":"y"},"entities":[{"id":1,"name":"c","entityType":"Cube","parentId":null,"siblingOrder":0,"position":{"x":0,"y":0,"z":0},"rotation":{"x":0,"y":0,"z":0},"scale":{"x":1,"y":1,"z":1}}],"assets":[]}""");

        var loaded = await _env.LoadAsync(scenePath);

        Assert.True(loaded.Succeeded);
        Assert.Empty(loaded.Value!.Snapshot.Assets ?? []);
    }

    [Fact]
    public async Task Plain_entity_without_model_asset_id_is_valid_in_v3()
    {
        var scenePath = _env.NewScenePath();
        await File.WriteAllTextAsync(scenePath,
            """{"format":"XuanYuScene","schemaVersion":3,"scene":{"id":"x","name":"y"},"entities":[{"id":1,"name":"c","entityType":"Cube","parentId":null,"siblingOrder":0,"position":{"x":0,"y":0,"z":0},"rotation":{"x":0,"y":0,"z":0},"scale":{"x":1,"y":1,"z":1}}],"assets":[]}""");

        var loaded = await _env.LoadAsync(scenePath);

        Assert.True(loaded.Succeeded);
    }

    [Fact]
    public async Task Unknown_asset_kind_rejected()
    {
        var scenePath = _env.NewScenePath();
        await File.WriteAllTextAsync(scenePath,
            """{"format":"XuanYuScene","schemaVersion":3,"scene":{"id":"x","name":"y"},"entities":[],"assets":[{"assetId":"asset_00000000000000000000000000000001","kind":"Texture","relativePath":"models/x/source.glb","displayName":"a.glb","importerVersion":1}]}""");

        var loaded = await _env.LoadAsync(scenePath);

        Assert.False(loaded.Succeeded);
        Assert.Equal("UnknownAssetKind", loaded.ErrorCode);
    }

    [Fact]
    public async Task Unsafe_relative_path_rejected()
    {
        var scenePath = _env.NewScenePath();
        await File.WriteAllTextAsync(scenePath,
            """{"format":"XuanYuScene","schemaVersion":3,"scene":{"id":"x","name":"y"},"entities":[],"assets":[{"assetId":"asset_00000000000000000000000000000001","kind":"ModelGltf","relativePath":"../outside.glb","displayName":"a.glb","importerVersion":1}]}""");

        var loaded = await _env.LoadAsync(scenePath);

        Assert.False(loaded.Succeeded);
        Assert.Equal("UnsafeAssetPath", loaded.ErrorCode);
    }

    [Fact]
    public async Task Unknown_model_asset_id_rejected()
    {
        var scenePath = _env.NewScenePath();
        await File.WriteAllTextAsync(scenePath,
            """{"format":"XuanYuScene","schemaVersion":3,"scene":{"id":"x","name":"y"},"entities":[{"id":1,"name":"m","entityType":"StaticModel","parentId":null,"siblingOrder":0,"position":{"x":0,"y":0,"z":0},"rotation":{"x":0,"y":0,"z":0},"scale":{"x":1,"y":1,"z":1},"modelAssetId":"asset_00000000000000000000000000000002"}],"assets":[{"assetId":"asset_00000000000000000000000000000001","kind":"ModelGltf","relativePath":"models/x/source.glb","displayName":"a.glb","importerVersion":1}]}""");

        var loaded = await _env.LoadAsync(scenePath);

        Assert.False(loaded.Succeeded);
        Assert.Equal("UnknownEntityAssetId", loaded.ErrorCode);
    }

    public void Dispose() => _env.Dispose();
}

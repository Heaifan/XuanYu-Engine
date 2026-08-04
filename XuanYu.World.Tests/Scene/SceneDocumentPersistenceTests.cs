using System.Text.Json;
using XuanYu.Editor.SceneDocument;
using XuanYu.World;
using XuanYu.World.Scene;

namespace XuanYu.World.Tests.World;

public sealed class SceneDocumentPersistenceTests
{
    [Fact]
    public async Task Save_writes_v3_and_load_preserves_cube()
    {
        var path = TempScene();
        var scene = new SceneStateOwner(null, false);
        var cube = scene.AddCubeEntity();
        var storage = new SceneStorageService();
        var snapshot = SceneDocumentWorldBridge.Capture(scene, "r2", "R2");

        Assert.True((await storage.SaveAsync(path, snapshot)).Succeeded);
        using var json = JsonDocument.Parse(await File.ReadAllTextAsync(path));
        Assert.Equal(4, json.RootElement.GetProperty("schemaVersion").GetInt32());
        Assert.Equal(WorldEntityTypes.Cube,
            json.RootElement.GetProperty("entities")[0].GetProperty("entityType").GetString());
        var loaded = await storage.LoadAsync(path);
        Assert.True(loaded.Succeeded);
        Assert.Equal(cube.EntityKey, loaded.Value!.Entities.Single().Id);
        Assert.Equal(WorldEntityTypes.Cube, loaded.Value.Entities.Single().EntityType);
    }

    [Fact]
    public async Task V1_missing_type_loads_as_legacy_triangle()
    {
        var path = TempScene();
        await File.WriteAllTextAsync(path, Document(1, typeLine: ""));

        var loaded = await new SceneStorageService().LoadAsync(path);

        Assert.True(loaded.Succeeded);
        Assert.Equal(WorldEntityTypes.LegacyMinimalTriangle,
            loaded.Value!.Entities.Single().EntityType);
    }

    [Theory]
    [InlineData(4, ",\"entityType\":\"Sphere\"", "UnknownEntityType")]
    [InlineData(4, ",\"entityType\":\"StaticModel\"", "MissingEntityAssetId")]
    [InlineData(5, ",\"entityType\":\"Cube\"", "UnsupportedSchema")]
    public async Task Invalid_schema_or_type_is_rejected(int version, string typeLine, string code)
    {
        var path = TempScene();
        await File.WriteAllTextAsync(path, Document(version, typeLine));

        var loaded = await new SceneStorageService().LoadAsync(path);

        Assert.False(loaded.Succeeded);
        Assert.Equal(code, loaded.ErrorCode);
    }

    static string TempScene() => Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.xyscene");

    static string Document(int version, string typeLine) => $$$"""
        {"format":"XuanYuScene","schemaVersion":{{{version}}},"scene":{"id":"s","name":"S"},
        "entities":[{"id":1,"name":"E"{{{typeLine}}},"parentId":null,"siblingOrder":0,
        "position":{"x":0,"y":0,"z":0},"rotation":{"x":0,"y":0,"z":0},
        "scale":{"x":1,"y":1,"z":1}}]}
        """;
}

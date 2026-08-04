using XuanYu.Editor.Assets;
using XuanYu.World;
using XuanYu.World.Scene;

namespace XuanYu.World.Tests.World;

public sealed class StaticModelAuthoringServiceTests
{
    readonly StaticModelAuthoringService _service = new();
    [Fact]
    public void Valid_glb_imports_entity_and_binding()
    {
        using var file = TempGlb();
        var (scene, catalog) = NewScene();
        var result = _service.Import(file.Path, scene, catalog);
        Assert.True(result.Succeeded);
        Assert.True(result.Entity!.Value.EntityKey.IsValid);
        Assert.True(result.AssetId.IsValid);
        Assert.True(catalog.TryGetByEntity(result.Entity!.Value.EntityKey, out var binding));
        Assert.Equal(result.AssetId, binding.AssetId);
        Assert.Equal(file.Path, binding.SourcePath);
    }
    [Fact]
    public void Missing_file_fails_without_entity()
    {
        var (scene, catalog) = NewScene();
        var before = scene.Entities.Count;
        var path = Path.Combine(Path.GetTempPath(), "missing-" + Guid.NewGuid() + ".glb");
        var result = _service.Import(path, scene, catalog);
        Assert.False(result.Succeeded);
        Assert.Equal(scene.Entities.Count, before);
        Assert.Empty(catalog.Snapshot);
    }
    [Fact]
    public void Non_glb_extension_fails_without_entity()
    {
        var (scene, catalog) = NewScene();
        var path = Path.Combine(Path.GetTempPath(), "not-model-" + Guid.NewGuid() + ".txt");
        File.WriteAllText(path, "not a glb");
        try
        {
            var result = _service.Import(path, scene, catalog);
            Assert.False(result.Succeeded);
            Assert.Empty(catalog.Snapshot);
        }
        finally { File.Delete(path); }
    }
    [Fact]
    public void Corrupt_glb_fails_without_entity()
    {
        using var file = TempGlb(GlbFactory.InvalidHeader());
        var (scene, catalog) = NewScene();
        var before = scene.Entities.Count;
        var result = _service.Import(file.Path, scene, catalog);
        Assert.False(result.Succeeded);
        Assert.Equal(scene.Entities.Count, before);
        Assert.Empty(catalog.Snapshot);
    }
    [Fact]
    public void Importing_twice_creates_independent_bindings()
    {
        using var file = TempGlb();
        var (scene, catalog) = NewScene();
        var first = _service.Import(file.Path, scene, catalog);
        var second = _service.Import(file.Path, scene, catalog);
        Assert.True(first.Succeeded);
        Assert.True(second.Succeeded);
        Assert.NotEqual(first.AssetId, second.AssetId);
        Assert.Equal(2, catalog.Snapshot.Count);
        Assert.Equal(2, scene.Entities.Count(e => e.Type == WorldEntityTypes.StaticModel));
    }
    [Fact]
    public void Imported_entity_is_static_model_type_with_bounds()
    {
        using var file = TempGlb();
        var (scene, catalog) = NewScene();
        var result = _service.Import(file.Path, scene, catalog);
        Assert.True(scene.TryGetEntity(result.Entity!.Value.EntityKey, out var entity));
        Assert.Equal(WorldEntityTypes.StaticModel, entity.Type);
        Assert.True(entity.Extent.Max.X > entity.Extent.Min.X);
    }

    static (SceneStateOwner, SceneStaticModelCatalog) NewScene() =>
        (new SceneStateOwner(null, seedInitialEntity: false), new SceneStaticModelCatalog());

    static TempFile TempGlb(byte[]? bytes = null)
    {
        var path = Path.Combine(Path.GetTempPath(), "xuanyu-d3-" + Guid.NewGuid().ToString("N") + ".glb");
        File.WriteAllBytes(path, bytes ?? GlbFactory.Triangle());
        return new TempFile(path);
    }

    sealed record TempFile(string Path) : IDisposable
    {
        public void Dispose() => TryDelete(Path);
        static void TryDelete(string p) { try { File.Delete(p); } catch (IOException) { } }
    }
}

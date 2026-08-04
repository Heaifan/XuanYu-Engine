using XuanYu.Editor.SceneDocument;
using XuanYu.World.Tests.World;

namespace XuanYu.World.Tests.Assets;

public sealed class LoadTransactionTests : IDisposable
{
    readonly ScenePersistenceEnv _env = new();

    [Fact]
    public async Task Single_model_round_trip_restores_entity_and_asset()
    {
        var glb = _env.NewGlb("soldier", GlbFactory.Triangle());
        var assetId = _env.ImportGlb(glb);
        var scenePath = _env.NewScenePath();
        await _env.SaveAsync(scenePath);

        var loaded = await _env.LoadAsync(scenePath);

        Assert.True(loaded.Succeeded);
        var value = loaded.Value!;
        Assert.Single(value.Entities);
        Assert.Single(value.Bindings);
        Assert.Equal(assetId, value.Bindings[0].AssetId.Value);
        Assert.Equal(0, value.MissingCount);
        Assert.Equal(0, value.FailedCount);
        Assert.Single(value.Models);
        Assert.Contains(".xyassets", value.Bindings[0].SourcePath);
    }

    [Fact]
    public async Task Two_models_round_trip_keeps_both()
    {
        var glb1 = _env.NewGlb("a", GlbFactory.Triangle());
        var glb2 = _env.NewGlb("b", GlbFactory.MultiPrimitive());
        _env.ImportGlb(glb1);
        _env.ImportGlb(glb2);
        var scenePath = _env.NewScenePath();
        await _env.SaveAsync(scenePath);

        var loaded = await _env.LoadAsync(scenePath);

        Assert.True(loaded.Succeeded);
        Assert.Equal(2, loaded.Value!.Bindings.Count);
        Assert.Equal(2, loaded.Value.Models.Count);
    }

    [Fact]
    public async Task Missing_glb_keeps_entity_and_reports_missing()
    {
        var glb = _env.NewGlb("soldier", GlbFactory.Triangle());
        _env.ImportGlb(glb);
        var scenePath = _env.NewScenePath();
        await _env.SaveAsync(scenePath);
        // 移走托管 source.glb 模拟缺失。
        var hosted = Directory.GetFiles(Path.Combine(_env.Dir, "Battle01.xyassets"), "source.glb", SearchOption.AllDirectories).Single();
        File.Delete(hosted);

        var loaded = await _env.LoadAsync(scenePath);

        Assert.True(loaded.Succeeded);
        var value = loaded.Value!;
        Assert.Single(value.Entities);
        Assert.Equal(1, value.MissingCount);
        Assert.Equal(0, value.FailedCount);
        Assert.Empty(value.Models);
        Assert.Equal(WorldEntityTypes.StaticModel, value.Entities[0].Type);
    }

    [Fact]
    public async Task Damaged_glb_keeps_entity_and_reports_failed()
    {
        var glb = _env.NewGlb("soldier", GlbFactory.Triangle());
        _env.ImportGlb(glb);
        var scenePath = _env.NewScenePath();
        await _env.SaveAsync(scenePath);
        // 破坏托管 source.glb 模拟损坏。
        var hosted = Directory.GetFiles(Path.Combine(_env.Dir, "Battle01.xyassets"), "source.glb", SearchOption.AllDirectories).Single();
        File.WriteAllBytes(hosted, [1, 2, 3]);

        var loaded = await _env.LoadAsync(scenePath);

        Assert.True(loaded.Succeeded);
        var value = loaded.Value!;
        Assert.Single(value.Entities);
        Assert.Equal(0, value.MissingCount);
        Assert.Equal(1, value.FailedCount);
        Assert.Empty(value.Models);
    }

    public void Dispose() => _env.Dispose();
}

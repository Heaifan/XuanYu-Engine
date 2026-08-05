using XuanYu.Editor.SceneDocument;
using XuanYu.Editor.Assets;
using XuanYu.World;
using XuanYu.World.Tests.World;

namespace XuanYu.World.Tests.Assets;

public sealed class SaveTransactionTests : IDisposable
{
    readonly ScenePersistenceEnv _env = new();

    [Fact]
    public async Task Single_model_save_writes_v3_assets_and_hosted_file()
    {
        var glb = _env.NewGlb("soldier", GlbFactory.Triangle());
        var assetId = _env.ImportGlb(glb);
        var scenePath = _env.NewScenePath();

        var result = await _env.SaveAsync(scenePath);

        Assert.True(result.Succeeded);
        var text = await File.ReadAllTextAsync(scenePath);
        Assert.Contains("\"schemaVersion\": 4", text);
        Assert.Contains($"\"assetId\": \"{assetId}\"", text);
        Assert.Contains("\"kind\": \"ModelGltf\"", text);
        Assert.Contains("models/", text);
        Assert.DoesNotContain(glb, text.Replace("\\", "/"));
        var hosted = Path.Combine(_env.Dir, "Battle01.xyassets", "models", assetId, "source.glb");
        Assert.True(File.Exists(hosted));
        Assert.Contains("source.glb", hosted);
    }

    [Fact]
    public async Task Two_models_save_writes_two_assets()
    {
        var glb1 = _env.NewGlb("a", GlbFactory.Triangle());
        var glb2 = _env.NewGlb("b", GlbFactory.MultiPrimitive());
        var id1 = _env.ImportGlb(glb1);
        var id2 = _env.ImportGlb(glb2);
        var scenePath = _env.NewScenePath();

        var result = await _env.SaveAsync(scenePath);

        Assert.True(result.Succeeded);
        var text = await File.ReadAllTextAsync(scenePath);
        Assert.Contains(id1, text);
        Assert.Contains(id2, text);
        Assert.Equal(2, Count(text, "ModelGltf"));
    }

    [Fact]
    public async Task Rebind_source_paths_after_save()
    {
        var glb = _env.NewGlb("soldier", GlbFactory.Triangle());
        _env.ImportGlb(glb);
        var scenePath = _env.NewScenePath();

        var result = await _env.SaveAsync(scenePath);

        Assert.True(result.Succeeded);
        Assert.Single(result.Value!.HostedSourcePaths);
        var hosted = result.Value.HostedSourcePaths.Values.Single();
        Assert.Contains(".xyassets", hosted);
    }

    [Fact]
    public async Task Missing_binding_fails_save()
    {
        var scenePath = _env.NewScenePath();
        var glb = _env.NewGlb("soldier", GlbFactory.Triangle());
        _env.ImportGlb(glb);
        // 解绑：从 Catalog 移除实体绑定（模拟 StaticModel 无 Binding）。
        var binding = _env.Catalog.Snapshot[0];
        _env.Catalog.Remove(binding.EntityId);

        var result = await _env.SaveAsync(scenePath);

        Assert.False(result.Succeeded);
    }

    static int Count(string text, string token) =>
        System.Text.RegularExpressions.Regex.Matches(text, token).Count;

    public void Dispose() => _env.Dispose();
}

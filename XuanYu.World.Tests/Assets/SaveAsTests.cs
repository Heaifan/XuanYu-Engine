using XuanYu.Editor.SceneDocument;
using XuanYu.World.Tests.World;

namespace XuanYu.World.Tests.Assets;

// D4：另存为与重复保存（拆分自 SaveTransactionTests，5+100）。
public sealed class SaveAsTests : IDisposable
{
    readonly ScenePersistenceEnv _env = new();

    [Fact]
    public async Task Save_twice_succeeds_and_replaces()
    {
        var glb = _env.NewGlb("soldier", GlbFactory.Triangle());
        _env.ImportGlb(glb);
        var scenePath = _env.NewScenePath();
        await _env.SaveAsync(scenePath);

        var second = await _env.SaveAsync(scenePath);

        Assert.True(second.Succeeded);
        var dirs = Directory.GetDirectories(_env.Dir, "*.staging-*").Concat(
            Directory.GetDirectories(_env.Dir, "*.backup-*"));
        Assert.Empty(dirs);
    }

    [Fact]
    public async Task Save_as_creates_independent_assets_root()
    {
        var glb = _env.NewGlb("soldier", GlbFactory.Triangle());
        _env.ImportGlb(glb);
        var first = _env.NewScenePath("Battle01");
        var second = _env.NewScenePath("Battle02");

        var r1 = await _env.SaveAsync(first);
        var r2 = await _env.SaveAsync(second);

        Assert.True(r1.Succeeded);
        Assert.True(r2.Succeeded);
        Assert.True(Directory.Exists(Path.Combine(_env.Dir, "Battle01.xyassets")));
        Assert.True(Directory.Exists(Path.Combine(_env.Dir, "Battle02.xyassets")));
        Assert.DoesNotContain("Battle01", r2.Value!.HostedSourcePaths.Values.Single());
    }

    [Fact]
    public async Task Save_as_does_not_modify_source_scene()
    {
        var glb = _env.NewGlb("soldier", GlbFactory.Triangle());
        _env.ImportGlb(glb);
        var first = _env.NewScenePath("Battle01");
        await _env.SaveAsync(first);
        var before = await File.ReadAllTextAsync(first);

        var second = _env.NewScenePath("Battle02");
        await _env.SaveAsync(second);

        var after = await File.ReadAllTextAsync(first);
        Assert.Equal(before, after);
        Assert.True(Directory.Exists(Path.Combine(_env.Dir, "Battle01.xyassets")));
        Assert.DoesNotContain("Battle02", after);
    }

    public void Dispose() => _env.Dispose();
}

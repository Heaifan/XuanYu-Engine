using XuanYu.Core.Identity;
using XuanYu.Editor.Assets;

namespace XuanYu.World.Tests.Assets;

public sealed class WorldCR4D4HostingPlannerTests : IDisposable
{
    readonly string _dir = HostingTestEnv.NewDirectory();

    [Fact]
    public void Single_asset_plans_correct_directories()
    {
        var glb = HostingTestEnv.NewGlb(_dir, "a");
        var scene = HostingTestEnv.NewScene(_dir);
        var id = HostingTestEnv.Asset("00");
        var plan = SceneAssetHostingPlanner.Create(scene, [Binding(id, glb)]).Value!;
        Assert.Equal(Path.Combine(_dir, "Battle01.xyassets"), plan.AssetRootPath);
        Assert.Equal($"models/{id.Value}/source.glb", plan.Assets[0].RelativePath);
        Assert.Equal(plan.Assets[0].FinalPath,
            Path.Combine(plan.AssetRootPath, "models", id.Value, "source.glb"));
        Assert.Single(plan.Assets);
    }

    [Fact]
    public void Multiple_assets_are_ordered_by_asset_id()
    {
        var glb1 = HostingTestEnv.NewGlb(_dir, "b");
        var glb2 = HostingTestEnv.NewGlb(_dir, "a");
        var scene = HostingTestEnv.NewScene(_dir);
        var low = HostingTestEnv.Asset("01");
        var high = HostingTestEnv.Asset("99");
        var plan = SceneAssetHostingPlanner.Create(scene,
            [Binding(high, glb1), Binding(low, glb2)]).Value!;
        Assert.Equal([low.Value, high.Value], plan.Assets.Select(a => a.AssetId.Value).ToArray());
    }

    [Fact]
    public void Same_asset_id_same_path_deduplicates()
    {
        var glb = HostingTestEnv.NewGlb(_dir, "a");
        var scene = HostingTestEnv.NewScene(_dir);
        var id = HostingTestEnv.Asset("00");
        var plan = SceneAssetHostingPlanner.Create(scene, [Binding(id, glb), Binding(id, glb)]).Value!;
        Assert.Single(plan.Assets);
    }

    [Fact]
    public void Same_path_different_asset_id_allowed()
    {
        var glb = HostingTestEnv.NewGlb(_dir, "a");
        var scene = HostingTestEnv.NewScene(_dir);
        var plan = SceneAssetHostingPlanner.Create(scene,
            [Binding(HostingTestEnv.Asset("00"), glb), Binding(HostingTestEnv.Asset("01"), glb)]).Value!;
        Assert.Equal(2, plan.Assets.Count);
    }

    [Fact]
    public void Scene_with_chinese_and_space_names_plan_correctly()
    {
        var glb = HostingTestEnv.NewGlb(_dir, "a");
        var scene = HostingTestEnv.NewScene(_dir, "战场 01");
        var plan = SceneAssetHostingPlanner.Create(scene, [Binding(HostingTestEnv.Asset("00"), glb)]).Value!;
        Assert.EndsWith("战场 01.xyassets", plan.AssetRootPath);
        Assert.DoesNotContain("..", plan.Assets[0].RelativePath);
    }

    static SceneStaticModelBinding Binding(AssetId id, string path) =>
        new(EntityId.FromInt(1), id, path);

    public void Dispose() => HostingTestEnv.Cleanup(_dir);
}

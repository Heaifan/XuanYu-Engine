using XuanYu.Core.Identity;
using XuanYu.Editor.Assets;

namespace XuanYu.World.Tests.Assets;

public sealed class WorldCR4D4HostingSaveAsTests : IDisposable
{
    readonly string _dir = HostingTestEnv.NewDirectory();

    [Fact]
    public void Same_assets_plan_to_distinct_roots_for_two_scenes()
    {
        var glb = HostingTestEnv.NewGlb(_dir, "a", [1, 2, 3, 4]);
        var scene1 = HostingTestEnv.NewScene(_dir, "Battle01");
        var scene2 = HostingTestEnv.NewScene(_dir, "Battle02");
        var plan1 = SceneAssetHostingPlanner.Create(scene1, [Binding(HostingTestEnv.Asset("00"), glb)]).Value!;
        var plan2 = SceneAssetHostingPlanner.Create(scene2, [Binding(HostingTestEnv.Asset("00"), glb)]).Value!;
        Assert.NotEqual(plan1.AssetRootPath, plan2.AssetRootPath);
        Assert.EndsWith("Battle01.xyassets", plan1.AssetRootPath);
        Assert.EndsWith("Battle02.xyassets", plan2.AssetRootPath);
    }

    [Fact]
    public void Two_scenes_activate_independently()
    {
        var glb = HostingTestEnv.NewGlb(_dir, "a", [1, 2, 3, 4]);
        var scene1 = HostingTestEnv.NewScene(_dir, "Battle01");
        var scene2 = HostingTestEnv.NewScene(_dir, "Battle02");
        var t1 = Prepare(scene1, glb, HostingTestEnv.Asset("00"));
        var t2 = Prepare(scene2, glb, HostingTestEnv.Asset("00"));
        Assert.True(t1.Activate().Succeeded);
        Assert.True(t2.Activate().Succeeded);
        Assert.True(Directory.Exists(t1.Plan.AssetRootPath));
        Assert.True(Directory.Exists(t2.Plan.AssetRootPath));
        Assert.NotEqual(t1.Plan.AssetRootPath, t2.Plan.AssetRootPath);
    }

    static SceneAssetHostingTransaction Prepare(string scene, string glb, AssetId id) =>
        SceneAssetHostingTransaction.Prepare(
            SceneAssetHostingPlanner.Create(scene, [Binding(id, glb)]).Value!);

    static SceneStaticModelBinding Binding(AssetId id, string path) =>
        new(EntityId.FromInt(1), id, path);

    public void Dispose() => HostingTestEnv.Cleanup(_dir);
}

using XuanYu.Core.Identity;
using XuanYu.Editor.Assets;

namespace XuanYu.World.Tests.Assets;

public sealed class HostingPlannerRejectTests : IDisposable
{
    readonly string _dir = HostingTestEnv.NewDirectory();

    [Fact]
    public void Same_asset_id_different_path_rejected()
    {
        var glb1 = HostingTestEnv.NewGlb(_dir, "a");
        var glb2 = HostingTestEnv.NewGlb(_dir, "b");
        var scene = HostingTestEnv.NewScene(_dir);
        var id = HostingTestEnv.Asset("00");
        var result = SceneAssetHostingPlanner.Create(scene, [Binding(id, glb1), Binding(id, glb2)]);
        Assert.False(result.Succeeded);
        Assert.Equal(SceneAssetHostingError.AssetSourceConflict, result.ErrorCode);
    }

    [Fact]
    public void Non_glb_source_rejected()
    {
        var txt = Path.Combine(_dir, "a.txt");
        File.WriteAllText(txt, "x");
        var scene = HostingTestEnv.NewScene(_dir);
        var result = SceneAssetHostingPlanner.Create(scene, [Binding(HostingTestEnv.Asset("00"), txt)]);
        Assert.False(result.Succeeded);
        Assert.Equal(SceneAssetHostingError.UnsupportedSourceExtension, result.ErrorCode);
    }

    [Fact]
    public void Missing_source_rejected()
    {
        var scene = HostingTestEnv.NewScene(_dir);
        var missing = Path.Combine(_dir, "nope.glb");
        var result = SceneAssetHostingPlanner.Create(scene, [Binding(HostingTestEnv.Asset("00"), missing)]);
        Assert.False(result.Succeeded);
        Assert.Equal(SceneAssetHostingError.SourceFileMissing, result.ErrorCode);
    }

    [Fact]
    public void Relative_source_rejected()
    {
        HostingTestEnv.NewGlb(_dir, "a");
        var scene = HostingTestEnv.NewScene(_dir);
        var result = SceneAssetHostingPlanner.Create(scene, [Binding(HostingTestEnv.Asset("00"), "a.glb")]);
        Assert.False(result.Succeeded);
        Assert.Equal(SceneAssetHostingError.InvalidSourcePath, result.ErrorCode);
    }

    [Fact]
    public void Invalid_asset_id_rejected()
    {
        var glb = HostingTestEnv.NewGlb(_dir, "a");
        var scene = HostingTestEnv.NewScene(_dir);
        var result = SceneAssetHostingPlanner.Create(scene,
            [Binding(default, glb)]);
        Assert.False(result.Succeeded);
        Assert.Equal(SceneAssetHostingError.InvalidAssetId, result.ErrorCode);
    }

    static SceneStaticModelBinding Binding(AssetId id, string path) =>
        new(EntityId.FromInt(1), id, path);

    public void Dispose() => HostingTestEnv.Cleanup(_dir);
}

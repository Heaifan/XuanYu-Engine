using XuanYu.Core.Identity;
using XuanYu.Editor.Assets;

namespace XuanYu.World.Tests.Assets;

public sealed class WorldCR4D4HostingTransactionTests : IDisposable
{
    readonly string _dir = HostingTestEnv.NewDirectory();

    [Fact]
    public void Prepare_copies_single_asset_into_staging()
    {
        var glb = HostingTestEnv.NewGlb(_dir, "a", [1, 2, 3, 4, 5, 6, 7, 8]);
        var scene = HostingTestEnv.NewScene(_dir);
        var transaction = Prepare(scene, glb, HostingTestEnv.Asset("00"));
        Assert.Equal(SceneAssetHostingState.Prepared, transaction.State);
        var staged = transaction.Plan.Assets[0].StagedPath;
        Assert.True(File.Exists(staged));
        Assert.Equal(8, new FileInfo(staged).Length);
        Assert.False(Directory.Exists(transaction.Plan.AssetRootPath));
    }

    [Fact]
    public void Prepare_does_not_touch_existing_asset_root()
    {
        var glb = HostingTestEnv.NewGlb(_dir, "a", [9, 9]);
        var scene = HostingTestEnv.NewScene(_dir);
        var root = Path.Combine(_dir, "Battle01.xyassets");
        Directory.CreateDirectory(root);
        File.WriteAllText(Path.Combine(root, "marker.txt"), "old");
        var transaction = Prepare(scene, glb, HostingTestEnv.Asset("00"));
        Assert.True(File.Exists(Path.Combine(root, "marker.txt")));
        Assert.True(Directory.Exists(root));
        Assert.Equal(SceneAssetHostingState.Prepared, transaction.State);
    }

    [Fact]
    public void Prepare_failure_cleans_staging()
    {
        var scene = HostingTestEnv.NewScene(_dir);
        var missing = Path.Combine(_dir, "missing.glb");
        var plan = SceneAssetHostingPlanner.Create(scene,
            [Binding(HostingTestEnv.Asset("00"), missing)]);
        Assert.False(plan.Succeeded);
        Assert.Equal(SceneAssetHostingError.SourceFileMissing, plan.ErrorCode);
    }

    [Fact]
    public void Activate_without_existing_root_moves_staging()
    {
        var glb = HostingTestEnv.NewGlb(_dir, "a", [1, 2, 3, 4]);
        var scene = HostingTestEnv.NewScene(_dir);
        var transaction = Prepare(scene, glb, HostingTestEnv.Asset("00"));
        var result = transaction.Activate();
        Assert.True(result.Succeeded);
        Assert.Equal(SceneAssetHostingState.Activated, transaction.State);
        Assert.True(File.Exists(transaction.Plan.Assets[0].FinalPath));
        Assert.False(Directory.Exists(transaction.Plan.StagingRootPath));
        Assert.False(Directory.Exists(transaction.Plan.BackupRootPath));
    }

    [Fact]
    public void Activate_with_existing_root_creates_backup()
    {
        var glb = HostingTestEnv.NewGlb(_dir, "a", [1, 2, 3, 4]);
        var scene = HostingTestEnv.NewScene(_dir);
        var old = Path.Combine(_dir, "Battle01.xyassets");
        Directory.CreateDirectory(Path.Combine(old, "models"));
        File.WriteAllText(Path.Combine(old, "keep.txt"), "old-data");
        var transaction = Prepare(scene, glb, HostingTestEnv.Asset("00"));
        var result = transaction.Activate();
        Assert.True(result.Succeeded);
        Assert.Equal(SceneAssetHostingState.Activated, transaction.State);
        Assert.Equal(4, new FileInfo(transaction.Plan.Assets[0].FinalPath).Length);
        Assert.True(File.Exists(Path.Combine(transaction.Plan.BackupRootPath, "keep.txt")));
    }

    [Fact]
    public void Double_activate_rejected()
    {
        var glb = HostingTestEnv.NewGlb(_dir, "a");
        var scene = HostingTestEnv.NewScene(_dir);
        var transaction = Prepare(scene, glb, HostingTestEnv.Asset("00"));
        transaction.Activate();
        var result = transaction.Activate();
        Assert.False(result.Succeeded);
        Assert.Equal(SceneAssetHostingError.InvalidTransactionState, result.ErrorCode);
    }

    static SceneAssetHostingTransaction Prepare(string scene, string glb, AssetId id) =>
        SceneAssetHostingTransaction.Prepare(
            SceneAssetHostingPlanner.Create(scene, [Binding(id, glb)]).Value!);

    static SceneStaticModelBinding Binding(AssetId id, string path) =>
        new(EntityId.FromInt(1), id, path);

    public void Dispose() => HostingTestEnv.Cleanup(_dir);
}

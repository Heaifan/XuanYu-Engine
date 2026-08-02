using XuanYu.Core.Identity;
using XuanYu.Editor.Assets;

namespace XuanYu.World.Tests.Assets;

public sealed class WorldCR4D4HostingCompleteTests : IDisposable
{
    readonly string _dir = HostingTestEnv.NewDirectory();

    [Fact]
    public void Complete_deletes_backup_and_keeps_asset_root()
    {
        var glb = HostingTestEnv.NewGlb(_dir, "a", [1, 2, 3, 4]);
        var scene = HostingTestEnv.NewScene(_dir);
        var root = Path.Combine(_dir, "Battle01.xyassets");
        Directory.CreateDirectory(root);
        File.WriteAllText(Path.Combine(root, "old.glb"), "old");
        var transaction = Prepare(scene, glb, HostingTestEnv.Asset("00"));
        transaction.Activate();
        var result = transaction.Complete();
        Assert.True(result.Succeeded);
        Assert.Equal(SceneAssetHostingState.Completed, transaction.State);
        Assert.False(Directory.Exists(transaction.Plan.BackupRootPath));
        Assert.True(Directory.Exists(transaction.Plan.AssetRootPath));
        Assert.False(Directory.Exists(transaction.Plan.StagingRootPath));
    }

    [Fact]
    public void Complete_without_activate_rejected()
    {
        var glb = HostingTestEnv.NewGlb(_dir, "a");
        var scene = HostingTestEnv.NewScene(_dir);
        var transaction = Prepare(scene, glb, HostingTestEnv.Asset("00"));
        var result = transaction.Complete();
        Assert.False(result.Succeeded);
        Assert.Equal(SceneAssetHostingError.InvalidTransactionState, result.ErrorCode);
    }

    [Fact]
    public void Double_complete_rejected()
    {
        var glb = HostingTestEnv.NewGlb(_dir, "a");
        var scene = HostingTestEnv.NewScene(_dir);
        var transaction = Prepare(scene, glb, HostingTestEnv.Asset("00"));
        transaction.Activate();
        transaction.Complete();
        var result = transaction.Complete();
        Assert.False(result.Succeeded);
        Assert.Equal(SceneAssetHostingError.InvalidTransactionState, result.ErrorCode);
    }

    [Fact]
    public void Activate_without_prepare_rejected()
    {
        var glb = HostingTestEnv.NewGlb(_dir, "a");
        var scene = HostingTestEnv.NewScene(_dir);
        var plan = SceneAssetHostingPlanner.Create(scene, [Binding(HostingTestEnv.Asset("00"), glb)]).Value!;
        var transaction = SceneAssetHostingTransaction.Prepare(plan);
        transaction.Rollback();
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

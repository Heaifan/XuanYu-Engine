using XuanYu.Core.Identity;
using XuanYu.Editor.Assets;

namespace XuanYu.World.Tests.Assets;

public sealed class HostingRollbackTests : IDisposable
{
    readonly string _dir = HostingTestEnv.NewDirectory();

    [Fact]
    public void Prepared_rollback_only_removes_staging()
    {
        var glb = HostingTestEnv.NewGlb(_dir, "a", [1, 2, 3, 4]);
        var scene = HostingTestEnv.NewScene(_dir);
        var transaction = Prepare(scene, glb, HostingTestEnv.Asset("00"));
        var result = transaction.Rollback();
        Assert.True(result.Succeeded);
        Assert.Equal(SceneAssetHostingState.RolledBack, transaction.State);
        Assert.False(Directory.Exists(transaction.Plan.StagingRootPath));
        Assert.False(Directory.Exists(transaction.Plan.AssetRootPath));
    }

    [Fact]
    public void Activated_rollback_without_old_root_removes_new_root()
    {
        var glb = HostingTestEnv.NewGlb(_dir, "a", [1, 2, 3, 4]);
        var scene = HostingTestEnv.NewScene(_dir);
        var transaction = Prepare(scene, glb, HostingTestEnv.Asset("00"));
        transaction.Activate();
        var result = transaction.Rollback();
        Assert.True(result.Succeeded);
        Assert.Equal(SceneAssetHostingState.RolledBack, transaction.State);
        Assert.False(Directory.Exists(transaction.Plan.AssetRootPath));
    }

    [Fact]
    public void Activated_rollback_with_old_root_restores_old_content()
    {
        var glb = HostingTestEnv.NewGlb(_dir, "a", [1, 2, 3, 4]);
        var scene = HostingTestEnv.NewScene(_dir);
        var oldRoot = Path.Combine(_dir, "Battle01.xyassets");
        Directory.CreateDirectory(Path.Combine(oldRoot, "models"));
        File.WriteAllText(Path.Combine(oldRoot, "keep.txt"), "original");
        var transaction = Prepare(scene, glb, HostingTestEnv.Asset("00"));
        transaction.Activate();
        Assert.False(File.Exists(Path.Combine(transaction.Plan.AssetRootPath, "keep.txt")));
        var result = transaction.Rollback();
        Assert.True(result.Succeeded);
        Assert.Equal(SceneAssetHostingState.RolledBack, transaction.State);
        Assert.True(File.Exists(Path.Combine(transaction.Plan.AssetRootPath, "keep.txt")));
        Assert.Equal("original", File.ReadAllText(Path.Combine(transaction.Plan.AssetRootPath, "keep.txt")));
        Assert.False(Directory.Exists(transaction.Plan.BackupRootPath));
    }

    [Fact]
    public void Double_rollback_rejected()
    {
        var glb = HostingTestEnv.NewGlb(_dir, "a");
        var scene = HostingTestEnv.NewScene(_dir);
        var transaction = Prepare(scene, glb, HostingTestEnv.Asset("00"));
        transaction.Rollback();
        var result = transaction.Rollback();
        Assert.False(result.Succeeded);
        Assert.Equal(SceneAssetHostingError.InvalidTransactionState, result.ErrorCode);
    }

    [Fact]
    public void Completed_rollback_rejected()
    {
        var glb = HostingTestEnv.NewGlb(_dir, "a");
        var scene = HostingTestEnv.NewScene(_dir);
        var transaction = Prepare(scene, glb, HostingTestEnv.Asset("00"));
        transaction.Activate();
        transaction.Complete();
        var result = transaction.Rollback();
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

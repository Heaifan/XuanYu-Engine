using XuanYu.Core.Identity;
using XuanYu.Editor.Assets;

namespace XuanYu.World.Tests.Assets;

public sealed class HostingStructuredFailureTests : IDisposable
{
    readonly string _dir = HostingTestEnv.NewDirectory();

    [Fact]
    public void Activate_non_io_move_failure_returns_result_and_restores_previous_assets()
    {
        var glb = HostingTestEnv.NewGlb(_dir, "new", [1, 2, 3]);
        var scene = HostingTestEnv.NewScene(_dir);
        var root = Path.Combine(_dir, "Battle01.xyassets");
        Directory.CreateDirectory(root);
        File.WriteAllText(Path.Combine(root, "old.txt"), "old");
        var plan = SceneAssetHostingPlanner.Create(scene,
            [new SceneStaticModelBinding(EntityId.FromInt(1), HostingTestEnv.Asset("00"), glb)]).Value!;
        var moveCount = 0;
        var transaction = SceneAssetHostingTransaction.Prepare(plan, (from, to) =>
        {
            if (++moveCount == 2) throw new InvalidOperationException("injected move failure");
            Directory.Move(from, to);
        });

        var result = transaction.Activate();

        Assert.False(result.Succeeded);
        Assert.Equal(SceneAssetHostingError.ActivateFailed, result.ErrorCode);
        Assert.Equal(SceneAssetHostingState.Failed, transaction.State);
        Assert.Contains("InvalidOperationException", transaction.LastError);
        Assert.Equal("old", File.ReadAllText(Path.Combine(root, "old.txt")));
        Assert.False(Directory.Exists(transaction.Plan.StagingRootPath));
        Assert.Equal(2, moveCount);
    }

    [Fact]
    public void Activate_non_io_move_failure_without_existing_root_returns_result()
    {
        var glb = HostingTestEnv.NewGlb(_dir, "new", [1, 2, 3]);
        var scene = HostingTestEnv.NewScene(_dir);
        var plan = SceneAssetHostingPlanner.Create(scene,
            [new SceneStaticModelBinding(EntityId.FromInt(1), HostingTestEnv.Asset("00"), glb)]).Value!;
        var transaction = SceneAssetHostingTransaction.Prepare(plan,
            (_, _) => throw new InvalidOperationException("injected move failure"));

        var result = transaction.Activate();

        Assert.False(result.Succeeded);
        Assert.Equal(SceneAssetHostingError.ActivateFailed, result.ErrorCode);
        Assert.Equal(SceneAssetHostingState.Failed, transaction.State);
        Assert.Contains("InvalidOperationException", transaction.LastError);
        Assert.False(Directory.Exists(transaction.Plan.StagingRootPath));
    }

    public void Dispose() => HostingTestEnv.Cleanup(_dir);
}

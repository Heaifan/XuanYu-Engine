using XuanYu.Core.Gizmo;
using XuanYu.Core.History;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Transform;

using XuanYu.World.Scene;
using XuanYu.World.Transform;
namespace XuanYu.Core.Tests.History;

public sealed class TransformHistoryRedoIntegrationTests
{
    [Fact]
    public void Undo_then_redo_restores_after_snapshot()
    {
        var scene = new SceneStateOwner();
        var history = new EditorHistoryOwner();

        CommitMove(scene, history, 2);
        Assert.True(history.TryUndo(out var undo));
        Assert.True(scene.RestoreTransform(undo.EntityKey, undo.Before));
        Assert.Equal(Vector3d.Zero, scene.RenderSnapshot.Entity.Transform.Position);

        Assert.True(history.TryRedo(out var redo));
        Assert.True(scene.RestoreTransform(redo.EntityKey, redo.After));
        Assert.Equal(new Vector3d(2, 0, 0), scene.RenderSnapshot.Entity.Transform.Position);
    }

    [Fact]
    public void New_commit_after_undo_disables_redo_branch()
    {
        var scene = new SceneStateOwner();
        var history = new EditorHistoryOwner();
        CommitMove(scene, history, 1);
        CommitMove(scene, history, 2);

        Assert.True(history.TryUndo(out var entry));
        Assert.True(scene.RestoreTransform(entry.EntityKey, entry.Before));

        CommitMove(scene, history, 3);

        Assert.Equal(0, history.RedoCount);
        Assert.False(history.TryRedo(out _));
        Assert.Equal(new Vector3d(3, 0, 0), scene.RenderSnapshot.Entity.Transform.Position);
    }

    static void CommitMove(SceneStateOwner scene, EditorHistoryOwner history, double x)
    {
        var session = new TransformSession();
        Assert.True(session.Begin(17, scene.RenderSnapshot.Entity, MoveGizmoAxis.X));
        session.TryPreview(17, new Vector3d(x, 0, 0));
        Assert.True(session.TryCommit(17, scene, out var commit));
        if (commit.Changed)
        {
            history.Push(new TransformHistoryEntry(commit.EntityKey, commit.Before, commit.After));
        }
    }
}

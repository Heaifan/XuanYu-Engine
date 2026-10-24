using XuanYu.Core.Gizmo;
using XuanYu.Core.History;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Transform;

using XuanYu.World.Scene;
using XuanYu.Editor.Transform;
namespace XuanYu.Core.Tests.History;

public sealed class TransformHistoryIntegrationTests
{
    [Fact]
    public void Successful_commit_creates_one_history_and_undo_restores_before()
    {
        var scene = new SceneStateOwner();
        var history = new EditorHistoryOwner();
        var session = Begin(scene);

        session.TryPreview(17, Vector3d.UnitX);
        session.TryPreview(17, new Vector3d(2, 0, 0));

        Assert.True(session.TryCommit(17, scene, out var commit));
        Push(history, commit);
        Assert.Equal(1, history.Count);

        Assert.True(history.TryUndo(out var entry));
        Assert.True(scene.RestoreTransform(entry.EntityKey, entry.Before));
        Assert.Equal(Vector3d.Zero, scene.RenderSnapshot.Entity.Transform.Position);
        Assert.Equal(0, history.Count);
    }

    [Fact]
    public void Cancel_and_late_mouseup_do_not_create_history()
    {
        var scene = new SceneStateOwner();
        var history = new EditorHistoryOwner();
        var session = Begin(scene);

        session.TryPreview(17, Vector3d.UnitZ);
        Assert.True(session.TryCancel(17));
        Assert.False(session.TryCommit(17, scene, out var commit));
        Push(history, commit);

        Assert.Equal(0, history.Count);
        Assert.Equal(Vector3d.Zero, scene.RenderSnapshot.Entity.Transform.Position);
    }

    [Fact]
    public void No_change_commit_does_not_create_history()
    {
        var scene = new SceneStateOwner();
        var history = new EditorHistoryOwner();
        var session = Begin(scene);

        Assert.True(session.TryCommit(17, scene, out var commit));
        Push(history, commit);

        Assert.False(commit.Changed);
        Assert.Equal(0, history.Count);
    }

    static TransformSession Begin(SceneStateOwner scene)
    {
        var session = new TransformSession();
        Assert.True(session.Begin(17, scene.RenderSnapshot.Entity, MoveGizmoAxis.X));
        return session;
    }

    static void Push(EditorHistoryOwner history, SceneTransformCommitResult commit)
    {
        if (commit.Changed)
        {
            history.Push(new TransformHistoryEntry(commit.EntityKey, commit.Before, commit.After));
        }
    }
}

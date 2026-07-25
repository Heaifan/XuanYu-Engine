using XuanYu.Core.Gizmo;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Transform;
using XuanYu.World.Scene;
using XuanYu.Editor.Transform;

namespace XuanYu.World.Tests.Transform;

public sealed class TransformSessionTests
{
    [Theory]
    [InlineData(MoveGizmoAxis.X, 2, 0, 0)]
    [InlineData(MoveGizmoAxis.Y, 0, 2, 0)]
    [InlineData(MoveGizmoAxis.Z, 0, 0, 2)]
    public void Preview_is_axis_constrained_without_changing_scene(
        MoveGizmoAxis axis, double x, double y, double z)
    {
        var scene = new SceneStateOwner();
        var session = Begin(scene, axis);

        Assert.True(session.TryPreview(17, new Vector3d(x, y, z)));

        Assert.Equal(Vector3d.Zero, scene.RenderSnapshot.Entity.Transform.Position);
        Assert.Equal(new Vector3d(x, y, z), session.Preview!.Value.Position);
        Assert.Equal(1, scene.SpatialRevision);
    }

    [Fact]
    public void Commit_changes_scene_once_and_invalidates_session()
    {
        var scene = new SceneStateOwner();
        var session = Begin(scene, MoveGizmoAxis.X);
        session.TryPreview(17, Vector3d.UnitX);

        Assert.True(session.TryCommit(17, scene));
        Assert.False(session.TryCommit(17, scene));
        Assert.Equal(Vector3d.UnitX, scene.RenderSnapshot.Entity.Transform.Position);
        Assert.Equal(2, scene.SpatialRevision);
    }

    [Fact]
    public void Cancel_discards_preview_and_late_commit_is_ignored()
    {
        var scene = new SceneStateOwner();
        var session = Begin(scene, MoveGizmoAxis.Z);
        session.TryPreview(17, Vector3d.UnitZ);

        Assert.True(session.TryCancel(17));
        Assert.False(session.TryCommit(17, scene));
        Assert.Equal(Vector3d.Zero, scene.RenderSnapshot.Entity.Transform.Position);
        Assert.Equal(1, scene.SpatialRevision);
    }

    [Fact]
    public void Render_snapshot_can_overlay_preview_without_replacing_committed_transform()
    {
        var scene = new SceneStateOwner();
        var preview = new PreviewTransform(Vector3d.UnitY);

        var render = scene.RenderSnapshot with { PreviewTransform = preview };

        Assert.Equal(Vector3d.UnitY, render.RenderPosition);
        Assert.Equal(Vector3d.Zero, render.Entity.Transform.Position);
    }

    static TransformSession Begin(SceneStateOwner scene, MoveGizmoAxis axis)
    {
        var session = new TransformSession();
        Assert.True(session.Begin(17, scene.RenderSnapshot.Entity, axis));
        return session;
    }
}

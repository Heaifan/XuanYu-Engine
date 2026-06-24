using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.Core.Math;
using FluidWarfare.Render.Scene;
using FluidWarfare.Render.Scene.Position;
using FluidWarfare.Render.Selection;
using FluidWarfare.Render.Selection.Presented;

namespace FluidWarfare.Tests.Render.Selection.Presented;

public sealed class PresentedScenePickSnapshotTests
{
    private static RenderScene MakeSceneWithEntity(int id, Vector3d pos, double half)
    {
        var bounds = new SceneAxisAlignedBounds(pos, new Vector3d(half, half, half));
        var placement = new RenderUnitPlacement(new Vector3d(pos.X, pos.Y, pos.Z - half));
        var obj = new RenderObjectInfo(
            EntityId.FromInt(id), $"E{id}", pos,
            RenderObjectVisualKind.UnitMarker, "", bounds)
        { Placement = placement };
        return new RenderScene([obj]);
    }

    [Fact]
    public void BuildSnapshot_ContainsEntity()
    {
        var pos = new Vector3d(10, 20, 0);
        var scene = MakeSceneWithEntity(1, pos, 0.625);
        var snap = PresentedScenePickSnapshotBuilder.Build(scene, 1, 1, 800, 600);

        Assert.True(snap.IsValid);
        Assert.Single(snap.Entities);
        Assert.Equal(1, snap.Entities[0].EntityId);
    }

    [Fact]
    public void BuildSnapshot_IncludesUnitMarker()
    {
        var bounds = new SceneAxisAlignedBounds(Vector3d.Zero, new Vector3d(0.5, 0.5, 0.5));
        var obj = new RenderObjectInfo(
            EntityId.FromInt(1), "Marker", Vector3d.Zero,
            RenderObjectVisualKind.UnitMarker, "", bounds);
        var scene = new RenderScene([obj]);
        var snap = PresentedScenePickSnapshotBuilder.Build(scene, 1, 1, 800, 600);
        Assert.Single(snap.Entities);
    }

    [Fact]
    public void PresentFailure_KeepsPreviousSnapshot()
    {
        var scene = MakeSceneWithEntity(1, new Vector3d(10, 20, 0), 0.625);
        var valid = PresentedScenePickSnapshotBuilder.Build(scene, 1, 1, 800, 600);
        var none = PresentedScenePickSnapshot.None;
        Assert.False(none.IsValid);
        Assert.True(valid.IsValid);
    }

    [Fact]
    public void Snapshot_StoresFrameAndCameraRevision()
    {
        var scene = MakeSceneWithEntity(1, Vector3d.Zero, 0.625);
        var snap = PresentedScenePickSnapshotBuilder.Build(scene, 42, 7, 1920, 1080);
        Assert.Equal(42, snap.FrameIndex);
        Assert.Equal(7, snap.CameraRevision);
        Assert.Equal(1920, snap.ViewportWidth);
        Assert.Equal(1080, snap.ViewportHeight);
    }

    [Fact]
    public void MoveEntity_AfterBuild_NotInSnapshot()
    {
        var scene = MakeSceneWithEntity(1, new Vector3d(0, 0, 0), 0.625);
        var snap = PresentedScenePickSnapshotBuilder.Build(scene, 1, 1, 800, 600);
        Assert.Equal(0, snap.Entities[0].Bounds.Center.X, 3);

        var moved = RenderSceneObjectPositionWriter.Update(
            scene, EntityId.FromInt(1), new Vector3d(100, 0, 0));
        var snap2 = PresentedScenePickSnapshotBuilder.Build(moved.NewScene!, 2, 1, 800, 600);

        Assert.Equal(0, snap.Entities[0].Bounds.Center.X, 3);
        Assert.Equal(100, snap2.Entities[0].Bounds.Center.X, 3);
    }
}

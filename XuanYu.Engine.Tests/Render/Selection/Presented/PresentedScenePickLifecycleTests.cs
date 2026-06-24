using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.Core.Math;
using XuanYu.Engine.Render.Scene;
using XuanYu.Engine.Render.Scene.Position;
using XuanYu.Engine.Render.Selection;
using XuanYu.Engine.Render.Selection.Ground;
using XuanYu.Engine.Render.Selection.Presented;
using XuanYu.Engine.Render.Selection.Pointer;

namespace FluidWarfare.Tests.Render.Selection.Presented;

/// <summary>
/// Preview → Confirm → Cancel 对 Picking 状态的影响。
/// 不使用真实 GPU，只验证 CPU RenderScene + PresentedScenePickSnapshot 行为。
/// </summary>
public sealed class PresentedScenePickLifecycleTests
{
    private static readonly SceneGroundPlane Ground = SceneGroundPlane.Default;

    private static RenderScene MakeScene(int id, Vector3d pos, double half)
    {
        var bounds = new SceneAxisAlignedBounds(pos, new Vector3d(half, half, half));
        var placement = new RenderUnitPlacement(new Vector3d(pos.X, pos.Y, pos.Z - half));
        var obj = new RenderObjectInfo(
            EntityId.FromInt(id), $"E{id}", pos,
            RenderObjectVisualKind.UnitMarker, "", bounds)
        { Placement = placement };
        return new RenderScene([obj]);
    }

    private static SceneRay MakeRay(Vector3d from, Vector3d to)
    {
        var dir = (to - from).Normalize();
        return new SceneRay(from, dir);
    }

    [Fact]
    public void PreviewPosition_UpdatesSelectionBounds()
    {
        var scene = MakeScene(1, new Vector3d(0, 0, 0), 0.625);
        var id = EntityId.FromInt(1);
        var moved = RenderSceneObjectPositionWriter.Update(scene, id, new Vector3d(100, 0, 0));
        Assert.True(moved.IsChanged);
        Assert.NotNull(moved.NewScene);

        var snap = PresentedScenePickSnapshotBuilder.Build(moved.NewScene!, 1, 1, 800, 600);
        var ray = MakeRay(new Vector3d(100, 0, 50), new Vector3d(100, 0, 0));
        var result = ScenePointerPicker.Pick(ray, snap, Ground);
        Assert.Equal(ScenePointerPickKind.Entity, result.Kind);
    }

    [Fact]
    public void PreviewPosition_NewLocationHits()
    {
        var scene = MakeScene(1, new Vector3d(50, 0, 0), 0.625);
        var id = EntityId.FromInt(1);
        var moved = RenderSceneObjectPositionWriter.Update(scene, id, new Vector3d(200, 0, 0));
        var snap = PresentedScenePickSnapshotBuilder.Build(moved.NewScene!, 1, 1, 800, 600);

        var ray = MakeRay(new Vector3d(200, 0, 50), new Vector3d(200, 0, 0));
        var result = ScenePointerPicker.Pick(ray, snap, Ground);
        Assert.Equal(ScenePointerPickKind.Entity, result.Kind);
    }

    [Fact]
    public void PreviewPosition_OldLocationMisses()
    {
        var scene = MakeScene(1, new Vector3d(50, 0, 0), 0.625);
        var id = EntityId.FromInt(1);
        var moved = RenderSceneObjectPositionWriter.Update(scene, id, new Vector3d(200, 0, 0));
        var snap = PresentedScenePickSnapshotBuilder.Build(moved.NewScene!, 1, 1, 800, 600);

        var ray = MakeRay(new Vector3d(50, 0, 50), new Vector3d(50, 0, 0));
        var result = ScenePointerPicker.Pick(ray, snap, Ground);
        Assert.NotEqual(ScenePointerPickKind.Entity, result.Kind);
    }

    [Fact]
    public void CancelPreview_InitialLocationHitsAgain()
    {
        var scene = MakeScene(1, new Vector3d(10, 0, 0), 0.625);
        var id = EntityId.FromInt(1);

        var moved = RenderSceneObjectPositionWriter.Update(scene, id, new Vector3d(100, 0, 0));
        var afterMove = PresentedScenePickSnapshotBuilder.Build(moved.NewScene!, 1, 1, 800, 600);
        var rayNew = MakeRay(new Vector3d(100, 0, 50), new Vector3d(100, 0, 0));
        Assert.Equal(ScenePointerPickKind.Entity,
            ScenePointerPicker.Pick(rayNew, afterMove, Ground).Kind);

        var restored = RenderSceneObjectPositionWriter.Update(moved.NewScene!, id, new Vector3d(10, 0, 0));
        var afterCancel = PresentedScenePickSnapshotBuilder.Build(restored.NewScene!, 2, 1, 800, 600);

        var rayInitial = MakeRay(new Vector3d(10, 0, 50), new Vector3d(10, 0, 0));
        Assert.Equal(ScenePointerPickKind.Entity,
            ScenePointerPicker.Pick(rayInitial, afterCancel, Ground).Kind);
        var rayOld = MakeRay(new Vector3d(100, 0, 50), new Vector3d(100, 0, 0));
        Assert.NotEqual(ScenePointerPickKind.Entity,
            ScenePointerPicker.Pick(rayOld, afterCancel, Ground).Kind);
    }

    [Fact]
    public void ConfirmPreview_FinalLocationHits()
    {
        var scene = MakeScene(1, new Vector3d(0, 0, 0), 0.625);
        var id = EntityId.FromInt(1);
        var final = RenderSceneObjectPositionWriter.Update(scene, id, new Vector3d(75, 0, 0));
        var snap = PresentedScenePickSnapshotBuilder.Build(final.NewScene!, 1, 1, 800, 600);

        var ray = MakeRay(new Vector3d(75, 0, 50), new Vector3d(75, 0, 0));
        Assert.Equal(ScenePointerPickKind.Entity,
            ScenePointerPicker.Pick(ray, snap, Ground).Kind);
    }

    [Fact]
    public void Snapshot_AfterSceneChange_ReflectsNewPosition()
    {
        var scene = MakeScene(1, new Vector3d(0, 0, 0), 0.625);
        var snap1 = PresentedScenePickSnapshotBuilder.Build(scene, 1, 1, 800, 600);

        var moved = RenderSceneObjectPositionWriter.Update(scene,
            EntityId.FromInt(1), new Vector3d(300, 0, 0));
        var snap2 = PresentedScenePickSnapshotBuilder.Build(moved.NewScene!, 2, 1, 800, 600);

        Assert.Equal(0, snap1.Entities[0].Bounds.Center.X, 3);
        Assert.Equal(300, snap2.Entities[0].Bounds.Center.X, 3);
    }
}

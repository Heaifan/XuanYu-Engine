using System.Reflection;
using XuanYu.Core.Gizmo;
using XuanYu.Core.History;
using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.UI;
using XuanYu.World.Scene;

namespace XuanYu.World.Tests.World;

public sealed partial class WorldMoveTransformUiTests
{
    [Theory]
    [InlineData(MoveGizmoAxis.XY, false, false, true)]
    [InlineData(MoveGizmoAxis.XZ, false, true, false)]
    [InlineData(MoveGizmoAxis.YZ, true, false, false)]
    public void Plane_move_changes_only_plane_axes(
        MoveGizmoAxis axis,
        bool keepX,
        bool keepY,
        bool keepZ)
    {
        var vm = MoveVm();
        var start = vm.RenderSnapshot.Entity.Transform.Position;
        var hit = PlaneHit(vm, axis);

        Assert.True(vm.TryBeginMoveGizmoCapture(7, hit.X, hit.Y, hit.Viewport, true));
        Assert.True(vm.PreviewViewportPointer(7, hit.EndX, hit.EndY));
        Assert.Equal(start, vm.RenderSnapshot.Entity.Transform.Position);
        Assert.True(vm.CommitViewportPointer(7, hit.EndX, hit.EndY));

        var end = vm.RenderSnapshot.Entity.Transform.Position;
        Assert.Equal(1, HistoryOf(vm).Count);
        AssertAxis(start.X, end.X, keepX);
        AssertAxis(start.Y, end.Y, keepY);
        AssertAxis(start.Z, end.Z, keepZ);
    }

    [Fact]
    public void Plane_commit_without_motion_does_not_create_history()
    {
        var vm = MoveVm();
        var hit = PlaneHit(vm, MoveGizmoAxis.XY);

        Assert.True(vm.TryBeginMoveGizmoCapture(7, hit.X, hit.Y, hit.Viewport, true));
        Assert.True(vm.CommitViewportPointer(7, hit.X, hit.Y));

        Assert.Equal(Vector3d.Zero, vm.RenderSnapshot.Entity.Transform.Position);
        Assert.Equal(0, HistoryOf(vm).Count);
    }

    static UiVm MoveVm()
    {
        var vm = new UiVm(null, () => true);
        vm.SelectedHierarchyItem = vm.HierarchyItems.Single(i => i.Key == "EntityId(1)");
        vm.SelectToolCommand.Execute("移动");
        return vm;
    }

    static void AssertAxis(double before, double after, bool keep)
    {
        if (keep) Assert.Equal(before, after);
        else Assert.NotEqual(before, after);
    }

    static (double X, double Y, double EndX, double EndY, ViewportState Viewport) PlaneHit(
        UiVm vm,
        MoveGizmoAxis axis)
    {
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
        var state = ViewProjectionState.Create(vm.RenderSnapshot.CameraState, viewport);
        var layout = MoveGizmoLayout.Project(state, vm.RenderSnapshot.Entity.Transform.Position);
        var plane = layout.Planes.Single(p => p.Axis == axis);
        var x = (plane.A.X + plane.B.X + plane.C.X + plane.D.X) / 4.0;
        var y = (plane.A.Y + plane.B.Y + plane.C.Y + plane.D.Y) / 4.0;
        return (x, y, plane.C.X, plane.C.Y, viewport);
    }

    static EditorHistoryOwner HistoryOf(UiVm vm)
    {
        var field = typeof(UiVm).GetField(
            "_historyOwner",
            BindingFlags.Instance | BindingFlags.NonPublic);
        return (EditorHistoryOwner)field!.GetValue(vm)!;
    }

    static SceneStateOwner SceneOf(UiVm vm)
    {
        var field = typeof(UiVm).GetField(
            "_sceneState",
            BindingFlags.Instance | BindingFlags.NonPublic);
        return (SceneStateOwner)field!.GetValue(vm)!;
    }
}

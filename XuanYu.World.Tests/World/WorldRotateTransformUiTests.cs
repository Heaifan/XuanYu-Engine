using System.Reflection;
using XuanYu.Core.Gizmo;
using XuanYu.Core.History;
using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.World;

public sealed class WorldRotateTransformUiTests
{
    [Fact]
    public void Rotate_gizmo_drag_commits_once_and_undo_redo_restores()
    {
        var vm = RotateVm();
        var hit = RingHit(vm, RotateGizmoAxis.Z);

        Assert.True(vm.TryBeginRotateGizmoCapture(7, hit.StartX, hit.StartY, hit.Viewport, true));
        Assert.True(vm.PreviewViewportPointer(7, hit.MidX, hit.MidY));
        Assert.True(vm.PreviewViewportPointer(7, hit.EndX, hit.EndY));
        Assert.True(vm.CommitViewportPointer(7, hit.EndX, hit.EndY));

        Assert.NotEqual(0.0, vm.RenderSnapshot.Entity.Transform.Rotation.Z);
        Assert.Equal(1, HistoryOf(vm).Count);
        vm.RunCommand.Execute("撤销");
        Assert.Equal(0.0, vm.RenderSnapshot.Entity.Transform.Rotation.Z);
        Assert.Equal(0, HistoryOf(vm).Count);
        vm.RunCommand.Execute("重做");
        Assert.NotEqual(0.0, vm.RenderSnapshot.Entity.Transform.Rotation.Z);
    }

    [Fact]
    public void Rotate_cancel_discards_preview_without_history()
    {
        var vm = RotateVm();
        var hit = RingHit(vm, RotateGizmoAxis.Z);

        Assert.True(vm.TryBeginRotateGizmoCapture(7, hit.StartX, hit.StartY, hit.Viewport, true));
        Assert.True(vm.PreviewViewportPointer(7, hit.MidX, hit.MidY));
        Assert.True(vm.PreviewViewportPointer(7, hit.EndX, hit.EndY));
        vm.CancelInteractionFromEscape();

        Assert.False(vm.CommitViewportPointer(7, hit.EndX, hit.EndY));
        Assert.Equal(0.0, vm.RenderSnapshot.Entity.Transform.Rotation.Z);
        Assert.Equal(0, HistoryOf(vm).Count);
    }

    static UiVm RotateVm()
    {
        var vm = new UiVm(null, () => true);
        vm.SelectedHierarchyItem = vm.HierarchyItems.Single(i => i.Key == "EntityId(1)");
        vm.SelectToolCommand.Execute("旋转");
        return vm;
    }

    static (double StartX, double StartY, double MidX, double MidY, double EndX, double EndY, ViewportState Viewport)
        RingHit(UiVm vm, RotateGizmoAxis axis)
    {
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
        var state = ViewProjectionState.Create(vm.RenderSnapshot.CameraState, viewport);
        var layout = RotateGizmoLayout.Project(state, vm.RenderSnapshot.Entity.Transform.Position);
        var ring = layout.Rings.Single(r => r.Axis == axis);
        var start = ring.Points[6];
        var mid = ring.Points[9];
        var end = ring.Points[15];
        return (start.X, start.Y, mid.X, mid.Y, end.X, end.Y, viewport);
    }

    static EditorHistoryOwner HistoryOf(UiVm vm)
    {
        var field = typeof(UiVm).GetField(
            "_historyOwner",
            BindingFlags.Instance | BindingFlags.NonPublic);
        return (EditorHistoryOwner)field!.GetValue(vm)!;
    }
}

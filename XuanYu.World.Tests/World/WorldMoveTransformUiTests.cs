using XuanYu.Core.Gizmo;
using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.World;

public sealed partial class WorldMoveTransformUiTests
{
    [Fact]
    public void Move_gizmo_drag_commits_once_and_undo_redo_restores()
    {
        var vm = new UiVm(null, () => true);
        vm.SelectedHierarchyItem = vm.HierarchyItems.Single(item => item.Key == "EntityId(1)");
        vm.SelectToolCommand.Execute("移动");
        var hit = AxisHit(vm, MoveGizmoAxis.X);

        Assert.True(vm.TryBeginMoveGizmoCapture(7, hit.X, hit.Y, hit.Viewport, true));
        Assert.True(vm.PreviewViewportPointer(7, hit.EndX, hit.EndY));
        Assert.True(vm.CommitViewportPointer(7, hit.EndX, hit.EndY));

        Assert.Equal(new Vector3d(0.6, 0, 0), vm.RenderSnapshot.Entity.Transform.Position);
        vm.RunCommand.Execute("撤销");
        Assert.Equal(Vector3d.Zero, vm.RenderSnapshot.Entity.Transform.Position);
        vm.RunCommand.Execute("重做");
        Assert.Equal(new Vector3d(0.6, 0, 0), vm.RenderSnapshot.Entity.Transform.Position);
    }

    [Fact]
    public void Move_capture_blocks_selection_change()
    {
        var vm = new UiVm(null, () => true);
        vm.SelectedHierarchyItem = vm.HierarchyItems.Single(item => item.Key == "EntityId(1)");
        vm.SelectToolCommand.Execute("移动");
        var hit = AxisHit(vm, MoveGizmoAxis.X);

        Assert.True(vm.TryBeginMoveGizmoCapture(7, hit.X, hit.Y, hit.Viewport, true));
        vm.SelectedHierarchyItem = vm.HierarchyItems.Single(item => item.Key == "EntityId(2)");

        Assert.Equal("EntityId(1)", vm.SelectionKey);
    }

    static (double X, double Y, double EndX, double EndY, ViewportState Viewport) AxisHit(
        UiVm vm,
        MoveGizmoAxis axis)
    {
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
        var state = ViewProjectionState.Create(vm.RenderSnapshot.CameraState, viewport);
        var layout = MoveGizmoLayout.Project(state, vm.RenderSnapshot.Entity.Transform.Position);
        var segment = layout.Segments.Single(item => item.Axis == axis);
        var x = (segment.Start.X + segment.End.X) / 2.0;
        var y = (segment.Start.Y + segment.End.Y) / 2.0;
        return (x, y, segment.End.X, segment.End.Y, viewport);
    }
}

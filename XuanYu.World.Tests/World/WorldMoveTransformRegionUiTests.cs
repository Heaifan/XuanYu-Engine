using XuanYu.Core.Gizmo;
using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Editor.UI;
using XuanYu.World;

namespace XuanYu.World.Tests.World;

public sealed partial class WorldMoveTransformUiTests
{
    [Fact]
    public void Cross_region_move_keeps_selection_and_undo_redo_regions()
    {
        var vm = new UiVm(null, () => true);
        var key = EntityId.FromInt(5);
        SceneOf(vm).CommitPositionWithResult(key, new Vector3d(4.8, 0, 0));
        vm.SelectedHierarchyItem = vm.HierarchyItems.Single(i => i.Key == key.ToString());
        vm.SelectToolCommand.Execute("移动");
        var hit = AxisHit(vm, MoveGizmoAxis.X);

        Assert.True(vm.TryBeginMoveGizmoCapture(7, hit.X, hit.Y, hit.Viewport, true));
        Assert.True(vm.CommitViewportPointer(7, hit.EndX, hit.EndY));

        Assert.Equal(key.ToString(), vm.SelectionKey);
        Assert.Equal(key, vm.RenderSnapshot.Entity.EntityKey);
        Assert.Equal(RegionKey.FromGrid(1, 0, 0), SceneOf(vm).GetRegion(key));
        Assert.Contains(vm.HierarchyItems, i => i.Key == "Region(1,0,0)" && i.IsRegion);
        Assert.Contains("区域 1,0,0", vm.SelectionPath);
        Assert.Contains(vm.InspectorFields, f => f.Contains("区域 1,0,0"));

        vm.RunCommand.Execute("撤销");
        Assert.Equal(RegionKey.Origin, SceneOf(vm).GetRegion(key));
        Assert.Contains("区域 0,0,0", vm.SelectionPath);
        vm.RunCommand.Execute("重做");
        Assert.Equal(RegionKey.FromGrid(1, 0, 0), SceneOf(vm).GetRegion(key));
        Assert.Equal(key.ToString(), vm.SelectedHierarchyItem!.Key);
    }

    [Fact]
    public void Cross_region_move_keeps_single_world_entity()
    {
        var vm = new UiVm(null, () => true);
        var key = EntityId.FromInt(5);
        SceneOf(vm).CommitPositionWithResult(key, new Vector3d(4.8, 0, 0));
        vm.SelectedHierarchyItem = vm.HierarchyItems.Single(i => i.Key == key.ToString());
        vm.SelectToolCommand.Execute("移动");
        var hit = AxisHit(vm, MoveGizmoAxis.X);

        Assert.True(vm.TryBeginMoveGizmoCapture(7, hit.X, hit.Y, hit.Viewport, true));
        Assert.True(vm.CommitViewportPointer(7, hit.EndX, hit.EndY));

        Assert.Equal(1, SceneOf(vm).Entities.Count(e => e.EntityKey == key));
        Assert.Contains(vm.RenderSnapshot.Entities, e => e.EntityKey == key);
    }
}

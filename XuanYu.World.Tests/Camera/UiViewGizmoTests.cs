using XuanYu.Core.Math;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.World;

// EDITOR-VIEW-R1：视角 Gizmo 六方向相机命令——朝向正确、观察中心与距离保持。
public sealed class UiViewGizmoTests
{
    [Fact]
    public void Top_view_looks_down_minus_z_keeping_center_and_distance()
    {
        var vm = new UiVm(null, () => true);
        vm.UpdateViewportFrame(1600, 900);
        vm.RunCommand.Execute("查看全部");
        var framed = vm.RenderSnapshot.CameraState;

        vm.RunCommand.Execute("视角-顶");

        var cam = vm.RenderSnapshot.CameraState;
        Assert.Equal(new Vector3d(0, 0, -1), cam.Forward);
        Assert.True(cam.Position.Z > 0, "顶视图相机应位于 +Z");
        // 未选中实体/无地图时观察中心=世界原点，切换前后距离保持。
        Assert.Equal(framed.Position.DistanceTo(Vector3d.Zero),
            cam.Position.DistanceTo(Vector3d.Zero), 6);
        Assert.Equal("顶", vm.ActiveViewFace);
        Assert.True(framed.Forward != cam.Forward);
    }

    [Fact]
    public void Six_faces_produce_frozen_directions()
    {
        var vm = new UiVm(null, () => true);
        vm.UpdateViewportFrame(1600, 900);

        vm.RunCommand.Execute("视角-顶");
        Assert.Equal(new Vector3d(0, 0, -1), vm.RenderSnapshot.CameraState.Forward);
        vm.RunCommand.Execute("视角-底");
        Assert.Equal(new Vector3d(0, 0, 1), vm.RenderSnapshot.CameraState.Forward);
        vm.RunCommand.Execute("视角-前");
        Assert.Equal(new Vector3d(0, 1, 0), vm.RenderSnapshot.CameraState.Forward);
        vm.RunCommand.Execute("视角-后");
        Assert.Equal(new Vector3d(0, -1, 0), vm.RenderSnapshot.CameraState.Forward);
        vm.RunCommand.Execute("视角-右");
        Assert.Equal(new Vector3d(-1, 0, 0), vm.RenderSnapshot.CameraState.Forward);
        vm.RunCommand.Execute("视角-左");
        Assert.Equal(new Vector3d(1, 0, 0), vm.RenderSnapshot.CameraState.Forward);
        Assert.Equal("左", vm.ActiveViewFace);
    }

    [Fact]
    public void View_switch_preserves_selection()
    {
        var vm = new UiVm(null, () => true);
        vm.SelectedHierarchyItem = vm.HierarchyItems.Single(item => item.Key == "EntityId(5)");
        var before = vm.RenderSnapshot.Entity.EntityKey;

        vm.RunCommand.Execute("视角-前");

        Assert.Equal(before, vm.RenderSnapshot.Entity.EntityKey);
        Assert.Equal("前", vm.ActiveViewFace);
    }
}

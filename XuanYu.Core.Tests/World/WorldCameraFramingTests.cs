using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.UI;

namespace XuanYu.Core.Tests.World;

public sealed class WorldCameraFramingTests
{
    [Fact]
    public void Frame_all_centers_current_visible_entities()
    {
        var camera = EditorCameraFraming.FrameAll(
            [Vector3d.Zero, new Vector3d(6, 1.5, 0)], 16.0 / 9.0, 9);

        Assert.Equal(9, camera.Revision);
        Assert.True(camera.Position.DistanceTo(new Vector3d(3, 0.75, 0)) > 4);
        Assert.True(camera.FarPlane >= 100);
    }

    [Fact]
    public void Ui_frame_selected_changes_snapshot_camera_without_changing_entity()
    {
        var vm = new UiVm(null, () => true);
        vm.SelectedHierarchyItem = vm.HierarchyItems.Single(item => item.Key == "EntityId(5)");
        var before = vm.RenderSnapshot.Entity.EntityKey;

        vm.RunCommand.Execute("聚焦");

        Assert.Equal(before, vm.RenderSnapshot.Entity.EntityKey);
        Assert.Equal(new Vector3d(6, 0, 0), vm.RenderSnapshot.CameraState.Position +
            (vm.RenderSnapshot.CameraState.Forward *
             vm.RenderSnapshot.CameraState.Position.DistanceTo(new Vector3d(6, 0, 0))));
    }
}

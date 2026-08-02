using XuanYu.Core.Math;
using XuanYu.Editor.Camera;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.World;

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
    public void Frame_map_uses_pitched_overhead_view()
    {
        // F4：地图取景必须是斜上方俯视，而非接近平视。
        var corners = new[]
        {
            new Vector3d(-1000, -1000, 0), new Vector3d(1000, -1000, 0),
            new Vector3d(-1000, 1000, 0), new Vector3d(1000, 1000, 0)
        };
        var frame = EditorCameraFraming.FrameMapAllWithCenter(corners, 16.0 / 9.0, 7);

        Assert.Equal(7, frame.Camera.Revision);
        Assert.True(frame.Camera.Position.Z > 0, "相机必须位于地图上方");
        Assert.True(frame.Camera.Forward.Z < -0.6, "视线必须明显向下俯视");
        Assert.True(frame.Camera.Position.DistanceTo(Vector3d.Zero) > 2000,
            "相机距离应足够容纳整张地图");
    }

    [Fact]
    public void Frame_map_contains_all_corners_with_margin()
    {
        // F4：45° 俯视下地图四角必须完整落在视锥内（含安全边距）。
        var corners = new[]
        {
            new Vector3d(-1000, -1000, 0), new Vector3d(1000, -1000, 0),
            new Vector3d(-1000, 1000, 0), new Vector3d(1000, 1000, 0)
        };
        var frame = EditorCameraFraming.FrameMapAllWithCenter(corners, 16.0 / 9.0, 8);
        var cam = frame.Camera;
        var halfFov = cam.VerticalFovDegrees * 0.5 * System.Math.PI / 180.0;
        foreach (var corner in corners)
        {
            var toCorner = corner - cam.Position;
            var angle = System.Math.Acos(
                toCorner.Normalize().Dot(cam.Forward));
            Assert.True(angle < halfFov, $"角点 {corner} 超出视锥（{angle * 180 / System.Math.PI:F1}° > {halfFov * 180 / System.Math.PI:F1}°）");
        }
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

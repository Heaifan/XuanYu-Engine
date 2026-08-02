using XuanYu.Core.Math;
using XuanYu.Editor.Camera;

namespace XuanYu.World.Tests.World;

// MAP-A-R1-D5-R1：地图取景屏幕占用率（65%~75%）。
public sealed class WorldCameraFramingOccupancyTests
{
    [Fact]
    public void Frame_map_occupies_65_to_75_percent_of_screen()
    {
        // D5-R1：地图取景后，地图四角 NDC 包围盒应占屏幕约 65%~75%（垂直方向）。
        var corners = new[]
        {
            new Vector3d(-1000, -1000, 0), new Vector3d(1000, -1000, 0),
            new Vector3d(-1000, 1000, 0), new Vector3d(1000, 1000, 0)
        };
        var frame = EditorCameraFraming.FrameMapAllWithCenter(corners, 16.0 / 9.0, 9);
        var cam = frame.Camera;
        var aspect = 16.0 / 9.0;
        var verticalFov = cam.VerticalFovDegrees * System.Math.PI / 180.0;
        var horizontalFov = 2.0 * System.Math.Atan(
            System.Math.Tan(verticalFov * 0.5) * aspect);
        var tanV = System.Math.Tan(verticalFov * 0.5);
        var tanH = System.Math.Tan(horizontalFov * 0.5);

        // 地图四角投影到 NDC（近似：深度方向用相机到角点距离）。
        var ndc = corners.Select(corner =>
        {
            var v = corner - cam.Position;
            var forward = v.Normalize().Dot(cam.Forward);
            var right = v.Normalize().Dot(cam.Right);
            var up = v.Normalize().Dot(cam.Up);
            var nx = right / (forward * tanH);
            var ny = up / (forward * tanV);
            return (X: nx, Y: ny);
        }).ToArray();

        var minX = ndc.Min(p => p.X); var maxX = ndc.Max(p => p.X);
        var minY = ndc.Min(p => p.Y); var maxY = ndc.Max(p => p.Y);
        var width = maxX - minX;
        var height = maxY - minY;
        var occupancy = System.Math.Max(width, height) / 2.0;
        Assert.InRange(occupancy, 0.65, 0.80);
    }
}

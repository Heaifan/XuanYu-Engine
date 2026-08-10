using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

public sealed class MapPickingRoundTripTests
{
    public static IEnumerable<object[]> Cases()
    {
        foreach (var size in new[] { 100.0, 10_000.0, 10_000_000.0 })
        foreach (var dpi in new[] { 1.0, 1.25, 1.5, 2.0 })
        foreach (var viewport in new[] { (1024, 640), (1360, 820), (1920, 1080) })
        foreach (var angle in new[] { 0, 45, 80 })
            yield return [size, dpi, viewport.Item1, viewport.Item2, angle];
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void Screen_pick_world_screen_stays_within_one_dip(
        double size, double dpi, int width, int height, int angle)
    {
        var map = MapDefaultDefinition.CreateDefault() with { SizeMeters = new MapSize(size, size) };
        var viewport = new ViewportState(0, 0, width, height,
            (int)(width * dpi), (int)(height * dpi), dpi, 1);
        var projection = ViewProjectionState.Create(CreateCamera(size, angle), viewport);
        foreach (var point in Points(size))
        {
            var world = MapCoordinateContract.MapToWorld(point, map.Surface.BaseHeightMeters);
            Assert.True(projection.TryProjectWorldPoint(world, out var screen));
            Assert.InRange(screen.X, 0, width);
            Assert.InRange(screen.Y, 0, height);
            Assert.True(MapSurfacePicker.TryPick(map, projection, screen.X, screen.Y, out var picked));
            var projected = projection.ProjectWorldPoint(
                MapCoordinateContract.MapToWorld(picked, map.Surface.BaseHeightMeters));
            var error = Math.Sqrt(Math.Pow(projected.X - screen.X, 2) + Math.Pow(projected.Y - screen.Y, 2));
            Assert.InRange(error, 0.0, 1.0);
        }
    }

    static IEnumerable<MapPoint> Points(double size)
    {
        var d = size * 0.1;
        yield return new MapPoint(0, 0);
        yield return new MapPoint(d, d);
        yield return new MapPoint(-d, d);
        yield return new MapPoint(d, -d);
        yield return new MapPoint(-d, -d);
    }

    static CameraState CreateCamera(double size, int angle)
    {
        if (angle == 0) return new CameraState(new Vector3d(0, 0, size),
            new Vector3d(0, 0, -1), new Vector3d(0, 1, 0), 60, 0.1, size * 10, 1,
            ProjectionMode.Orthographic, size * 1.4);
        var radians = angle * Math.PI / 180.0;
        var position = new Vector3d(0, -size * Math.Cos(radians), size * Math.Sin(radians));
        return new CameraState(position, -position, Vector3d.UnitZ, 60, 0.1, size * 10, 1);
    }
}

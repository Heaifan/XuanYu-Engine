using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

public sealed class MapSurfacePickerTests
{
    [Fact]
    public void Picks_flat_surface_point_inside_bounds()
    {
        var map = MapDefaultDefinition.CreateDefault();
        var camera = new CameraState(
            new Vector3d(50, 50, 100), new Vector3d(0, 0, -1),
            new Vector3d(0, 1, 0), 45, 0.1, 1000, 1,
            ProjectionMode.Orthographic, 100);
        var viewport = new ViewportState(0, 0, 100, 100, 100, 100, 1, 1);
        var projection = ViewProjectionState.Create(camera, viewport);

        Assert.True(MapSurfacePicker.TryPick(map, projection, 50, 50, out var point));
        Assert.Equal(50, point.X, 5);
        Assert.Equal(50, point.Y, 5);
    }

    [Fact]
    public void Rejects_pointer_outside_map_surface()
    {
        var map = MapDefaultDefinition.CreateDefault();
        var camera = new CameraState(
            new Vector3d(0, 0, 100), new Vector3d(0, 0, -1),
            new Vector3d(0, 1, 0), 45, 0.1, 1000, 1,
            ProjectionMode.Orthographic, 100);
        var projection = ViewProjectionState.Create(camera,
            new ViewportState(0, 0, 100, 100, 100, 100, 1, 1));

        Assert.False(MapSurfacePicker.TryPick(map, projection, 0, 0, out _));
    }
}

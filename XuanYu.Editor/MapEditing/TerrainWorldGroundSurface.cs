using XuanYu.World.Geo;
using XuanYu.World.Map;
using XuanYu.World.Terrain;

namespace XuanYu.Editor.MapEditing;

public sealed class TerrainWorldGroundSurface(TerrainWorld world) : IGroundSurface
{
    readonly GeographicWorldMapping _mapping = new(new(
        world.Metadata.WorldExtent.South, world.Metadata.WorldExtent.West, 0));
    public SurfaceBinding Binding => SurfaceBinding.Terrain("terrain-world");
    public int? Revision => world.EditDelta.Revision;

    public bool TryGetElevation(MapPoint worldXY, out double elevation)
    {
        var geo = _mapping.ToGeographic(new(worldXY.X, worldXY.Y, 0));
        var extent = world.Metadata.WorldExtent;
        var x = (geo.Longitude - extent.West) / (extent.East - extent.West);
        var y = (extent.North - geo.Latitude) / (extent.North - extent.South);
        if (x is < 0 or > 1 || y is < 0 or > 1) { elevation = 0; return false; }
        var coordinate = new TerrainSampleCoordinate(
            (int)Math.Round(x * (world.Metadata.Width - 1)),
            (int)Math.Round(y * (world.Metadata.Height - 1)));
        return world.TryGetFinalHeight(coordinate, out elevation);
    }
}

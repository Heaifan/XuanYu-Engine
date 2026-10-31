using XuanYu.World.Geo;
using XuanYu.World.Map;
using XuanYu.World.Terrain;
using XuanYu.World;

namespace XuanYu.Editor.MapEditing;

public sealed class TerrainWorldGroundSurface(TerrainWorld world) : IGroundSurface
{
    readonly GeographicWorldMapping _mapping = new(new(
        world.Metadata.WorldExtent.South, world.Metadata.WorldExtent.West, 0));
    public SurfaceBinding Binding => SurfaceBinding.Terrain("terrain-world");
    public int? Revision => world.EditDelta.Revision;

    public SurfaceQueryResult QuerySurface(MapPoint worldXY)
    {
        GeographicPosition geo;
        try { geo = _mapping.ToGeographic(new(worldXY.X, worldXY.Y, 0)); }
        catch (ArgumentOutOfRangeException) { return SurfaceQueryResult.OutOfBounds; }
        var extent = world.Metadata.WorldExtent;
        var x = (geo.Longitude - extent.West) / (extent.East - extent.West);
        var y = (extent.North - geo.Latitude) / (extent.North - extent.South);
        if (x is < 0 or > 1 || y is < 0 or > 1)
            return SurfaceQueryResult.OutOfBounds;
        var coordinate = new TerrainSampleCoordinate(
            (int)Math.Round(x * (world.Metadata.Width - 1)),
            (int)Math.Round(y * (world.Metadata.Height - 1)));
        var result = world.QueryElevation(coordinate);
        return result.IsValid
            ? SurfaceQueryResult.Valid(result.ElevationMeters)
            : result.Status switch
            {
                WorldQueryStatus.NoData => SurfaceQueryResult.NoData,
                WorldQueryStatus.OutOfBounds => SurfaceQueryResult.OutOfBounds,
                WorldQueryStatus.NoTerrain => SurfaceQueryResult.NoTerrain,
                _ => SurfaceQueryResult.InvalidBinding
            };
    }

    public ElevationQueryResult QueryElevation(MapPoint worldXY)
    {
        var result = QuerySurface(worldXY);
        return result.Status switch
        {
            WorldQueryStatus.Valid => ElevationQueryResult.Valid(result.SurfaceZ),
            WorldQueryStatus.NoData => ElevationQueryResult.NoData,
            WorldQueryStatus.OutOfBounds => ElevationQueryResult.OutOfBounds,
            WorldQueryStatus.NoTerrain => ElevationQueryResult.NoTerrain,
            _ => ElevationQueryResult.InvalidBinding
        };
    }

    public bool TryGetElevation(MapPoint worldXY, out double elevation)
    {
        var result = QuerySurface(worldXY);
        elevation = result.IsValid ? result.SurfaceZ : 0.0;
        return result.IsValid;
    }
}

using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;
using XuanYu.World.Terrain;
using XuanYu.World.Terrain.Import;
using XuanYu.World.Tests.Terrain;

namespace XuanYu.World.Tests.Camera;

/// <summary>
/// Characterization evidence for the current finite Dolly path schedule.
/// These tests do not define a new surface contract or claim a product RED.
/// They show when a dense TerrainWorld query observes a feature that the
/// current maximum 64-position schedule does not observe.
/// </summary>
public sealed class XYEPR2NavigationSafetySamplingEvidenceTests
{
    [Fact(DisplayName = "XYEPR2-NAV-SAMPLING-CANDIDATE-001")]
    public void Narrow_ridge_is_visible_to_dense_query_but_missed_by_64_positions()
    {
        const int side = 257;
        const int ridgeColumn = 127;
        const short ridgeHeight = 30000;
        var world = CreateSingleColumnRidge(side, ridgeColumn, ridgeHeight);
        var surface = new TerrainWorldGroundSurface(world);

        var metersPerDegreeLatitude = 6378137.0 * Math.PI / 180.0;
        var metersPerDegreeLongitude = metersPerDegreeLatitude *
            Math.Cos(world.Metadata.WorldExtent.South * Math.PI / 180.0);
        var pathY = 0.5 * metersPerDegreeLatitude;
        var pathLength = metersPerDegreeLongitude;

        var densePeak = Enumerable.Range(0, 4097)
            .Select(index => surface.QuerySurface(new(pathLength * index / 4096.0, pathY)))
            .Any(query => query.IsValid && query.SurfaceZ >= ridgeHeight);
        var sampledPeak = Enumerable.Range(1, 64)
            .Select(index => surface.QuerySurface(new(pathLength * index / 64.0, pathY)))
            .Any(query => query.IsValid && query.SurfaceZ >= ridgeHeight);

        Assert.True(densePeak, "Dense path queries must observe the one-sample ridge.");
        Assert.False(sampledPeak,
            "The characterization fixture expects the current 64-position schedule to miss the ridge.");
    }

    [Fact(DisplayName = "XYEPR2-NAV-SAMPLING-CANDIDATE-002")]
    public void Five_x_exaggeration_changes_clearance_height_but_not_surface_identity()
    {
        const int side = 5;
        const short sourceHeight = 100;
        var world = CreateSingleColumnRidge(side, 2, sourceHeight);
        var surface = new TerrainWorldGroundSurface(world);
        var metersPerDegreeLatitude = 6378137.0 * Math.PI / 180.0;
        var metersPerDegreeLongitude = metersPerDegreeLatitude *
            Math.Cos(world.Metadata.WorldExtent.South * Math.PI / 180.0);
        var center = new MapPoint(metersPerDegreeLongitude * 0.5, metersPerDegreeLatitude * 0.5);

        var query = surface.QuerySurface(center);

        Assert.True(query.IsValid);
        Assert.Equal(sourceHeight, query.SurfaceZ);
        Assert.Equal(sourceHeight * 5.0, query.SurfaceZ * 5.0);
    }

    static TerrainWorld CreateSingleColumnRidge(int side, int ridgeColumn, short ridgeHeight)
    {
        var values = new short[side * side];
        for (var row = 0; row < side; row++)
            values[row * side + ridgeColumn] = ridgeHeight;
        using var stream = TerrainImportFixture.HgtStream(values);
        var tile = new HgtTerrainElevationTileReader().Read(stream, "n23e121");
        return TerrainWorld.FromElevationTile(tile);
    }
}

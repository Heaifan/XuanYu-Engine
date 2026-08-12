using XuanYu.Editor.MapDocument;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

public sealed class RegionSpatialIndexTests
{
    [Fact]
    public void Query_returns_only_intersecting_region_ids_in_stable_order()
    {
        var hitB = Region("00000000000000000000000000000002", 0, 0);
        var hitA = Region("00000000000000000000000000000001", 5, 5);
        var miss = Region("00000000000000000000000000000003", 0, 100);
        var index = new RegionSpatialIndex();
        index.Rebuild([hitB, miss, hitA]);

        var actual = index.Query(new RegionSpatialBounds(-20, -20, 20, 20));

        Assert.Equal([hitA.RegionId, hitB.RegionId], actual.ToArray());
        Assert.Equal(3, index.Count);
    }

    [Fact]
    public void Full_map_region_has_bounded_single_node_storage()
    {
        var id = MapRegionId.New();
        var region = new MapRegion(id, MapLayerId.New(), "全图区域", MapRegionKind.Generic,
            [new(-500000, -500000), new(500000, -500000), new(0, 500000)]);
        var index = new RegionSpatialIndex();

        index.Upsert(region);

        Assert.Equal(1, index.NodeCount);
        Assert.Equal([id], index.Query(new(-1, -1, 1, 1)).ToArray());
    }

    [Fact]
    public void Query_bounds_reject_invalid_values_and_handle_far_finite_values()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new RegionSpatialBounds(double.NaN, 0, 1, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => new RegionSpatialBounds(0, 0, double.PositiveInfinity, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => new RegionSpatialBounds(0, 2, 1, 1));
        var index = new RegionSpatialIndex();
        Assert.Empty(index.Query(new(549755813632, 0, 549755813632, 0)));
    }

    [Fact]
    public void Invalid_upsert_preserves_existing_index_state()
    {
        var region = Region(MapRegionId.New().ToString(), 0, 0);
        var index = new RegionSpatialIndex();
        index.Upsert(region);
        var outside = region with { Vertices = [new(500001, 0), new(500010, 0), new(500001, 10)] };

        Assert.Throws<ArgumentOutOfRangeException>(() => index.Upsert(outside));

        Assert.Equal(1, index.Count);
        Assert.Equal([region.RegionId], index.Query(new(-20, -20, 20, 20)).ToArray());
    }

    [Fact]
    public void Index_is_derived_runtime_state_not_region_json()
    {
        var feature = MapRegionDatasetCodec.Write(Region(MapRegionId.New().ToString(), 0, 0));
        Assert.Equal(["geometry", "id", "properties"],
            feature.EnumerateObject().Select(item => item.Name).Order().ToArray());
        Assert.DoesNotContain(typeof(MapDefinition).GetProperties(), property =>
            property.Name.Contains("Spatial", StringComparison.Ordinal));
    }

    [Fact]
    public void Query_has_no_global_entity_or_full_map_scan_fallback()
    {
        var root = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor", "MapEditing");
        var index = string.Join('\n', Directory.GetFiles(root, "RegionSpatial*.cs").Select(File.ReadAllText));
        var session = File.ReadAllText(Path.Combine(root, "MapEditSession.RegionSpatialIndex.cs"));
        Assert.DoesNotContain("EntityId", index + session, StringComparison.Ordinal);
        Assert.DoesNotContain("_currentMap.Regions", index, StringComparison.Ordinal);
        Assert.Contains("TryGetRegion", session, StringComparison.Ordinal);
        Assert.Contains("return _regionSpatialIndex.Query(bounds);", session, StringComparison.Ordinal);
    }

    static MapRegion Region(string id, double x, double y) => new(
        MapRegionId.TryParse(id, out var regionId) ? regionId : MapRegionId.New(), MapLayerId.New(), "区域",
        MapRegionKind.Generic, [new(x - 10, y - 10), new(x + 10, y - 10), new(x, y + 10)]);
}

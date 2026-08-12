using System.Collections.Immutable;
using System.Text.Json;
using XuanYu.Editor.MapDocument;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map;

public sealed class MapRoadDatasetContractTests
{
    [Fact]
    public void Polyline_round_trip_preserves_stable_id_and_properties()
    {
        var id = MapRoadId.New();
        var road = new MapRoad(id, MapDatasetLayerIdProjection.Project("roads"), "主干道", "arterial", [new(-2, 0), new(0, 1), new(4, 1)]);
        var raw = MapRoadDatasetCodec.Write(road);
        var read = MapRoadDatasetCodec.Read(raw);
        Assert.True(read.Succeeded);
        Assert.Equal(id, read.Value!.RoadId);
        Assert.Equal(road.Points.ToArray(), read.Value.Points.ToArray());
        Assert.Equal("arterial", read.Value.Kind);
    }

    [Fact]
    public void Contract_rejects_duplicate_adjacent_nodes_and_accepts_legacy_region_version()
    {
        var road = new MapRoad(MapRoadId.New(), MapLayerId.New(), "道路", "generic", [new(0, 0), new(0, 0)]);
        var duplicate = MapRoadDatasetCodec.Read(MapRoadDatasetCodec.Write(road));
        Assert.Equal("AdjacentDuplicatePoint", duplicate.ErrorCode);
        var old = new MapDatasetDocument(MapDatasetDocument.CurrentFormat, "0.2.0", "regions", MapDatasetTypes.Region, ImmutableArray<JsonElement>.Empty);
        Assert.True(MapDatasetDocumentValidator.Validate(old).Succeeded);
    }

    [Fact]
    public void Road_domain_requires_two_points_and_map_layer()
    {
        var map = MapDefaultDefinition.CreateDefault();
        var invalid = new MapRoad(MapRoadId.New(), map.Layers[2].LayerId, "道路", "generic", [new(0, 0)]);
        var result = MapRoadValidator.Validate([invalid], map.Layers, map.SizeMeters);
        Assert.Equal("TooFewRoadPoints", result.ErrorCode);
    }
}

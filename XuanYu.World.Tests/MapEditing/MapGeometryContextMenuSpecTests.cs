using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

public sealed class MapGeometryContextMenuSpecTests
{
    [Fact]
    public void Region_vertex_menu_disables_delete_at_polygon_minimum()
    {
        var map = MapDefaultDefinition.CreateDefault();
        var region = new MapRegion(MapRegionId.New(), map.ActiveLayer(), "区域", MapRegionKind.Generic,
            [new(0, 0), new(1, 0), new(0, 1)]);
        var hit = new MapGeometryContextHit(MapGeometryContextKind.Vertex,
            new(MapGeometryFeatureKind.Region, region.RegionId.ToString()), 1, -1, 0);

        var items = MapGeometryContextMenuSpec.Build(map with { Regions = [region] }, hit);

        Assert.Contains(items, item => item.Id == "delete-vertex" && !item.IsEnabled);
    }

    [Fact]
    public void Road_edge_menu_offers_add_vertex_and_delete_road()
    {
        var map = MapDefaultDefinition.CreateDefault();
        var road = new MapRoad(MapRoadId.New(), map.ActiveLayer(), "道路", "generic", [new(0, 0), new(1, 1)]);
        var hit = new MapGeometryContextHit(MapGeometryContextKind.Edge,
            new(MapGeometryFeatureKind.Road, road.RoadId.ToString()), -1, 0, 0);

        var items = MapGeometryContextMenuSpec.Build(map with { Roads = [road] }, hit);

        Assert.Contains(items, item => item.Id == "add-vertex" && item.IsEnabled);
        Assert.Contains(items, item => item.Id == "delete-road" && item.IsEnabled);
    }
}

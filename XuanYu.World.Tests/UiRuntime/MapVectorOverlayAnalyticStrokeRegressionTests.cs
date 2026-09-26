using System.Collections.Immutable;
using XuanYu.Core.Math;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class MapVectorOverlayAnalyticStrokeRegressionTests
{
    [Theory]
    [InlineData(-10, -10, 10, -10)]
    [InlineData(-10, -10, 10, 10)]
    public void Draft_segment_keeps_one_screen_space_direction(
        double ax, double ay, double bx, double by)
    {
        var map = MapDefaultDefinition.CreateDefault();
        var drawing = new RegionDrawingState();
        drawing.Start(map.Layers[2].LayerId, "草稿", MapRegionKind.Generic);
        drawing.AddVertex(new(ax, ay)); drawing.AddVertex(new(bx, by));
        var resource = MapRegionRenderProjection.Build(map, drawing);
        var stroke = Assert.Single(resource.Primitives, x => x.Kind == RenderVectorOverlayPrimitiveKind.Stroke);
        AssertSegment(resource, stroke, new(ax, ay), new(bx, by), map.Surface.BaseHeightMeters);
    }

    [Fact]
    public void Closed_region_and_two_segment_road_keep_capsule_geometry_when_selected_or_not()
    {
        var map = MapDefaultDefinition.CreateDefault();
        var layer = map.Layers[2].LayerId;
        var region = new MapRegion(MapRegionId.New(), layer, "区域", MapRegionKind.Generic,
            [new(-20, -20), new(20, -20), new(20, 20), new(-20, 20)]);
        var road = new MapRoad(MapRoadId.New(), layer, "道路", "generic",
            [new(-30, 0), new(0, 15), new(30, 0)]);
        map = map with { Regions = [region], Roads = [road] };
        var plain = MapRegionRenderProjection.Build(map, new(), new());
        var regionStroke = Assert.Single(plain.Primitives, x => x.Kind == RenderVectorOverlayPrimitiveKind.Stroke && x.IndexCount == 24);
        var roadStroke = Assert.Single(plain.Primitives, x => x.Kind == RenderVectorOverlayPrimitiveKind.Stroke && x.IndexCount == 12);
        Assert.Equal(24, regionStroke.IndexCount); Assert.Equal(12, roadStroke.IndexCount);
        AssertSegmentSet(plain, regionStroke, region.Vertices, true);
        AssertSegmentSet(plain, roadStroke, road.Points, false);
        var selection = new MapGeometrySelection(MapGeometryFeatureKind.Region, region.RegionId.ToString());
        var selected = MapRegionRenderProjection.Build(map, new(), new(),
            new(selection, region.Vertices));
        Assert.Equal(new RenderStaticModelColor(.98, .75, .12, .98),
            Assert.Single(selected.Primitives, x => x.Kind == RenderVectorOverlayPrimitiveKind.Stroke && x.IndexCount == 24).Color);
    }

    static void AssertSegmentSet(RenderVectorOverlayResource resource,
        RenderVectorOverlayPrimitive primitive, IReadOnlyList<MapPoint> points, bool close)
    {
        var count = close ? points.Count : points.Count - 1;
        for (var i = 0; i < count; i++)
            AssertSegment(resource, primitive, points[i], points[(i + 1) % points.Count], 0, i * 6, 6);
    }

    static void AssertSegment(RenderVectorOverlayResource resource,
        RenderVectorOverlayPrimitive primitive, MapPoint a, MapPoint b, double height,
        int offset = 0, int count = -1)
    {
        var expectedA = MapCoordinateContract.MapToWorld(a, height);
        var expectedB = MapCoordinateContract.MapToWorld(b, height);
        var vertices = resource.Indices.Skip(primitive.FirstIndex + offset)
            .Take(count < 0 ? primitive.IndexCount : count)
            .Select(index => resource.Vertices[(int)index]);
        Assert.All(vertices, vertex => { Assert.Equal(expectedA, vertex.Position); Assert.Equal(expectedB, vertex.Secondary); });
    }
}

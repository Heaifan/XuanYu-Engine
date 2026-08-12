using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

public sealed class MapGeometryHitTesterTests
{
    [Fact]
    public void Region_body_and_vertex_are_hit_in_screen_space()
    {
        var map = MapDefaultDefinition.CreateDefault();
        var region = new MapRegion(MapRegionId.New(), map.ActiveLayer(), "区域", MapRegionKind.Generic,
            [new(-100, -100), new(100, -100), new(100, 100), new(-100, 100)]);
        map = map with { Regions = [region] };
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
        var projection = ViewProjectionState.Create(new CameraState(new Vector3d(0, 0, 1000),
            new Vector3d(0, 0, -1), Vector3d.UnitY, 60, 0.1, 5000, 1,
            ProjectionMode.Orthographic, 1200), viewport);
        var center = projection.ProjectWorldPoint(new Vector3d(0, 0, 0));
        var vertex = projection.ProjectWorldPoint(new Vector3d(-100, -100, 0));

        Assert.True(MapGeometryHitTester.TryHitFeature(map, projection, center.X, center.Y, 0, out var feature));
        Assert.Equal(region.RegionId.ToString(), feature.Selection.FeatureId);
        Assert.True(MapGeometryHitTester.TryHitVertex(map, feature.Selection, projection,
            vertex.X, vertex.Y, 10, 0, out var index));
        Assert.Equal(0, index);
    }
}

static class MapGeometryTestExtensions
{
    public static MapLayerId ActiveLayer(this MapDefinition map) => map.Layers[2].LayerId;
}

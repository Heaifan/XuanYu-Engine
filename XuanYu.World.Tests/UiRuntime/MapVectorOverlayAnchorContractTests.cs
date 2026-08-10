using System.Collections.Immutable;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class MapVectorOverlayAnchorContractTests
{
    [Fact]
    public void Fill_stroke_and_marker_share_exact_map_world_anchor()
    {
        const double height = 37.5;
        var map = MapDefaultDefinition.CreateDefault() with
        {
            Surface = MapDefaultDefinition.CreateDefault().Surface with { BaseHeightMeters = height }
        };
        var points = ImmutableArray.Create(
            new MapPoint(-10, -20), new MapPoint(30, -20), new MapPoint(0, 25));
        var region = new MapRegion(MapRegionId.New(), map.Layers[2].LayerId, "锚点区域",
            MapRegionKind.Generic, points);
        var drawing = new RegionDrawingState();
        drawing.Start(map.Layers[2].LayerId, "锚点草稿", MapRegionKind.Generic);
        foreach (var point in points) drawing.AddVertex(point);

        var resource = MapRegionRenderProjection.Build(map with { Regions = [region] }, drawing);
        var expected = points.Select(p => MapCoordinateContract.MapToWorld(p, height)).ToArray();
        var fill = Assert.Single(resource.Primitives, p => p.Kind == RenderVectorOverlayPrimitiveKind.Fill);
        var strokes = resource.Primitives.Where(p => p.Kind == RenderVectorOverlayPrimitiveKind.Stroke).ToArray();
        var markers = resource.Primitives.Where(p => p.Kind == RenderVectorOverlayPrimitiveKind.Marker).ToArray();
        var fillAnchors = Anchors(resource, fill);
        var strokeAnchors = strokes.SelectMany(p => Anchors(resource, p)).ToArray();
        var markerAnchors = markers.SelectMany(p => Anchors(resource, p)).ToArray();

        Assert.Equal(3, markers.Length);
        Assert.All(fillAnchors.Concat(strokeAnchors).Concat(markerAnchors), p => Assert.Equal(height, p.Z));
        foreach (var point in expected)
        {
            Assert.Contains(point, fillAnchors);
            Assert.Contains(point, strokeAnchors);
            Assert.Contains(point, markerAnchors);
        }
    }

    static IEnumerable<XuanYu.Core.Math.Vector3d> Anchors(
        RenderVectorOverlayResource resource, RenderVectorOverlayPrimitive primitive)
    {
        var indices = resource.Indices.Skip(primitive.FirstIndex).Take(primitive.IndexCount);
        foreach (var index in indices)
        {
            var vertex = resource.Vertices[(int)index];
            yield return vertex.Position;
            if (primitive.Kind == RenderVectorOverlayPrimitiveKind.Stroke)
                yield return vertex.Secondary;
        }
    }
}

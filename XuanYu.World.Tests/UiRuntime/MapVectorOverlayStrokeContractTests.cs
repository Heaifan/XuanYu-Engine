using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class MapVectorOverlayStrokeContractTests
{
    [Fact]
    public void Stroke_vertices_carry_capsule_segment_coordinates()
    {
        var resource = MapRegionRenderProjection.Build(MapDefaultDefinition.CreateDefault(), Draft());
        var stroke = Assert.Single(resource.Primitives, x => x.Kind == RenderVectorOverlayPrimitiveKind.Stroke);
        var vertices = resource.Indices.Skip(stroke.FirstIndex).Take(stroke.IndexCount)
            .Select(index => resource.Vertices[(int)index]).ToArray();
        Assert.All(vertices, vertex => Assert.Contains(vertex.U, new[] { -1d, 1d }));
        Assert.All(vertices, vertex => Assert.Contains(vertex.V, new[] { -1d, 2d }));
    }

    static RegionDrawingState Draft()
    {
        var state = new RegionDrawingState();
        var map = MapDefaultDefinition.CreateDefault();
        state.Start(map.Layers[2].LayerId, "草稿", MapRegionKind.Generic);
        state.AddVertex(new(-10, -10)); state.AddVertex(new(10, -10));
        return state;
    }
}

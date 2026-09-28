using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

public sealed partial class MapVectorOverlayV1Tests
{
    [Fact]
    public void V1_R11_region_uses_region_semantic_colors()
    {
        var map = MapDefaultDefinition.CreateDefault();
        var region = new MapRegion(MapRegionId.New(), map.Layers[2].LayerId, "区域1", MapRegionKind.Generic,
            [new(-10, -10), new(10, -10), new(10, 10)]);
        var resource = MapRegionRenderProjection.Build(map with { Regions = [region] }, new());
        var fill = Assert.Single(resource.Primitives, x => x.Kind == RenderVectorOverlayPrimitiveKind.Fill).Color;
        var rgb = region.FillColorRgb;
        Assert.Equal(((rgb >> 16) & 0xFF) / 255d, fill.R, 12);
        Assert.Equal(((rgb >> 8) & 0xFF) / 255d, fill.G, 12);
        Assert.Equal((rgb & 0xFF) / 255d, fill.B, 12);
        Assert.Equal(.32, fill.A, 12);
        Assert.Equal(new RenderStaticModelColor(.12, .38, .70, .92),
            Assert.Single(resource.Primitives, x => x.Kind == RenderVectorOverlayPrimitiveKind.Stroke).Color);
    }
}

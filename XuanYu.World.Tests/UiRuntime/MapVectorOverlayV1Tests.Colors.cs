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
        Assert.Equal(new RenderStaticModelColor(.20, .55, .90, .32),
            Assert.Single(resource.Primitives, x => x.Kind == RenderVectorOverlayPrimitiveKind.Fill).Color);
        Assert.Equal(new RenderStaticModelColor(.12, .38, .70, .92),
            Assert.Single(resource.Primitives, x => x.Kind == RenderVectorOverlayPrimitiveKind.Stroke).Color);
    }
}

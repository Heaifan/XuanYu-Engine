using System.Collections.Immutable;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class MapRegionLabelProjectionTests
{
    [Fact]
    public void Label_uses_display_name_and_interior_visual_center()
    {
        var points = new[] { new MapPoint(-100, -100), new(100, -100), new(100, 0),
            new(0, -20), new(100, 100), new(-100, 100) };
        var map = MapDefaultDefinition.CreateDefault();
        var region = new MapRegion(MapRegionId.New(), map.Layers[2].LayerId, "台湾北部", MapRegionKind.Generic,
            points.ToImmutableArray());
        var resource = MapRegionRenderProjection.Build(map with { Regions = [region] }, new());
        var label = Assert.Single(resource.LabelInstances);
        Assert.Equal("台湾北部", label.Text);
        Assert.True(label.Anchor.X < 0 || label.Anchor.Y > 0);
    }

    [Fact]
    public void Label_cache_key_changes_for_text_and_dpi()
    {
        var map = MapDefaultDefinition.CreateDefault();
        var region = new MapRegion(MapRegionId.New(), map.Layers[2].LayerId, "区域1", MapRegionKind.Generic,
            ImmutableArray.Create(new MapPoint(-10, -10), new(10, -10), new(10, 10), new(-10, 10)));
        var first = MapRegionRenderProjection.Build(map with { Regions = [region] }, new(), new(), null, 1);
        var second = MapRegionRenderProjection.Build(map with { Regions = [region with { DisplayName = "区域2" }] }, new(), new(), null, 1.5);
        Assert.NotEqual(Assert.Single(first.LabelInstances).CacheKey, Assert.Single(second.LabelInstances).CacheKey);
    }
}

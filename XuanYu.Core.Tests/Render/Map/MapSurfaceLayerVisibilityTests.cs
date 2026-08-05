using XuanYu.Core.Map;
using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render;

// MAP-A-R2-D4（R06）：图层显隐不进 GPU 资源判等键——显隐切换只推进序号，不重建资源。
public sealed class MapSurfaceLayerVisibilityTests
{
    static MapRenderSnapshot Snap(long seq) => new(
        "21e4a2d34d4a4a1eb2539eac76d412a8", 10000, 10000,
        MapSurfaceKind.Flat, 0, 0, 1, 1, seq);

    [Fact]
    public void R06_layer_visibility_toggle_does_not_rebuild()
    {
        var current = MapSurfaceResourceKey.From(Snap(1));
        var hidden = Snap(2) with { ShowGround = false, ShowBoundary = false };

        var update = MapSurfaceResourceUpdatePolicy.Decide(hidden, 1, current);

        Assert.Equal(MapSurfaceResourceUpdateKind.NoRebuild, update.Kind);
        Assert.Equal(current, update.Key);
    }

    [Fact]
    public void Ground_only_toggle_does_not_rebuild()
    {
        var current = MapSurfaceResourceKey.From(Snap(1));
        var hidden = Snap(2) with { ShowGround = false };

        var update = MapSurfaceResourceUpdatePolicy.Decide(hidden, 1, current);

        Assert.Equal(MapSurfaceResourceUpdateKind.NoRebuild, update.Kind);
    }
}

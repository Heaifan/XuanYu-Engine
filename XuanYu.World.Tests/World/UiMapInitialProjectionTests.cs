using XuanYu.Editor.UI;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.World;

// MAP-A-R2-D3-A1：默认地图初始快照进入首帧 RenderProjection（无需新建地图）。
public sealed class UiMapInitialProjectionTests
{
    [Fact]
    public void First_projection_carries_default_map_snapshot()
    {
        var vm = new UiVm(null, () => true);

        var map = vm.RenderProjection.Projection.Map;

        Assert.True(map.HasMap);
        Assert.Equal(10000.0, map.WidthMeters);
        Assert.Equal(10000.0, map.DepthMeters);
        Assert.Equal(0.0, map.BaseHeightMeters);
        Assert.Equal(Core.Map.MapSurfaceKind.Flat, map.SurfaceKind);
        Assert.True(map.IsVisible);
    }

    [Fact]
    public void First_projection_matches_session_current_map()
    {
        var vm = new UiVm(null, () => true);

        var map = vm.RenderProjection.Projection.Map;
        var session = vm.MapSession.CurrentMap;

        Assert.Equal(session.MapId.Value, map.MapId);
        Assert.Equal(session.SizeMeters.Width, map.WidthMeters);
        Assert.Equal(session.Surface.BaseHeightMeters, map.BaseHeightMeters);
    }

    [Fact]
    public void Projection_keeps_default_map_without_new_map_command()
    {
        var vm = new UiVm(null, () => true);

        var first = vm.RenderProjection.Projection.Map;
        var second = vm.RenderProjection.Projection.Map;

        Assert.True(first.HasMap);
        Assert.Equal(first, second); // 无修改时投影稳定，无双地图状态
    }
}

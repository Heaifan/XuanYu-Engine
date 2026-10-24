using XuanYu.Core.Map;
using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render;

// MAP-A-R2-D3-A1：GPU 资源判等键合同——Rename 不重建、几何变化必重建、Sequence 不进键。
public sealed class MapSurfaceResourceKeyTests
{
    static MapRenderSnapshot Snap(long seq, double width = 10000, double depth = 10000,
        double baseHeight = 0, string mapId = "21e4a2d34d4a4a1eb2539eac76d412a8") => new(
        mapId, width, depth, MapSurfaceKind.Flat, baseHeight, 0, 1, 1, seq);

    [Fact]
    public void Rename_snapshot_has_same_key()
    {
        var a = MapSurfaceResourceKey.From(Snap(1));
        var b = MapSurfaceResourceKey.From(Snap(2)); // Rename 只推进 Sequence

        Assert.Equal(a, b);
    }

    [Fact]
    public void Resize_changes_key()
    {
        var a = MapSurfaceResourceKey.From(Snap(1, 10000, 10000));
        var b = MapSurfaceResourceKey.From(Snap(2, 20000, 8000));

        Assert.NotEqual(a, b);
    }

    [Fact]
    public void Base_height_changes_key()
    {
        var a = MapSurfaceResourceKey.From(Snap(1, baseHeight: 0));
        var b = MapSurfaceResourceKey.From(Snap(2, baseHeight: 100));

        Assert.NotEqual(a, b);
    }

    [Fact]
    public void Map_id_changes_key()
    {
        var a = MapSurfaceResourceKey.From(Snap(1));
        var b = MapSurfaceResourceKey.From(Snap(2, mapId: "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"));

        Assert.NotEqual(a, b);
    }

    [Fact]
    public void Sequence_never_enters_key()
    {
        var a = MapSurfaceResourceKey.From(Snap(0));
        var b = MapSurfaceResourceKey.From(Snap(999));

        Assert.Equal(a, b);
    }
}

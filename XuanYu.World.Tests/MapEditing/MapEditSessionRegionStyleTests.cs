using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

public sealed class MapEditSessionRegionStyleTests
{
    [Fact]
    public void Region_fill_color_is_undoable_and_redoable()
    {
        var session = new MapEditSession();
        var region = new MapRegion(
            MapRegionId.New(),
            session.ActiveRegionLayerId,
            "区域",
            MapRegionKind.Generic,
            [new(0, 0), new(100, 0), new(100, 100)]);

        Assert.True(session.CreateRegion(region).IsSuccess);
        Assert.Equal(MapRegion.DefaultFillColorRgb, session.CurrentMap.Regions[0].FillColorRgb);

        Assert.True(session.SetRegionFillColor(region.RegionId, 0x00CC6633).IsSuccess);
        Assert.Equal(0x00CC6633u, session.CurrentMap.Regions[0].FillColorRgb);

        Assert.True(session.Undo().IsSuccess);
        Assert.Equal(MapRegion.DefaultFillColorRgb, session.CurrentMap.Regions[0].FillColorRgb);

        Assert.True(session.Redo().IsSuccess);
        Assert.Equal(0x00CC6633u, session.CurrentMap.Regions[0].FillColorRgb);
    }
}

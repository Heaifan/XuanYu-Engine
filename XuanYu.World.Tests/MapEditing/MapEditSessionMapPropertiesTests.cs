using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

// MAP-A-R2-D3-A1：地图属性原子提交（单历史节点/失败零污染）。
public sealed class MapEditSessionMapPropertiesTests
{
    static MapEditSession Session() => new();
    [Fact]
    public void Update_properties_commits_atomically_with_single_history()
    {
        var session = Session();
        var events = 0;
        session.ContentChanged += _ => events++;
        var result = session.UpdateMapProperties(20000, 8000, 100);
        Assert.True(result.IsSuccess);
        Assert.Equal(20000.0, session.CurrentMap.SizeMeters.Width);
        Assert.Equal(8000.0, session.CurrentMap.SizeMeters.Depth);
        Assert.Equal(100.0, session.CurrentMap.Surface.BaseHeightMeters);
        Assert.Equal(1, events);
        Assert.Equal(1, session.ChangeSequence);
        Assert.True(session.CanUndo);
        Assert.False(session.CanRedo);
    }
    [Fact]
    public void Single_undo_restores_all_three_fields()
    {
        var session = Session();
        session.UpdateMapProperties(20000, 8000, 100);
        Assert.True(session.Undo().IsSuccess);
        Assert.Equal(10000.0, session.CurrentMap.SizeMeters.Width); Assert.Equal(10000.0, session.CurrentMap.SizeMeters.Depth);
        Assert.Equal(0.0, session.CurrentMap.Surface.BaseHeightMeters); Assert.False(session.CanUndo);
    }
    [Fact]
    public void Single_redo_restores_all_three_fields()
    {
        var session = Session();
        session.UpdateMapProperties(20000, 8000, 100);
        session.Undo();
        Assert.True(session.Redo().IsSuccess);
        Assert.Equal(20000.0, session.CurrentMap.SizeMeters.Width); Assert.Equal(8000.0, session.CurrentMap.SizeMeters.Depth);
        Assert.Equal(100.0, session.CurrentMap.Surface.BaseHeightMeters); Assert.False(session.CanRedo);
    }
    [Fact]
    public void NaN_base_height_rejected_with_zero_pollution()
    {
        var session = Session();
        var sequenceBefore = session.ChangeSequence;
        Assert.False(session.UpdateMapProperties(20000, 8000, double.NaN).IsSuccess);
        Assert.Equal(10000.0, session.CurrentMap.SizeMeters.Width); Assert.Equal(10000.0, session.CurrentMap.SizeMeters.Depth);
        Assert.Equal(0.0, session.CurrentMap.Surface.BaseHeightMeters); Assert.Equal(sequenceBefore, session.ChangeSequence);
        Assert.False(session.CanUndo);
    }
    [Fact]
    public void Infinity_size_rejected()
    {
        var session = Session();
        Assert.False(session.UpdateMapProperties(double.PositiveInfinity, 8000, 100).IsSuccess);
        Assert.Equal(10000.0, session.CurrentMap.SizeMeters.Width);
        Assert.False(session.CanUndo);
    }
    [Fact]
    public void Invalid_size_rejects_whole_candidate()
    {
        var session = Session();
        Assert.False(session.UpdateMapProperties(50, 8000, 100).IsSuccess);
        Assert.Equal(10000.0, session.CurrentMap.SizeMeters.Width); Assert.Equal(10000.0, session.CurrentMap.SizeMeters.Depth);
        Assert.Equal(0.0, session.CurrentMap.Surface.BaseHeightMeters); Assert.False(session.CanUndo);
    }
    [Fact]
    public void Shrinking_over_region_rejected_without_crop_or_move()
    {
        var session = Session();
        var region = new MapRegion(MapRegionId.New(), session.CurrentMap.Layers[1].LayerId,
            "主区域", MapRegionKind.Generic,
            [new MapPoint(-3000, 0), new MapPoint(3000, 0), new MapPoint(0, 3000)]);
        session.ReplaceCurrentMap(session.CurrentMap with { Regions = [region] }, markSaved: false, path: null);
        Assert.False(session.UpdateMapProperties(2000, 2000, 0).IsSuccess);
        Assert.Equal(10000.0, session.CurrentMap.SizeMeters.Width);
        Assert.Equal(10000.0, session.CurrentMap.SizeMeters.Depth);
        Assert.Single(session.CurrentMap.Regions);
        Assert.Equal(region.RegionId, session.CurrentMap.Regions[0].RegionId);
        Assert.Equal(-3000.0, session.CurrentMap.Regions[0].Vertices[0].X);
    }
    [Fact]
    public void No_op_identical_values_succeed_without_history()
    {
        var session = Session();
        var events = 0;
        session.ContentChanged += _ => events++;
        Assert.True(session.UpdateMapProperties(10000, 10000, 0).IsSuccess);
        Assert.Equal(0, events);
        Assert.False(session.CanUndo);
        Assert.Equal(0, session.ChangeSequence);
    }
}

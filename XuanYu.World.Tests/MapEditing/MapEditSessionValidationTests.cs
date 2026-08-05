using System.Collections.Immutable;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

// MAP-A-R2-D2：候选校验与失败不污染（缩小越界整体拒绝/无效替换拒绝）。
public sealed class MapEditSessionValidationTests
{
    static MapDefinition MapWithRegion()
    {
        var map = MapDefaultDefinition.CreateDefault();
        var region = new MapRegion(
            MapRegionId.New(), map.Layers[2].LayerId, "部署区", MapRegionKind.Deployment,
            ImmutableArray.Create(
                new MapPoint(-100, -100), new MapPoint(100, -100),
                new MapPoint(100, 100), new MapPoint(-100, 100)));
        return map with { Regions = [region] };
    }

    [Fact]
    public void Resize_causing_region_out_of_bounds_rejected()
    {
        var session = new MapEditSession(MapWithRegion());
        var before = session.CurrentMap;
        var stateId = session.CurrentStateId;
        var sequence = session.ChangeSequence;

        var result = session.ResizeMap(100.0, 100.0);
        Assert.True(result.IsFailure);
        Assert.Equal("RegionWouldBeOutOfBounds", result.Error!.Value.Code);
        Assert.Same(before, session.CurrentMap); // 不自动裁剪/不移动区域/整体拒绝
        Assert.Equal(stateId, session.CurrentStateId);
        Assert.Equal(sequence, session.ChangeSequence);
        Assert.False(session.CanUndo);
        Assert.Equal(MapSelectionKind.Map, session.Selection.Kind);
    }

    [Fact]
    public void Replace_invalid_map_rejected()
    {
        var session = new MapEditSession();
        var invalid = session.CurrentMap with { SizeMeters = new MapSize(10.0, 10.0) };
        var result = session.ReplaceCurrentMap(invalid, markSaved: false, path: null);
        Assert.True(result.IsFailure);
        Assert.Equal("InvalidMap", result.Error!.Value.Code);
        Assert.Equal(10000.0, session.CurrentMap.SizeMeters.Width);
    }

    [Fact]
    public void Replace_valid_map_marks_saved_when_requested()
    {
        var session = new MapEditSession();
        var candidate = session.CurrentMap with { DisplayName = "已保存地图" };
        Assert.True(session.ReplaceCurrentMap(candidate, markSaved: true, path: @"D:\m.xymap").IsSuccess);
        Assert.Equal("已保存地图", session.CurrentMap.DisplayName);
        Assert.False(session.IsDirty);
        Assert.Equal(@"D:\m.xymap", session.CurrentFilePath);
    }

    [Fact]
    public void Replace_unmarks_saved_when_not_requested()
    {
        var session = new MapEditSession();
        var candidate = session.CurrentMap with { DisplayName = "候选地图" };
        session.ReplaceCurrentMap(candidate, markSaved: false, path: null);
        Assert.True(session.IsDirty);
        Assert.Null(session.CurrentFilePath);
        Assert.Null(session.SavedStateId);
    }
}

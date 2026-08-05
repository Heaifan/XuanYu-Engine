using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map.Editing;

// MAP-A-R2-D4-F3：拖动排序会话行为（H04 No-op / H05 失败零污染 / H06 活动图层保持）。
public sealed partial class MapLayerSessionTests
{
    [Fact]
    public void H04_same_position_noop_no_dirty_no_history()
    {
        var session = NewSession();
        session.MarkSaved("dummy.xymap");
        var id = AddSecondRegion(session);
        session.MarkSaved("dummy.xymap");
        var stateId = session.CurrentStateId;
        var canUndo = session.CanUndo;
        var index = MapLayerRules.IndexOfId(MapLayerStack.RegionLayers(session.CurrentMap.Layers), id);
        Assert.True(session.MoveLayerToRegionIndex(id, index).IsSuccess);
        Assert.Equal(stateId, session.CurrentStateId);
        Assert.False(session.IsDirty);
        Assert.Equal(canUndo, session.CanUndo); // No-op 不新增历史
    }

    [Fact]
    public void H05_failures_do_not_pollute_history()
    {
        var session = NewSession();
        session.AddRegionLayer();
        var stateId = session.CurrentStateId;
        var canUndo = session.CanUndo;
        Assert.False(session.MoveLayerToRegionIndex(MapLayerId.New(), 0).IsSuccess);
        Assert.False(session.MoveLayerToRegionIndex(session.CurrentMap.Layers[0].LayerId, 0).IsSuccess);
        Assert.False(session.MoveLayerToRegionIndex(
            MapLayerStack.RegionLayers(session.CurrentMap.Layers)[0].LayerId, 9).IsSuccess);
        Assert.False(session.MoveLayerToRegionIndex(
            MapLayerStack.RegionLayers(session.CurrentMap.Layers)[0].LayerId, -1).IsSuccess);
        Assert.Equal(stateId, session.CurrentStateId);
        Assert.Equal(canUndo, session.CanUndo); // 失败不新增历史
    }

    [Fact]
    public void H06_active_layer_kept_after_drag()
    {
        var session = NewSession();
        var id = AddSecondRegion(session);
        session.SetActiveRegionLayer(id);
        session.MoveLayerToRegionIndex(id, 0);
        Assert.Equal(id, session.ActiveRegionLayerId);
    }
}

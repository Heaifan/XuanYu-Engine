using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map.Editing;

// MAP-A-R2-D4：图层命令会话行为（T02 默认活动图层 + H07～H10、活动转移、No-op）。
public sealed partial class MapLayerSessionTests
{
    [Fact]
    public void T02_default_active_layer_is_first_region_layer()
    {
        var session = NewSession();
        Assert.Equal(FirstRegion(session).LayerId, session.ActiveRegionLayerId);
    }

    [Fact]
    public void H07_content_changes_set_dirty()
    {
        var session = NewSession();
        session.MarkSaved("dummy.xymap");
        Assert.False(session.IsDirty);
        session.AddRegionLayer();
        Assert.True(session.IsDirty);
        session.MarkSaved("dummy.xymap");
        session.SetLayerVisibility(FirstRegion(session).LayerId, false);
        Assert.True(session.IsDirty);
        session.MarkSaved("dummy.xymap");
        session.RenameLayer(FirstRegion(session).LayerId, "战区");
        Assert.True(session.IsDirty);
        session.MarkSaved("dummy.xymap");
        session.SetLayerLocked(FirstRegion(session).LayerId, true);
        Assert.True(session.IsDirty);
    }

    [Fact]
    public void H08_set_active_layer_does_not_set_dirty()
    {
        var session = NewSession();
        session.MarkSaved("dummy.xymap");
        session.AddRegionLayer();
        session.MarkSaved("dummy.xymap");
        var newLayer = session.CurrentMap.Layers.Last();
        Assert.True(session.SetActiveRegionLayer(newLayer.LayerId).IsSuccess);
        Assert.False(session.IsDirty);
        Assert.Equal(newLayer.LayerId, session.ActiveRegionLayerId);
    }

    [Fact]
    public void H09_invalid_operations_do_not_pollute_history()
    {
        var session = NewSession();
        var stateId = session.CurrentStateId;
        Assert.False(session.RenameLayer(MapLayerId.New(), "不存在").IsSuccess);
        Assert.False(session.RenameLayer(FirstRegion(session).LayerId, "  ").IsSuccess);
        Assert.False(session.RemoveLayer(FirstRegion(session).LayerId).IsSuccess);
        Assert.False(session.RemoveLayer(session.CurrentMap.Layers[0].LayerId).IsSuccess);
        Assert.False(session.RenameLayer(session.CurrentMap.Layers[0].LayerId, "改名").IsSuccess);
        Assert.False(session.MoveLayerUp(MapLayerStack.RegionLayers(session.CurrentMap.Layers)[0].LayerId).IsSuccess);
        Assert.False(session.SetActiveRegionLayer(session.CurrentMap.Layers[0].LayerId).IsSuccess);
        Assert.False(session.CanUndo);
        Assert.Equal(stateId, session.CurrentStateId);
    }

    [Fact]
    public void H10_undo_keeps_active_layer_valid()
    {
        var session = NewSession();
        session.AddRegionLayer();
        var added = session.CurrentMap.Layers.Last();
        Assert.Equal(added.LayerId, session.ActiveRegionLayerId);
        session.Undo();
        Assert.Contains(session.CurrentMap.Layers,
            l => l.LayerId == session.ActiveRegionLayerId && l.Kind == MapLayerKind.Region);
    }

    [Fact]
    public void Remove_active_layer_transfers_to_neighbor()
    {
        var session = NewSession();
        session.AddRegionLayer();
        var regions = MapLayerStack.RegionLayers(session.CurrentMap.Layers);
        var middle = regions[0];
        session.SetActiveRegionLayer(middle.LayerId);
        Assert.True(session.RemoveLayer(middle.LayerId).IsSuccess);
        var remaining = MapLayerStack.RegionLayers(session.CurrentMap.Layers);
        Assert.Equal(remaining[0].LayerId, session.ActiveRegionLayerId);
    }

    [Fact]
    public void Same_value_change_is_noop_without_history()
    {
        var session = NewSession();
        session.MarkSaved("dummy.xymap");
        var stateId = session.CurrentStateId;
        Assert.True(session.SetLayerVisibility(FirstRegion(session).LayerId, true).IsSuccess);
        Assert.Equal(stateId, session.CurrentStateId);
        Assert.False(session.CanUndo);
        Assert.False(session.IsDirty);
    }
}

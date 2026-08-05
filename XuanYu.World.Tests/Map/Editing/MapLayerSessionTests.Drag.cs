using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map.Editing;

// MAP-A-R2-D4-F3：拖动排序会话命令（H01～H03 单历史节点与 Undo/Redo）。
public sealed partial class MapLayerSessionTests
{
    static MapLayerId AddSecondRegion(MapEditSession session)
    {
        session.AddRegionLayer();
        return session.CurrentMap.Layers.Last().LayerId;
    }

    [Fact]
    public void H01_one_drag_produces_one_history_entry()
    {
        var session = NewSession();
        var id = AddSecondRegion(session);
        session.AddRegionLayer(); // 区域 3 位于最上方
        var stateId = session.CurrentStateId;
        Assert.True(session.MoveLayerToRegionIndex(id, 0).IsSuccess);
        Assert.Equal(stateId + 1, session.CurrentStateId);
    }

    [Fact]
    public void H02_undo_restores_drag_before_order()
    {
        var session = NewSession();
        var id = AddSecondRegion(session);
        session.AddRegionLayer(); // 区域 3 位于最上方
        var before = MapLayerStack.RegionLayers(session.CurrentMap.Layers)
            .Select(l => l.LayerId).ToArray();
        session.MoveLayerToRegionIndex(id, 0);
        session.Undo();
        var restored = MapLayerStack.RegionLayers(session.CurrentMap.Layers)
            .Select(l => l.LayerId).ToArray();
        Assert.Equal(before, restored);
    }

    [Fact]
    public void H03_redo_restores_drag_after_order()
    {
        var session = NewSession();
        var id = AddSecondRegion(session);
        session.AddRegionLayer(); // 区域 3 位于最上方
        session.MoveLayerToRegionIndex(id, 0);
        var after = MapLayerStack.RegionLayers(session.CurrentMap.Layers)
            .Select(l => l.LayerId).ToArray();
        session.Undo();
        session.Redo();
        var redone = MapLayerStack.RegionLayers(session.CurrentMap.Layers)
            .Select(l => l.LayerId).ToArray();
        Assert.Equal(after, redone);
    }
}

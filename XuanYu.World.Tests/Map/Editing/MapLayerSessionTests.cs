using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map.Editing;

// MAP-A-R2-D4：图层命令接入 MapEditSession（H01～H06 撤销/重做；T02 见 Behavior）。
public sealed partial class MapLayerSessionTests
{
    static MapEditSession NewSession() => new(isWriteThread: () => true);

    static MapLayer FirstRegion(MapEditSession session) =>
        MapLayerStack.RegionLayers(session.CurrentMap.Layers)[0];

    [Fact]
    public void H01_add_layer_undo_redo()
    {
        var session = NewSession();
        Assert.True(session.AddRegionLayer().IsSuccess);
        Assert.Equal(2, session.CurrentMap.Layers.Count(l => l.Kind == MapLayerKind.Region));
        var added = session.CurrentMap.Layers.Last();
        Assert.Equal("区域 2", added.DisplayName);
        Assert.Equal(added.LayerId, session.ActiveRegionLayerId);
        Assert.True(session.Undo().IsSuccess);
        Assert.Single(session.CurrentMap.Layers.Where(l => l.Kind == MapLayerKind.Region));
        Assert.Equal(FirstRegion(session).LayerId, session.ActiveRegionLayerId);
        Assert.True(session.Redo().IsSuccess);
        Assert.Equal(added.LayerId, session.CurrentMap.Layers.Last().LayerId);
    }

    [Fact]
    public void H02_rename_undo_redo()
    {
        var session = NewSession();
        var layer = FirstRegion(session);
        Assert.True(session.RenameLayer(layer.LayerId, "主战区").IsSuccess);
        Assert.Equal("主战区", session.CurrentMap.Layers[2].DisplayName);
        Assert.True(session.Undo().IsSuccess);
        Assert.Equal("区域 1", session.CurrentMap.Layers[2].DisplayName);
        Assert.True(session.Redo().IsSuccess);
        Assert.Equal("主战区", session.CurrentMap.Layers[2].DisplayName);
    }

    [Fact]
    public void H03_remove_undo_restores_same_id()
    {
        var session = NewSession();
        session.AddRegionLayer();
        var removed = session.CurrentMap.Layers.Last();
        var removedId = removed.LayerId;
        var removedOrder = removed.Order;
        Assert.True(session.RemoveLayer(removedId).IsSuccess);
        Assert.DoesNotContain(session.CurrentMap.Layers, l => l.LayerId == removedId);
        Assert.True(session.Undo().IsSuccess);
        var restored = session.CurrentMap.Layers.First(l => l.LayerId == removedId);
        Assert.Equal(removedId, restored.LayerId);
        Assert.Equal(removedOrder, restored.Order);
        Assert.Equal("区域 2", restored.DisplayName);
        Assert.True(session.Redo().IsSuccess);
        Assert.DoesNotContain(session.CurrentMap.Layers, l => l.LayerId == removedId);
    }
    [Fact]
    public void H04_move_undo_redo()
    {
        var session = NewSession();
        session.AddRegionLayer();
        session.AddRegionLayer();
        var top = MapLayerStack.RegionLayers(session.CurrentMap.Layers)[0];
        var topOrder = top.Order;
        Assert.True(session.MoveLayerDown(top.LayerId).IsSuccess);
        Assert.Equal(topOrder - 1, session.CurrentMap.Layers.First(l => l.LayerId == top.LayerId).Order);
        Assert.True(session.Undo().IsSuccess);
        Assert.Equal(topOrder, session.CurrentMap.Layers.First(l => l.LayerId == top.LayerId).Order);
        Assert.True(session.Redo().IsSuccess);
        Assert.Equal(topOrder - 1, session.CurrentMap.Layers.First(l => l.LayerId == top.LayerId).Order);
    }
    [Fact]
    public void H05_visibility_undo_redo()
    {
        var session = NewSession();
        var layer = FirstRegion(session);
        Assert.True(session.SetLayerVisibility(layer.LayerId, false).IsSuccess);
        Assert.False(session.CurrentMap.Layers[2].IsVisible);
        Assert.True(session.Undo().IsSuccess);
        Assert.True(session.CurrentMap.Layers[2].IsVisible);
        Assert.True(session.Redo().IsSuccess);
        Assert.False(session.CurrentMap.Layers[2].IsVisible);
    }
    [Fact]
    public void H06_lock_undo_redo()
    {
        var session = NewSession();
        var layer = FirstRegion(session);
        Assert.True(session.SetLayerLocked(layer.LayerId, true).IsSuccess);
        Assert.True(session.CurrentMap.Layers[2].IsLocked);
        Assert.True(session.Undo().IsSuccess);
        Assert.False(session.CurrentMap.Layers[2].IsLocked);
        Assert.True(session.Redo().IsSuccess);
        Assert.True(session.CurrentMap.Layers[2].IsLocked);
    }
}

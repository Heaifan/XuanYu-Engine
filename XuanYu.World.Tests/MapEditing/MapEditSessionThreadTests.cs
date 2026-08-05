using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

// MAP-A-R2-D2：写线程保护（非法线程拒绝且状态完全不变）。
public sealed class MapEditSessionThreadTests
{
    static MapEditSession BlockedSession() => new(isWriteThread: () => false);

    [Fact]
    public void Rename_blocked_off_write_thread()
    {
        var session = BlockedSession();
        var result = session.RenameMap("战场B");
        Assert.True(result.IsFailure);
        Assert.Equal("NotOnWriteThread", result.Error!.Value.Code);
        Assert.Equal("未命名地图", session.CurrentMap.DisplayName);
        Assert.False(session.CanUndo);
    }

    [Fact]
    public void Resize_blocked_off_write_thread()
    {
        var session = BlockedSession();
        Assert.True(session.ResizeMap(20000.0, 8000.0).IsFailure);
        Assert.Equal(10000.0, session.CurrentMap.SizeMeters.Width);
    }

    [Fact]
    public void Undo_and_redo_blocked_off_write_thread()
    {
        var session = BlockedSession();
        Assert.Equal("NotOnWriteThread", session.Undo().Error!.Value.Code);
        Assert.Equal("NotOnWriteThread", session.Redo().Error!.Value.Code);
    }

    [Fact]
    public void New_map_and_replace_blocked_off_write_thread()
    {
        var session = BlockedSession();
        Assert.Equal("NotOnWriteThread", session.CreateNewMap().Error!.Value.Code);
        Assert.Equal("NotOnWriteThread",
            session.ReplaceCurrentMap(MapDefaultDefinition.CreateDefault(), false, null).Error!.Value.Code);
        Assert.Equal("NotOnWriteThread", session.MarkSaved(@"D:\m.xymap").Error!.Value.Code);
    }

    [Fact]
    public void Selection_blocked_off_write_thread()
    {
        var session = BlockedSession();
        var layerId = session.CurrentMap.Layers[2].LayerId;
        Assert.Equal("NotOnWriteThread", session.SelectLayer(layerId).Error!.Value.Code);
        Assert.Equal("NotOnWriteThread", session.SelectMap().Error!.Value.Code);
        Assert.Equal("NotOnWriteThread", session.ClearSelection().Error!.Value.Code);
        Assert.Equal(MapSelectionKind.Map, session.Selection.Kind);
    }

    [Fact]
    public void Read_state_is_safe_off_write_thread()
    {
        var session = BlockedSession();
        Assert.NotNull(session.CurrentMap);
        Assert.True(session.IsDirty); // 无路径 → 恒为 Dirty，与线程无关
        Assert.False(session.CanUndo);
        Assert.False(session.CanRedo);
    }
}

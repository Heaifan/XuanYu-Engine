using XuanYu.Editor.MapEditing;

namespace XuanYu.World.Tests.MapEditing;

// MAP-A-R2-D2：Saved/Dirty 合同（Dirty 随 Undo/Redo 回到保存点）。
public sealed class MapEditSessionDirtyTests
{
    [Fact]
    public void Dirty_follows_saved_point_roundtrip()
    {
        var session = new MapEditSession();
        Assert.True(session.MarkSaved(@"D:\map.xymap").IsSuccess);
        Assert.False(session.IsDirty);

        session.RenameMap("战场B");
        Assert.True(session.IsDirty);

        session.Undo();
        Assert.False(session.IsDirty); // 回到保存点

        session.Redo();
        Assert.True(session.IsDirty); // 离开保存点
    }

    [Fact]
    public void Save_after_edit_then_undo_redo()
    {
        var session = new MapEditSession();
        session.RenameMap("战场B");
        Assert.True(session.MarkSaved(@"D:\map.xymap").IsSuccess);
        Assert.False(session.IsDirty);

        session.Undo(); // 回到 A
        Assert.True(session.IsDirty);

        session.Redo(); // 回到保存点 B
        Assert.False(session.IsDirty);
    }

    [Fact]
    public void New_map_resets_saved_state()
    {
        var session = new MapEditSession();
        session.RenameMap("战场B");
        session.MarkSaved(@"D:\map.xymap");
        Assert.False(session.IsDirty);

        session.CreateNewMap();
        Assert.True(session.IsDirty);
        Assert.Null(session.CurrentFilePath);
        Assert.Null(session.SavedStateId);
        Assert.False(session.CanUndo); // 不允许 Undo 回旧文档
        Assert.Equal("未命名地图", session.CurrentMap.DisplayName);
    }

    [Fact]
    public void Untitled_map_is_always_dirty()
    {
        var session = new MapEditSession();
        Assert.True(session.IsDirty);
        session.Undo();
        Assert.True(session.IsDirty);
    }
}

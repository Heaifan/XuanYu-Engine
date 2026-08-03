using XuanYu.Editor.MapEditing;

namespace XuanYu.World.Tests.MapEditing;

// MAP-A-R2-D2：Undo/Redo、分支清除与 ChangeSequence 单调递增。
public sealed class MapEditSessionHistoryTests
{
    [Fact]
    public void Undo_redo_cycle_restores_content()
    {
        var session = new MapEditSession();
        session.RenameMap("战场B");
        session.RenameMap("战场C");
        Assert.Equal("战场C", session.CurrentMap.DisplayName);

        Assert.True(session.Undo().IsSuccess);
        Assert.Equal("战场B", session.CurrentMap.DisplayName);
        Assert.True(session.Undo().IsSuccess);
        Assert.Equal("未命名地图", session.CurrentMap.DisplayName);
        Assert.False(session.CanUndo);

        Assert.True(session.Redo().IsSuccess);
        Assert.Equal("战场B", session.CurrentMap.DisplayName);
        Assert.True(session.Redo().IsSuccess);
        Assert.Equal("战场C", session.CurrentMap.DisplayName);
        Assert.False(session.CanRedo);
    }

    [Fact]
    public void Change_sequence_increases_on_every_observable_change()
    {
        var session = new MapEditSession();
        Assert.Equal(0, session.ChangeSequence);
        session.RenameMap("战场B");
        Assert.Equal(1, session.ChangeSequence);
        session.Undo();
        Assert.Equal(2, session.ChangeSequence);
        session.Redo();
        Assert.Equal(3, session.ChangeSequence);
    }

    [Fact]
    public void New_commit_clears_redo_branch()
    {
        var session = new MapEditSession();
        session.RenameMap("战场B");
        session.RenameMap("战场C");
        session.Undo(); // 回到 B
        Assert.True(session.CanRedo);

        session.RenameMap("战场D"); // 新分支
        Assert.Equal("战场D", session.CurrentMap.DisplayName);
        Assert.False(session.CanRedo); // C 不再可达

        session.Undo();
        Assert.Equal("战场B", session.CurrentMap.DisplayName);
        Assert.True(session.CanRedo);
        session.Redo();
        Assert.Equal("战场D", session.CurrentMap.DisplayName);
    }

    [Fact]
    public void Undo_empty_returns_error_without_change()
    {
        var session = new MapEditSession();
        var result = session.Undo();
        Assert.True(result.IsFailure);
        Assert.Equal("NoUndoAvailable", result.Error!.Value.Code);
        Assert.Equal("未命名地图", session.CurrentMap.DisplayName);
    }

    [Fact]
    public void Redo_empty_returns_error_without_change()
    {
        var session = new MapEditSession();
        session.RenameMap("战场B");
        var result = session.Redo();
        Assert.True(result.IsFailure);
        Assert.Equal("NoRedoAvailable", result.Error!.Value.Code);
        Assert.Equal("战场B", session.CurrentMap.DisplayName);
    }
}

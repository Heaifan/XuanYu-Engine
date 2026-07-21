using XuanYu.Core.History;
using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;

namespace XuanYu.Core.Tests.History;

public sealed class EditorHistoryRedoTests
{
    [Fact]
    public void Redo_restores_undone_entry_and_keeps_history_cursor()
    {
        var history = new EditorHistoryOwner();
        var entity = EntityId.FromInt(1);
        var entry = Entry(entity, 0, 1);

        history.Push(entry);
        Assert.True(history.TryUndo(out var undone));
        Assert.Equal(entry, undone);
        Assert.Equal(0, history.Count);
        Assert.Equal(1, history.RedoCount);

        Assert.True(history.TryRedo(out var redone));
        Assert.Equal(entry, redone);
        Assert.Equal(1, history.Count);
        Assert.Equal(0, history.RedoCount);
    }

    [Fact]
    public void Redo_replays_multiple_undone_entries_in_order()
    {
        var history = new EditorHistoryOwner();
        var entity = EntityId.FromInt(1);

        history.Push(Entry(entity, 0, 1));
        history.Push(Entry(entity, 1, 2));
        Assert.True(history.TryUndo(out var second));
        Assert.True(history.TryUndo(out var first));

        Assert.True(history.TryRedo(out var redoFirst));
        Assert.Equal(first, redoFirst);
        Assert.True(history.TryRedo(out var redoSecond));
        Assert.Equal(second, redoSecond);
        Assert.False(history.TryRedo(out _));
    }

    [Fact]
    public void New_commit_clears_redo_branch()
    {
        var history = new EditorHistoryOwner();
        var entity = EntityId.FromInt(1);

        history.Push(Entry(entity, 0, 1));
        history.Push(Entry(entity, 1, 2));
        Assert.True(history.TryUndo(out _));

        history.Push(Entry(entity, 1, 3));

        Assert.Equal(2, history.Count);
        Assert.Equal(0, history.RedoCount);
        Assert.False(history.TryRedo(out _));
    }

    static TransformHistoryEntry Entry(EntityId entity, double before, double after) =>
        new(entity, new CommittedTransform(new Vector3d(before, 0, 0)),
            new CommittedTransform(new Vector3d(after, 0, 0)));
}

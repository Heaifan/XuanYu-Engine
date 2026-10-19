using XuanYu.Core.History;
using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;

namespace XuanYu.Core.Tests.History;

public sealed class EditorHistoryAnySnapshotTests
{
    [Fact]
    public void Peek_undo_and_redo_expose_entry_and_revisions_without_moving_history()
    {
        var history = new EditorHistoryOwner();
        var first = Entry(1, 0, 1);
        var second = Entry(2, 0, 2);
        history.PushEntry(first);
        history.PushEntry(second);

        Assert.True(history.TryPeekUndoAny(out var undo));
        Assert.Equal(second, undo.Entry);
        Assert.Equal(1, undo.BeforeRevision);
        Assert.Equal(2, undo.AfterRevision);
        Assert.Equal(2, undo.CurrentRevision);
        Assert.Equal((2, 0, 2L), (history.Count, history.RedoCount, history.CurrentRevision));

        Assert.True(history.TryCompleteUndo(undo));
        Assert.True(history.TryPeekRedoAny(out var redo));
        Assert.Equal(second, redo.Entry);
        Assert.Equal(1, redo.CurrentRevision);
        Assert.Equal((1, 1, 1L), (history.Count, history.RedoCount, history.CurrentRevision));
    }

    [Fact]
    public void Complete_undo_rejects_snapshot_after_history_changes_without_mutation()
    {
        var history = new EditorHistoryOwner();
        history.PushEntry(Entry(1, 0, 1));
        Assert.True(history.TryPeekUndoAny(out var stale));
        history.PushEntry(Entry(2, 0, 2));

        Assert.False(history.TryCompleteUndo(stale));
        Assert.Equal((2, 0, 2L), (history.Count, history.RedoCount, history.CurrentRevision));
        Assert.True(history.TryPeekUndoAny(out var actual));
        Assert.Equal(2, ((TransformHistoryEntry)actual.Entry).EntityKey.Value);
    }

    [Fact]
    public void Complete_redo_rejects_snapshot_from_another_history_owner()
    {
        var history = new EditorHistoryOwner();
        var entry = Entry(1, 0, 1);
        history.PushEntry(entry);
        Assert.True(history.TryPeekUndoAny(out var undo));
        Assert.True(history.TryCompleteUndo(undo));
        Assert.True(history.TryPeekRedoAny(out var redo));
        var other = new EditorHistoryOwner();
        other.PushEntry(entry);

        Assert.False(other.TryCompleteRedo(redo));
        Assert.Equal((1, 0, 1L), (other.Count, other.RedoCount, other.CurrentRevision));
    }

    static TransformHistoryEntry Entry(int id, double before, double after) =>
        new(EntityId.FromInt(id), new CommittedTransform(new Vector3d(before, 0, 0)),
            new CommittedTransform(new Vector3d(after, 0, 0)));
}

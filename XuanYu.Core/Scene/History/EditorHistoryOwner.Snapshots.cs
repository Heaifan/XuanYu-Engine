namespace XuanYu.Core.History;

public sealed partial class EditorHistoryOwner
{
    public bool TryPeekUndoAny(out HistorySnapshot snapshot) =>
        TryPeek(_undo, out snapshot);

    public bool TryPeekRedoAny(out HistorySnapshot snapshot) =>
        TryPeek(_redo, out snapshot);

    public bool TryCompleteUndo(HistorySnapshot expected) =>
        TryComplete(_undo, _redo, expected, false);

    public bool TryCompleteRedo(HistorySnapshot expected) =>
        TryComplete(_redo, _undo, expected, true);

    bool TryPeek(Stack<HistoryItem> source, out HistorySnapshot snapshot)
    {
        if (source.TryPeek(out var item))
        {
            snapshot = new HistorySnapshot(item.Entry, item.BeforeRevision,
                item.AfterRevision, CurrentRevision, item);
            return true;
        }
        snapshot = null!;
        return false;
    }

    bool TryComplete(Stack<HistoryItem> source, Stack<HistoryItem> target,
        HistorySnapshot expected, bool redo)
    {
        ArgumentNullException.ThrowIfNull(expected);
        if (!source.TryPeek(out var item) ||
            !ReferenceEquals(item, expected.HistoryItemIdentity) ||
            item.BeforeRevision != expected.BeforeRevision ||
            item.AfterRevision != expected.AfterRevision ||
            CurrentRevision != expected.CurrentRevision ||
            CurrentRevision != (redo ? item.BeforeRevision : item.AfterRevision))
            return false;

        source.Pop();
        target.Push(item);
        CurrentRevision = redo ? item.AfterRevision : item.BeforeRevision;
        return true;
    }
}

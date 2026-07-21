namespace XuanYu.Core.History;

public sealed class EditorHistoryOwner
{
    readonly Stack<TransformHistoryEntry> _undo = new();
    readonly Stack<TransformHistoryEntry> _redo = new();

    public int Count => _undo.Count;
    public int RedoCount => _redo.Count;

    public void Push(TransformHistoryEntry entry)
    {
        if (!entry.EntityKey.IsValid) throw new ArgumentOutOfRangeException(nameof(entry));
        if (entry.Before == entry.After) return;
        _undo.Push(entry);
        _redo.Clear();
    }

    public bool TryPeekUndo(out TransformHistoryEntry entry) => _undo.TryPeek(out entry);

    public bool TryUndo(out TransformHistoryEntry entry)
    {
        if (!_undo.TryPop(out entry)) return false;
        _redo.Push(entry);
        return true;
    }

    public bool TryRedo(out TransformHistoryEntry entry)
    {
        if (!_redo.TryPop(out entry)) return false;
        _undo.Push(entry);
        return true;
    }

    public void Clear()
    {
        _undo.Clear();
        _redo.Clear();
    }
}

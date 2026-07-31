namespace XuanYu.Core.History;

public sealed class EditorHistoryOwner
{
    readonly Stack<HistoryItem> _undo = new();
    readonly Stack<HistoryItem> _redo = new();
    long _nextRevision = 1;

    public int Count => _undo.Count;
    public int RedoCount => _redo.Count;
    public long CurrentRevision { get; private set; }

    public void Push(TransformHistoryEntry entry)
    {
        if (!entry.EntityKey.IsValid) throw new ArgumentOutOfRangeException(nameof(entry));
        if (entry.Before == entry.After) return;
        PushEntry(entry);
    }

    public void PushEntry(object entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        var item = new HistoryItem(entry, CurrentRevision, _nextRevision++);
        _undo.Push(item);
        _redo.Clear();
        CurrentRevision = item.AfterRevision;
    }

    public bool TryPeekUndo(out TransformHistoryEntry entry)
    {
        if (_undo.TryPeek(out var item) && item.Entry is TransformHistoryEntry transform)
        {
            entry = transform;
            return true;
        }
        entry = default;
        return false;
    }

    public bool TryUndo(out TransformHistoryEntry entry)
    {
        if (!_undo.TryPeek(out var item) || item.Entry is not TransformHistoryEntry transform)
        {
            entry = default;
            return false;
        }
        _undo.Pop();
        _redo.Push(item);
        CurrentRevision = item.BeforeRevision;
        entry = transform;
        return true;
    }

    public bool TryRedo(out TransformHistoryEntry entry)
    {
        if (!_redo.TryPeek(out var item) || item.Entry is not TransformHistoryEntry transform)
        {
            entry = default;
            return false;
        }
        _redo.Pop();
        _undo.Push(item);
        CurrentRevision = item.AfterRevision;
        entry = transform;
        return true;
    }

    public bool TryUndoAny(out object entry) => TryMove(_undo, _redo, false, out entry);

    public bool TryRedoAny(out object entry) => TryMove(_redo, _undo, true, out entry);

    public void Clear()
    {
        _undo.Clear();
        _redo.Clear();
        CurrentRevision = 0;
        _nextRevision = 1;
    }

    bool TryMove(Stack<HistoryItem> source, Stack<HistoryItem> target, bool redo, out object entry)
    {
        if (!source.TryPop(out var item))
        {
            entry = null!;
            return false;
        }
        target.Push(item);
        CurrentRevision = redo ? item.AfterRevision : item.BeforeRevision;
        entry = item.Entry;
        return true;
    }

    readonly record struct HistoryItem(object Entry, long BeforeRevision, long AfterRevision);
}

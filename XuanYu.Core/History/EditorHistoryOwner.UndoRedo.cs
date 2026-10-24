namespace XuanYu.Core.History;

// 只移动历史游标；领域状态的撤销、重做由调用方执行。
public sealed partial class EditorHistoryOwner
{
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

    public bool TryUndoAny(out object entry) => Move(_undo, _redo, false, out entry);

    public bool TryRedoAny(out object entry) => Move(_redo, _undo, true, out entry);

    bool Move(Stack<HistoryItem> source, Stack<HistoryItem> target, bool redo, out object entry)
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
}

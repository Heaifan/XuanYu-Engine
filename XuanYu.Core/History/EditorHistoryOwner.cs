namespace XuanYu.Core.History;

public sealed class EditorHistoryOwner
{
    readonly Stack<TransformHistoryEntry> _undo = new();

    public int Count => _undo.Count;

    public void Push(TransformHistoryEntry entry)
    {
        if (!entry.EntityKey.IsValid) throw new ArgumentOutOfRangeException(nameof(entry));
        if (entry.Before == entry.After) return;
        _undo.Push(entry);
    }

    public bool TryPeekUndo(out TransformHistoryEntry entry) => _undo.TryPeek(out entry);

    public bool TryUndo(out TransformHistoryEntry entry) => _undo.TryPop(out entry);

    public void Clear() => _undo.Clear();
}

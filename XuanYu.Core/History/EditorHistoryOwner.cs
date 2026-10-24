namespace XuanYu.Core.History;

// 历史记录唯一所有者：保管栈、修订号和写入入口。
public sealed partial class EditorHistoryOwner
{
    readonly Stack<HistoryItem> _undo = new();
    readonly Stack<HistoryItem> _redo = new();
    long _nextRevision = 1;

    public int Count => _undo.Count;
    public int RedoCount => _redo.Count;
    public long CurrentRevision { get; private set; }

    public void Push(TransformHistoryEntry entry)
    {
        if (!entry.EntityKey.IsValid)
        {
            throw new ArgumentOutOfRangeException(nameof(entry));
        }

        if (entry.Before == entry.After)
        {
            return;
        }

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

    public void Clear()
    {
        _undo.Clear();
        _redo.Clear();
        CurrentRevision = 0;
        _nextRevision = 1;
    }

    sealed record HistoryItem(object Entry, long BeforeRevision, long AfterRevision);
}

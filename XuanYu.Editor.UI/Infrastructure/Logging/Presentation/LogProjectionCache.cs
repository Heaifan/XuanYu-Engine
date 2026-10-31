using System.Collections.ObjectModel;

namespace XuanYu.Editor.UI;

public sealed class LogProjectionCache
{
    readonly ObservableCollection<LogEntry> _items = [];

    public IReadOnlyList<LogEntry> Items => _items;
    public int RefreshCount { get; private set; }
    public int RebuildCount { get; private set; }

    public bool Sync(IReadOnlyList<LogEntry> desired)
    {
        RefreshCount++;
        if (SameItems(desired)) return false;
        if (CanAppend(desired))
        {
            _items.Add(desired[^1]);
            return true;
        }
        _items.Clear();
        foreach (var item in desired) _items.Add(item);
        RebuildCount++;
        return true;
    }

    bool SameItems(IReadOnlyList<LogEntry> desired)
    {
        if (desired.Count != _items.Count) return false;
        for (var i = 0; i < desired.Count; i++)
            if (!Equals(desired[i], _items[i])) return false;
        return true;
    }

    bool CanAppend(IReadOnlyList<LogEntry> desired)
    {
        if (desired.Count != _items.Count + 1) return false;
        for (var i = 0; i < _items.Count; i++)
            if (!Equals(desired[i], _items[i])) return false;
        return true;
    }
}

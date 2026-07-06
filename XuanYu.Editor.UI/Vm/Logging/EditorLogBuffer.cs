namespace XuanYu.Editor.UI;

public sealed class EditorLogBuffer
{
    const int MaxEntries = 500;
    readonly List<LogEntry> _entries = [];

    public IReadOnlyList<LogEntry> All => _entries;

    public void Seed(IEnumerable<LogEntry> entries)
    {
        foreach (var entry in entries) Add(entry);
    }

    public void Add(LogEntry entry)
    {
        if (_entries.Count > 0 && EditorLogRepeatKey.From(_entries[^1]) == EditorLogRepeatKey.From(entry))
        {
            _entries[^1] = _entries[^1] with { RepeatCount = _entries[^1].RepeatCount + entry.RepeatCount };
            return;
        }

        _entries.Add(entry);
        if (_entries.Count > MaxEntries) _entries.RemoveRange(0, _entries.Count - MaxEntries);
    }

    public IReadOnlyList<LogEntry> Filter(EditorLogFilter filter) =>
        _entries.Where(entry => filter.Allows(entry)).ToArray();
}

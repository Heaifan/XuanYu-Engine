namespace XuanYu.Editor.UI;

public sealed class InspectorRecentStore
{
    public const int MaxRecentProperties = 6;
    readonly Dictionary<InspectorObjectKind, List<string>> _keys = new();

    public IReadOnlyList<string> KeysFor(InspectorObjectKind kind) =>
        _keys.TryGetValue(kind, out var keys) ? keys : Array.Empty<string>();

    public void Record(InspectorObjectKind kind, string key) => RecordCommit(kind, key, true);

    public void RecordCommit(InspectorObjectKind kind, string key, bool succeeded)
    {
        if (!succeeded || string.IsNullOrWhiteSpace(key)) return;
        if (!_keys.TryGetValue(kind, out var keys)) _keys[kind] = keys = [];
        keys.Remove(key);
        keys.Insert(0, key);
        if (keys.Count > MaxRecentProperties) keys.RemoveRange(MaxRecentProperties, keys.Count - MaxRecentProperties);
    }
}

namespace XuanYu.Editor.UI;

public sealed class InspectorRecentStore
{
    public const int MaxRecentProperties = 6;
    readonly Dictionary<InspectorObjectIdentity, List<string>> _keys = new();

    public IReadOnlyList<string> KeysFor(InspectorObjectKind kind) =>
        KeysFor(new InspectorObjectIdentity(kind, ""));

    public IReadOnlyList<string> KeysFor(InspectorObjectIdentity identity) =>
        _keys.TryGetValue(identity, out var keys) ? keys : Array.Empty<string>();

    public void Record(InspectorObjectKind kind, string key) => RecordCommit(kind, key, true);

    public void RecordCommit(InspectorObjectKind kind, string key, bool succeeded)
        => RecordCommit(new InspectorObjectIdentity(kind, ""), key, succeeded);

    public void RecordCommit(InspectorEditTarget target, bool succeeded) =>
        RecordCommit(target.Identity, target.PropertyKey, succeeded);

    void RecordCommit(InspectorObjectIdentity identity, string key, bool succeeded)
    {
        if (!succeeded || string.IsNullOrWhiteSpace(key)) return;
        if (!_keys.TryGetValue(identity, out var keys)) _keys[identity] = keys = [];
        keys.Remove(key);
        keys.Insert(0, key);
        if (keys.Count > MaxRecentProperties) keys.RemoveRange(MaxRecentProperties, keys.Count - MaxRecentProperties);
    }
}

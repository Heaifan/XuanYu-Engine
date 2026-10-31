namespace XuanYu.Editor.UI;

sealed class TerrainRevisionLedger
{
    readonly Dictionary<string, Entry> _entries = new(StringComparer.Ordinal);

    public int Resolve(string terrainId, ulong fingerprint)
    {
        if (!_entries.TryGetValue(terrainId, out var entry))
        {
            _entries[terrainId] = new(fingerprint, 1);
            return 1;
        }
        if (entry.Fingerprint == fingerprint) return entry.Revision;
        var next = checked(entry.Revision + 1);
        _entries[terrainId] = new(fingerprint, next);
        return next;
    }

    readonly record struct Entry(ulong Fingerprint, int Revision);
}

using XuanYu.Core.Spatial;

namespace XuanYu.Core.Space;

public readonly record struct TerrainLodStateKey(
    string TerrainId, int Revision, int ChunkX, int ChunkY);

public sealed class TerrainLodStateCache
{
    readonly Dictionary<TerrainLodStateKey, int> _previous = [];

    public int Count => _previous.Count;

    public TerrainLodSelection Select(TerrainLodStateKey key,
        ViewProjectionState state, SpatialAabb bounds)
        => Select(key, state, bounds, 240, 240);

    public TerrainLodSelection Select(TerrainLodStateKey key,
        ViewProjectionState state, SpatialAabb bounds, int cellCountX, int cellCountY)
    {
        var previous = _previous.TryGetValue(key, out var value) ? value : -1;
        var selection = TerrainLodSelector.Select(state, bounds, cellCountX, cellCountY, previous);
        _previous[key] = selection.Lod;
        return selection;
    }

    public void RetainOnly(IEnumerable<(string TerrainId, int Revision)> resources)
    {
        var retained = resources.ToHashSet();
        foreach (var key in _previous.Keys.Where(x =>
            !retained.Contains((x.TerrainId, x.Revision))).ToArray())
            _previous.Remove(key);
    }

    public void Clear() => _previous.Clear();
}

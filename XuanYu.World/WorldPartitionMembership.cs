using XuanYu.Core.Identity;
namespace XuanYu.World;

public sealed class WorldPartitionMembership
{
    readonly Dictionary<EntityId, RegionKey> _regions = new();
    readonly Dictionary<EntityId, WorldEntityActivity> _activities = new();

    public void Add(EntityId entityKey, RegionKey region)
    {
        if (!entityKey.IsValid) throw new ArgumentOutOfRangeException(nameof(entityKey));
        _regions.Add(entityKey, region);
        _activities.Add(entityKey, WorldEntityActivity.Active);
    }

    public bool Remove(EntityId entityKey)
    {
        var removed = _regions.Remove(entityKey);
        _activities.Remove(entityKey);
        return removed;
    }

    public bool MoveToRegion(EntityId entityKey, RegionKey region)
    {
        if (!_regions.ContainsKey(entityKey)) return false;
        _regions[entityKey] = region;
        return true;
    }

    public bool SetActivity(EntityId entityKey, WorldEntityActivity activity)
    {
        if (!_activities.ContainsKey(entityKey)) return false;
        _activities[entityKey] = activity;
        return true;
    }

    public RegionKey GetRegion(EntityId entityKey) => _regions[entityKey];

    public WorldEntityActivity GetActivity(EntityId entityKey) => _activities[entityKey];

    public IReadOnlyList<WorldPartitionEntry> Snapshot =>
        _regions.OrderBy(item => item.Key.Value)
            .Select(item => new WorldPartitionEntry(
                item.Key,
                item.Value,
                _activities[item.Key]))
            .ToArray();

    public IReadOnlyList<EntityId> EntitiesIn(RegionKey region) =>
        _regions.Where(item => item.Value == region)
            .Select(item => item.Key)
            .OrderBy(item => item.Value)
            .ToArray();

    public void Clear()
    {
        _regions.Clear();
        _activities.Clear();
    }
}

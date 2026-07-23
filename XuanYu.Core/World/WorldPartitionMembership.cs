using XuanYu.Core.Identity;
using XuanYu.Core.Math;

namespace XuanYu.Core.World;

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

    public IReadOnlyList<EntityId> EntitiesIn(RegionKey region) =>
        _regions.Where(item => item.Value == region)
            .Select(item => item.Key)
            .OrderBy(item => item.Value)
            .ToArray();

    public static RegionKey RegionFor(Vector3d globalPosition, double regionSize = 1000)
    {
        if (regionSize <= 0) throw new ArgumentOutOfRangeException(nameof(regionSize));
        return RegionKey.FromGrid(
            (int)global::System.Math.Floor(globalPosition.X / regionSize),
            (int)global::System.Math.Floor(globalPosition.Y / regionSize),
            (int)global::System.Math.Floor(globalPosition.Z / regionSize));
    }
}

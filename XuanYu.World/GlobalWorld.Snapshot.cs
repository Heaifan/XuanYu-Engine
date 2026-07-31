using XuanYu.Core.Identity;
using XuanYu.Core.Spatial;

namespace XuanYu.World;

public sealed partial class GlobalWorld
{
    public int EntityCount => _registry.Count;

    public IReadOnlyList<WorldEntitySnapshot> Entities => _registry.Snapshot;

    public IReadOnlyList<EntityId> EntitiesIn(RegionKey region) =>
        _partition.EntitiesIn(region);

    public IReadOnlyList<WorldPartitionEntry> PartitionSnapshot =>
        _partition.Snapshot;

    public int SpatialEntityCount => _query.Count;

    public SpatialQueryStats LastSpatialQueryStats => _query.LastStats;
}

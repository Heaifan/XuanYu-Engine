using XuanYu.Core.Identity;

namespace XuanYu.World;

public readonly record struct WorldPartitionEntry(
    EntityId EntityKey,
    RegionKey RegionKey,
    WorldEntityActivity Activity);

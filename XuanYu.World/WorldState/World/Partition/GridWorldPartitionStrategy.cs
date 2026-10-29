using XuanYu.Core.Math;

namespace XuanYu.World;

public sealed class GridWorldPartitionStrategy : IWorldPartitionStrategy
{
    readonly double _regionSize;

    public GridWorldPartitionStrategy(double regionSize = 1000)
    {
        if (regionSize <= 0) throw new ArgumentOutOfRangeException(nameof(regionSize));
        _regionSize = regionSize;
    }

    public RegionKey RegionFor(Vector3d globalPosition) =>
        RegionKey.FromGrid(
            (int)global::System.Math.Floor(globalPosition.X / _regionSize),
            (int)global::System.Math.Floor(globalPosition.Y / _regionSize),
            (int)global::System.Math.Floor(globalPosition.Z / _regionSize));
}

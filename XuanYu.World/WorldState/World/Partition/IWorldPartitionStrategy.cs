using XuanYu.Core.Math;

namespace XuanYu.World;

public interface IWorldPartitionStrategy
{
    RegionKey RegionFor(Vector3d globalPosition);
}

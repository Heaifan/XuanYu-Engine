using XuanYu.Core.Math;

namespace XuanYu.Core.World;

public interface IWorldPartitionStrategy
{
    RegionKey RegionFor(Vector3d globalPosition);
}

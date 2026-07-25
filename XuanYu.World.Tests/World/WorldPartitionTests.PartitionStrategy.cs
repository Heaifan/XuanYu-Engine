using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.World;

namespace XuanYu.World.Tests.World;

public sealed partial class WorldPartitionTests
{
    [Fact]
    public void Partition_strategy_can_be_replaced_without_changing_entity_owner()
    {
        var world = new GlobalWorld(new GridWorldPartitionStrategy(regionSize: 10));
        var entity = world.Create("Scout");

        Assert.True(world.UpdateTransform(entity.EntityKey, new CommittedTransform(new Vector3d(11, 0, 0))));

        Assert.Equal(entity.EntityKey, world.Get(entity.EntityKey).EntityKey);
        Assert.Equal(RegionKey.FromGrid(1, 0), world.GetRegion(entity.EntityKey));
    }
}

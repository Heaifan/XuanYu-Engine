using XuanYu.World;
using XuanYu.World.Scene;

namespace XuanYu.World.Tests.World;

public sealed partial class WorldPartitionMigrationTests
{
    [Fact]
    public void Active_dormant_active_keeps_identity_region_and_position()
    {
        var scene = new SceneStateOwner();
        var before = scene.RenderSnapshot.Entity;

        Assert.True(scene.SetEntityActivity(before.EntityKey, WorldEntityActivity.Dormant));
        Assert.True(scene.SetEntityActivity(before.EntityKey, WorldEntityActivity.Active));

        var after = scene.RenderSnapshot.Entity;
        Assert.Equal(before.EntityKey, after.EntityKey);
        Assert.Equal(before.Transform.Position, after.Transform.Position);
        Assert.Equal(RegionKey.Origin, scene.GetRegion(after.EntityKey));
    }
}

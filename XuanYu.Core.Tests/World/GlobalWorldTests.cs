using System.Diagnostics;
using XuanYu.Core.Identity;
using XuanYu.Core.World;

namespace XuanYu.Core.Tests.World;

public sealed class GlobalWorldTests
{
    [Fact]
    public void Global_world_owns_registry_lifecycle()
    {
        var world = new GlobalWorld();

        var entity = world.Create("World Entity");

        Assert.True(world.Exists(entity.EntityKey));
        Assert.Equal(1, world.EntityCount);
        Assert.True(world.Destroy(entity.EntityKey));
        Assert.Equal(0, world.EntityCount);
    }

    [Fact]
    public void Thousand_entity_smoke_keeps_stable_keys()
    {
        var world = new GlobalWorld();
        var beforeMemory = GC.GetTotalMemory(true);
        var createWatch = Stopwatch.StartNew();
        var entities = Enumerable.Range(0, 1000).Select(i => world.Create($"Entity {i}")).ToArray();
        createWatch.Stop();

        var queryWatch = Stopwatch.StartNew();
        foreach (var entity in entities)
        {
            Assert.True(world.Exists(entity.EntityKey));
            Assert.Equal(entity.EntityKey, world.Get(entity.EntityKey).EntityKey);
        }
        queryWatch.Stop();

        var memoryDelta = GC.GetTotalMemory(true) - beforeMemory;
        Debug.WriteLine(
            $"WORLD-A-R1 1000实体冒烟：创建Ticks={createWatch.ElapsedTicks}; " +
            $"查询Ticks={queryWatch.ElapsedTicks}; 内存变化Bytes={memoryDelta}");
        Assert.Equal(1000, world.EntityCount);
        Assert.Equal(1000, entities.Select(item => item.EntityKey).Distinct().Count());
        Assert.True(createWatch.ElapsedTicks >= 0);
        Assert.True(queryWatch.ElapsedTicks >= 0);
        Assert.NotEqual(long.MinValue, memoryDelta);
    }

    [Fact]
    public void Destroyed_entity_key_is_not_reused_by_next_create()
    {
        var world = new GlobalWorld();
        var first = world.Create("First");

        Assert.True(world.Destroy(first.EntityKey));
        var second = world.Create("Second");

        Assert.NotEqual(first.EntityKey, second.EntityKey);
        Assert.False(world.TryGet(first.EntityKey, out _));
    }
}

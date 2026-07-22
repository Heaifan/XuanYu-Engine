using XuanYu.Core.Identity;
using XuanYu.Core.Scene;
using XuanYu.Core.World;

namespace XuanYu.Core.Tests.World;

public sealed class EntityRegistryTests
{
    [Fact]
    public void Create_get_exists_and_destroy_single_entity()
    {
        var registry = new EntityRegistry();

        var entity = registry.Create("R1 Entity");

        Assert.True(registry.Exists(entity.EntityKey));
        Assert.Equal(entity, registry.Get(entity.EntityKey));
        Assert.True(registry.Destroy(entity.EntityKey));
        Assert.False(registry.Exists(entity.EntityKey));
        Assert.False(registry.Destroy(entity.EntityKey));
    }

    [Fact]
    public void Invalid_and_missing_keys_are_not_found()
    {
        var registry = new EntityRegistry();

        Assert.False(registry.Exists(EntityId.None));
        Assert.False(registry.TryGet(EntityId.FromInt(99), out _));
        Assert.Throws<KeyNotFoundException>(() => registry.Get(EntityId.FromInt(99)));
    }

    [Fact]
    public void Ten_entities_have_stable_independent_identity()
    {
        var registry = new EntityRegistry();
        var entities = Enumerable.Range(0, 10).Select(i => registry.Create($"Entity {i}")).ToArray();

        Assert.Equal(10, registry.Count);
        Assert.Equal(10, entities.Select(item => item.EntityKey).Distinct().Count());
        foreach (var entity in entities)
        {
            Assert.Equal(entity.EntityKey, registry.Get(entity.EntityKey).EntityKey);
        }
    }

    [Fact]
    public void Entity_identity_survives_repeated_lookup()
    {
        var registry = new EntityRegistry();
        var transform = new CommittedTransform(new(1, 2, 3));
        var entity = registry.Create("Stable", transform: transform);

        var first = registry.Get(entity.EntityKey);
        var second = registry.Get(entity.EntityKey);

        Assert.Equal(entity.EntityKey, first.EntityKey);
        Assert.Equal(first.EntityKey, second.EntityKey);
        Assert.Equal(transform, second.Transform);
    }
}

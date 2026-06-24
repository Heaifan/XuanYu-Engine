using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.Core.Math;
using XuanYu.Engine.World;

namespace FluidWarfare.Tests.Engine.World;

public sealed class WorldStateTests
{
    [Fact]
    public void CreateEntity_WithValidData_ShouldCreateEntity()
    {
        var world = new WorldState();

        var entityId = world.CreateEntity("测试单位", Vector3d.Zero);

        Assert.True(entityId.IsValid);
        Assert.True(world.ContainsEntity(entityId));
    }

    [Fact]
    public void CreateEntity_WithEmptyDisplayName_ShouldThrow()
    {
        var world = new WorldState();

        var ex = Assert.Throws<ArgumentException>(
            () => world.CreateEntity("", Vector3d.Zero));

        Assert.Contains("显示名称", ex.Message);
    }

    [Fact]
    public void CreateEntity_WithWhiteSpaceDisplayName_ShouldThrow()
    {
        var world = new WorldState();

        Assert.Throws<ArgumentException>(
            () => world.CreateEntity("   ", Vector3d.Zero));
    }

    [Fact]
    public void ContainsEntity_WithCreatedEntity_ShouldReturnTrue()
    {
        var world = new WorldState();
        var entityId = world.CreateEntity("测试单位", Vector3d.Zero);

        Assert.True(world.ContainsEntity(entityId));
    }

    [Fact]
    public void ContainsEntity_WithUnknownEntity_ShouldReturnFalse()
    {
        var world = new WorldState();
        var unknownId = EntityId.FromInt(999);

        Assert.False(world.ContainsEntity(unknownId));
    }

    [Fact]
    public void FindEntity_WithCreatedEntity_ShouldReturnEntityInfo()
    {
        var world = new WorldState();
        var entityId = world.CreateEntity("测试单位", Vector3d.Zero);

        var info = world.FindEntity(entityId);

        Assert.NotNull(info);
        Assert.Equal(entityId, info.EntityId);
        Assert.Equal("测试单位", info.DisplayName);
    }

    [Fact]
    public void FindEntity_WithUnknownEntity_ShouldReturnNull()
    {
        var world = new WorldState();
        var unknownId = EntityId.FromInt(999);

        Assert.Null(world.FindEntity(unknownId));
    }

    [Fact]
    public void FindPosition_WithCreatedEntity_ShouldReturnPosition()
    {
        var expectedPosition = new Vector3d(10.0, 20.0, 30.0);
        var world = new WorldState();
        var entityId = world.CreateEntity("测试单位", expectedPosition);

        var position = world.FindPosition(entityId);

        Assert.NotNull(position);
        Assert.Equal(expectedPosition, position.Value.Value);
    }

    [Fact]
    public void FindPosition_WithUnknownEntity_ShouldReturnNull()
    {
        var world = new WorldState();
        var unknownId = EntityId.FromInt(999);

        Assert.Null(world.FindPosition(unknownId));
    }

    [Fact]
    public void CreateEntity_WithSource_ShouldStoreSource()
    {
        var world = new WorldState();
        var source = new ProjectContentEntitySource("units/test.json", "unitTemplate");

        var entityId = world.CreateEntity("测试单位", Vector3d.Zero, source);

        var info = world.FindEntity(entityId);
        Assert.NotNull(info);
        Assert.NotNull(info.Source);
        Assert.Equal("units/test.json", info.Source.RelativePath);
        Assert.Equal("unitTemplate", info.Source.ContentKind);
    }

    [Fact]
    public void CreateEntity_WithoutSource_ShouldReturnNullSource()
    {
        var world = new WorldState();

        var entityId = world.CreateEntity("测试单位", Vector3d.Zero);

        var info = world.FindEntity(entityId);
        Assert.NotNull(info);
        Assert.Null(info.Source);
    }

    [Fact]
    public void ListEntities_AfterCreatingEntities_ShouldReturnAllEntities()
    {
        var world = new WorldState();
        var id1 = world.CreateEntity("实体甲", Vector3d.Zero);
        var id2 = world.CreateEntity("实体乙", new Vector3d(1.0, 2.0, 3.0));

        var entities = world.ListEntities();

        Assert.Equal(2, entities.Count);
        Assert.Contains(entities, e => e.EntityId == id1 && e.DisplayName == "实体甲");
        Assert.Contains(entities, e => e.EntityId == id2 && e.DisplayName == "实体乙");
    }

    [Fact]
    public void SetPosition_ExistingEntity_UpdatesPosition()
    {
        var world = new WorldState();
        var id = world.CreateEntity("测试单位", new Vector3d(0, 0, 0));

        var changed = world.SetPosition(id, new Vector3d(10, 20, 30));
        Assert.True(changed);

        var pos = world.FindPosition(id);
        Assert.NotNull(pos);
        Assert.Equal(new Vector3d(10, 20, 30), pos.Value.Value);
    }

    [Fact]
    public void SetPosition_UnknownEntity_ReturnsFalse()
    {
        var world = new WorldState();
        var unknownId = EntityId.FromInt(999);

        Assert.False(world.SetPosition(unknownId, Vector3d.Zero));
    }

    [Fact]
    public void SetPosition_SamePosition_DoesNotChange()
    {
        var world = new WorldState();
        var id = world.CreateEntity("测试单位", new Vector3d(5, 0, -3));

        var changed = world.SetPosition(id, new Vector3d(5, 0, -3));
        Assert.False(changed);

        var pos = world.FindPosition(id);
        Assert.NotNull(pos);
        Assert.Equal(new Vector3d(5, 0, -3), pos.Value.Value);
    }

    [Fact]
    public void SetPosition_DifferentPosition_IncreasesRevision()
    {
        var world = new WorldState();
        var id = world.CreateEntity("测试单位", new Vector3d(0, 0, 0));

        world.SetPosition(id, new Vector3d(1, 0, 0));
        world.SetPosition(id, new Vector3d(2, 0, 0));
        var pos = world.FindPosition(id);
        Assert.NotNull(pos);
        Assert.Equal(new Vector3d(2, 0, 0), pos.Value.Value);
    }

    [Fact]
    public void SetPosition_OtherEntities_RemainUnchanged()
    {
        var world = new WorldState();
        var id1 = world.CreateEntity("实体1", new Vector3d(0, 0, 0));
        var id2 = world.CreateEntity("实体2", new Vector3d(100, 100, 100));

        world.SetPosition(id1, new Vector3d(10, 10, 10));

        var pos2 = world.FindPosition(id2);
        Assert.NotNull(pos2);
        Assert.Equal(new Vector3d(100, 100, 100), pos2.Value.Value);
    }
}

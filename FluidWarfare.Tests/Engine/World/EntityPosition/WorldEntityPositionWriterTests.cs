using FluidWarfare.Core.Identity;
using FluidWarfare.Core.Math;
using FluidWarfare.Engine.World;
using FluidWarfare.Engine.World.EntityPosition;

namespace FluidWarfare.Tests.Engine.World.EntityPosition;

public sealed class WorldEntityPositionWriterTests
{
    [Fact]
    public void ExistingEntity_UpdatesPosition()
    {
        var world = new WorldState();
        var id = world.CreateEntity("测试单位", new Vector3d(0, 0, 0));

        var result = WorldEntityPositionWriter.Write(world, id, new Vector3d(10, 20, 30));

        Assert.True(result.IsSuccess);
        Assert.True(result.IsChanged);
        Assert.NotNull(result.Change);
        Assert.Equal(id, result.Change.EntityId);
        Assert.Equal(new Vector3d(10, 20, 30), result.Change.NewPosition);
    }

    [Fact]
    public void UnknownEntity_ReturnsFailure()
    {
        var world = new WorldState();
        var unknownId = EntityId.FromInt(999);

        var result = WorldEntityPositionWriter.Write(world, unknownId, Vector3d.Zero);

        Assert.False(result.IsSuccess);
        Assert.False(result.IsChanged);
        Assert.Contains("不存在", result.Message);
    }

    [Fact]
    public void SamePosition_DoesNotIncreaseRevision()
    {
        var world = new WorldState();
        var id = world.CreateEntity("测试单位", new Vector3d(5, 0, -3));

        var result = WorldEntityPositionWriter.Write(world, id, new Vector3d(5, 0, -3));

        Assert.True(result.IsSuccess);
        Assert.False(result.IsChanged);
        Assert.NotNull(result.Change);
        Assert.Equal("位置未变化。", result.Message);
    }

    [Fact]
    public void DifferentPosition_IncreasesRevisionOnce()
    {
        var world = new WorldState();
        var id = world.CreateEntity("测试单位", new Vector3d(0, 0, 0));

        WorldEntityPositionWriter.Write(world, id, new Vector3d(10, 0, 0));
        var pos = world.FindPosition(id);
        Assert.NotNull(pos);
        Assert.Equal(new Vector3d(10, 0, 0), pos.Value.Value);
    }

    [Fact]
    public void OtherEntities_RemainUnchanged()
    {
        var world = new WorldState();
        var id1 = world.CreateEntity("实体1", Vector3d.Zero);
        var id2 = world.CreateEntity("实体2", new Vector3d(99, 99, 99));

        WorldEntityPositionWriter.Write(world, id1, new Vector3d(10, 10, 10));

        var pos2 = world.FindPosition(id2);
        Assert.NotNull(pos2);
        Assert.Equal(new Vector3d(99, 99, 99), pos2.Value.Value);
    }
}

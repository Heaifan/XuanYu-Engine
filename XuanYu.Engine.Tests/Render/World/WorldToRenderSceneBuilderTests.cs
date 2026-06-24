using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.Core.Math;
using XuanYu.Engine.World;
using XuanYu.Engine.Render.Scene;
using XuanYu.Engine.Render.World;

namespace FluidWarfare.Tests.Render.World;

public sealed class WorldToRenderSceneBuilderTests
{
    [Fact]
    public void Build_WithEmptyWorld_ShouldReturnEmptyScene()
    {
        var world = new WorldState();

        var scene = WorldToRenderSceneBuilder.Build(world);

        Assert.Empty(scene.Objects);
    }

    [Fact]
    public void Build_WithOneEntity_ShouldCreateOneRenderObject()
    {
        var world = new WorldState();
        world.CreateEntity("测试单位", Vector3d.Zero);

        var scene = WorldToRenderSceneBuilder.Build(world);

        Assert.Single(scene.Objects);
    }

    [Fact]
    public void Build_ShouldCopyEntityId()
    {
        var world = new WorldState();
        var entityId = world.CreateEntity("测试单位", Vector3d.Zero);

        var scene = WorldToRenderSceneBuilder.Build(world);

        Assert.Equal(entityId, scene.Objects[0].EntityId);
    }

    [Fact]
    public void Build_ShouldCopyDisplayName()
    {
        var world = new WorldState();
        world.CreateEntity("测试单位", Vector3d.Zero);

        var scene = WorldToRenderSceneBuilder.Build(world);

        Assert.Equal("测试单位", scene.Objects[0].DisplayName);
    }

    [Fact]
    public void Build_ShouldCopyPosition()
    {
        var expectedPosition = new Vector3d(10.0, 20.0, 30.0);
        var world = new WorldState();
        world.CreateEntity("测试单位", expectedPosition);

        var scene = WorldToRenderSceneBuilder.Build(world);

        Assert.Equal(expectedPosition, scene.Objects[0].Position);
    }

    [Fact]
    public void Build_ShouldUseUnitMarkerVisualKind()
    {
        var world = new WorldState();
        world.CreateEntity("测试单位", Vector3d.Zero);

        var scene = WorldToRenderSceneBuilder.Build(world);

        Assert.Equal(RenderObjectVisualKind.UnitMarker, scene.Objects[0].VisualKind);
    }

    [Fact]
    public void Build_ShouldCopySourcePath()
    {
        var world = new WorldState();
        var source = new ProjectContentEntitySource("units/test.json", "unitTemplate");
        world.CreateEntity("测试单位", Vector3d.Zero, source);

        var scene = WorldToRenderSceneBuilder.Build(world);

        Assert.Equal("units/test.json", scene.Objects[0].SourcePath);
    }

    [Fact]
    public void Build_WithoutSource_ShouldHaveNullSourcePath()
    {
        var world = new WorldState();
        world.CreateEntity("测试单位", Vector3d.Zero);

        var scene = WorldToRenderSceneBuilder.Build(world);

        Assert.Null(scene.Objects[0].SourcePath);
    }

    [Fact]
    public void Build_WithNullWorld_ShouldThrow()
    {
        var ex = Assert.Throws<ArgumentNullException>(
            () => WorldToRenderSceneBuilder.Build(null!));

        Assert.Contains("worldState", ex.Message);
    }
}

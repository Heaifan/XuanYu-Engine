using FluidWarfare.Core.Identity;
using FluidWarfare.Core.Math;
using FluidWarfare.Render.Scene;
using FluidWarfare.Render.Scene.Position;
using FluidWarfare.Render.Selection;

namespace FluidWarfare.Tests.Render.Scene.Position;

public sealed class RenderSceneObjectPositionWriterTests
{
    private static RenderScene MakeSceneWithEntity(EntityId id, Vector3d position)
    {
        var bounds = new SceneAxisAlignedBounds(position, new Vector3d(0.625, 0.625, 0.625));
        var obj = new RenderObjectInfo(id, "test", position,
            RenderObjectVisualKind.UnitMarker, "test.json", bounds);
        return new RenderScene([obj]);
    }

    [Fact]
    public void PositionChange_UpdatesRenderObject()
    {
        var id = EntityId.FromInt(1);
        var scene = MakeSceneWithEntity(id, new Vector3d(0, 0, 0));

        var result = RenderSceneObjectPositionWriter.Update(
            scene, id, new Vector3d(10, 20, 30));

        Assert.True(result.IsSuccess);
        Assert.True(result.IsChanged);
        Assert.NotNull(result.NewScene);
        var updated = result.NewScene.Objects[0];
        Assert.Equal(new Vector3d(10, 20, 30), updated.Position);
    }

    [Fact]
    public void PositionChange_PreservesEntityId()
    {
        var id = EntityId.FromInt(1);
        var scene = MakeSceneWithEntity(id, new Vector3d(0, 0, 0));

        var result = RenderSceneObjectPositionWriter.Update(
            scene, id, new Vector3d(5, 5, 5));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.NewScene);
        Assert.Equal(id, result.NewScene.Objects[0].EntityId);
    }

    [Fact]
    public void PositionChange_PreservesVisualKind()
    {
        var id = EntityId.FromInt(1);
        var scene = MakeSceneWithEntity(id, new Vector3d(0, 0, 0));

        var result = RenderSceneObjectPositionWriter.Update(
            scene, id, new Vector3d(5, 5, 5));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.NewScene);
        Assert.Equal(RenderObjectVisualKind.UnitMarker, result.NewScene.Objects[0].VisualKind);
    }

    [Fact]
    public void PositionChange_UpdatesSelectionBounds()
    {
        var id = EntityId.FromInt(1);
        var oldPos = new Vector3d(0, 0, 0);
        var scene = MakeSceneWithEntity(id, oldPos);

        var newPos = new Vector3d(10, 0, -5);
        var result = RenderSceneObjectPositionWriter.Update(scene, id, newPos);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.NewScene);
        var bounds = result.NewScene.Objects[0].SelectionBounds;
        Assert.NotNull(bounds);
        // SelectionBounds center should be at (newPos.X, newPos.Y, newPos.Z + 0.5) [Z-Up]
        Assert.Equal(new Vector3d(10, 0, -4.5), bounds.Center);
    }

    [Fact]
    public void SamePosition_ReturnsNoChange()
    {
        var id = EntityId.FromInt(1);
        var pos = new Vector3d(3, 0, 4);
        var scene = MakeSceneWithEntity(id, pos);

        var result = RenderSceneObjectPositionWriter.Update(scene, id, pos);

        Assert.True(result.IsSuccess);
        Assert.False(result.IsChanged);
        Assert.Same(scene, result.NewScene); // Same instance = no copy
    }

    [Fact]
    public void OtherRenderObjects_RemainUnchanged()
    {
        var id1 = EntityId.FromInt(1);
        var id2 = EntityId.FromInt(2);
        var obj1 = new RenderObjectInfo(id1, "a", new Vector3d(0, 0, 0),
            RenderObjectVisualKind.UnitMarker, null,
            new SceneAxisAlignedBounds(new Vector3d(0, 0, 0.5), new Vector3d(0.625, 0.625, 0.625)));
        var obj2 = new RenderObjectInfo(id2, "b", new Vector3d(100, 0, 0),
            RenderObjectVisualKind.UnitMarker, null,
            new SceneAxisAlignedBounds(new Vector3d(100, 0, 0.5), new Vector3d(0.625, 0.625, 0.625)));
        var scene = new RenderScene([obj1, obj2]);

        var result = RenderSceneObjectPositionWriter.Update(
            scene, id1, new Vector3d(10, 10, 10));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.NewScene);
        Assert.Equal(2, result.NewScene.Objects.Count);
        Assert.Equal(id2, result.NewScene.Objects[1].EntityId);
        Assert.Equal(new Vector3d(100, 0, 0), result.NewScene.Objects[1].Position);
    }

    [Fact]
    public void UnknownEntity_ReturnsFailure()
    {
        var scene = RenderScene.Empty;
        var unknownId = EntityId.FromInt(999);

        var result = RenderSceneObjectPositionWriter.Update(
            scene, unknownId, Vector3d.Zero);

        Assert.False(result.IsSuccess);
        Assert.False(result.IsChanged);
        Assert.Contains("未找到", result.Message);
    }
}

using FluidWarfare.Core.Identity;
using FluidWarfare.Core.Math;
using FluidWarfare.Render.Scene;
using FluidWarfare.Render.Selection;
using FluidWarfare.Render.Selection.Ground;
using FluidWarfare.Render.Selection.Pointer;

namespace FluidWarfare.Tests.Render.Selection.Pointer;

public sealed class ScenePointerPickerTests
{
    private static readonly SceneGroundPlane DefaultGround = SceneGroundPlane.Default;

    private static RenderScene MakeSceneWithEntity(EntityId id, Vector3d center, double halfSize, RenderObjectVisualKind kind)
    {
        var bounds = new SceneAxisAlignedBounds(center, new Vector3d(halfSize, halfSize, halfSize));
        var obj = new RenderObjectInfo(id, "test", center, kind, "test.json", bounds);
        return new RenderScene([obj]);
    }

    [Fact]
    public void RayHitsEntityAndGround_ReturnsEntity()
    {
        // Entity at (0, 0, 0.5) with 1.25 bounds (Z-Up: visual center at Z+0.5)
        var id = EntityId.FromInt(1);
        var scene = MakeSceneWithEntity(id, new Vector3d(0, 0, 0.5), 0.625, RenderObjectVisualKind.UnitMarker);

        var ray = new SceneRay(new Vector3d(0, 0, 10), new Vector3d(0, 0, -1));
        var result = ScenePointerPicker.Pick(ray, scene, DefaultGround);

        Assert.Equal(ScenePointerPickKind.Entity, result.Kind);
        Assert.NotNull(result.EntityId);
        Assert.Equal(id, result.EntityId);
    }

    [Fact]
    public void RayMissesEntityButHitsGround_ReturnsGround()
    {
        // Entity at (100, 0, 0.5) — far away
        var id = EntityId.FromInt(1);
        var scene = MakeSceneWithEntity(id, new Vector3d(100, 0, 0.5), 0.625, RenderObjectVisualKind.UnitMarker);

        // Ray through origin downward along -Z
        var ray = new SceneRay(new Vector3d(0, 0, 10), new Vector3d(0, 0, -1));
        var result = ScenePointerPicker.Pick(ray, scene, DefaultGround);

        Assert.Equal(ScenePointerPickKind.Ground, result.Kind);
        Assert.NotNull(result.GroundPosition);
        Assert.Equal(0, result.GroundPosition!.Value.X, 0.01);
        Assert.Equal(0, result.GroundPosition.Value.Y, 0.01);
        Assert.Equal(0, result.GroundPosition.Value.Z, 0.01);
    }

    [Fact]
    public void RayMissesBoth_ReturnsNone()
    {
        var scene = RenderScene.Empty;

        // Ray parallel to ground (Direction.Z = 0)
        var ray = new SceneRay(new Vector3d(0, 0, 10), new Vector3d(1, 0, 0));
        var result = ScenePointerPicker.Pick(ray, scene, DefaultGround);

        Assert.Equal(ScenePointerPickKind.None, result.Kind);
    }

    [Fact]
    public void NearestEntityStillWinsBeforeGround()
    {
        // Two entities in Z-Up: center Z = 0.5
        var id1 = EntityId.FromInt(1);
        var id2 = EntityId.FromInt(2);
        var bounds1 = new SceneAxisAlignedBounds(new Vector3d(0, 0, 0.5), new Vector3d(0.625, 0.625, 0.625));
        var obj1 = new RenderObjectInfo(id1, "near", new Vector3d(0, 0, 0.5),
            RenderObjectVisualKind.UnitMarker, "a.json", bounds1);
        var bounds2 = new SceneAxisAlignedBounds(new Vector3d(5, 0, 0.5), new Vector3d(0.625, 0.625, 0.625));
        var obj2 = new RenderObjectInfo(id2, "far", new Vector3d(5, 0, 0.5),
            RenderObjectVisualKind.UnitMarker, "b.json", bounds2);
        var scene = new RenderScene([obj1, obj2]);

        // Ray through origin downward
        var ray = new SceneRay(new Vector3d(0, 0, 10), new Vector3d(0, 0, -1));
        var result = ScenePointerPicker.Pick(ray, scene, DefaultGround);

        Assert.Equal(ScenePointerPickKind.Entity, result.Kind);
        Assert.Equal(id1, result.EntityId);
    }

    [Fact]
    public void EmptySceneRayHitsGround_ReturnsGround()
    {
        var ray = new SceneRay(new Vector3d(0, 0, 10), new Vector3d(0, 0, -1));
        var result = ScenePointerPicker.Pick(ray, null, DefaultGround);

        Assert.Equal(ScenePointerPickKind.Ground, result.Kind);
    }

    [Fact]
    public void EmptySceneParallelRay_ReturnsNone()
    {
        var ray = new SceneRay(new Vector3d(0, 0, 10), new Vector3d(1, 0, 0));
        var result = ScenePointerPicker.Pick(ray, null, DefaultGround);

        Assert.Equal(ScenePointerPickKind.None, result.Kind);
    }
}

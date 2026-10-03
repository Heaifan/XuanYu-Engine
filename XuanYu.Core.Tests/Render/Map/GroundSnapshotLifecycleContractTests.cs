using System.Reflection;
using System.Runtime.CompilerServices;
using XuanYu.Core.Map;
using XuanYu.Core.Math;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Render;

namespace XuanYu.Core.Tests.Render.Map;

public sealed class GroundSnapshotLifecycleContractTests
{
    static MapRenderSnapshot Snap(long generation, double width = 100, double depth = 200) =>
        new("ground", width, depth, MapSurfaceKind.Flat, 10, 0, 1, 1, generation);

    [Fact]
    public void Clearing_resources_preserves_authoritative_map_snapshot()
    {
        var owner = (VulkanClearFrameOwner)RuntimeHelpers.GetUninitializedObject(
            typeof(VulkanClearFrameOwner));
        var field = typeof(VulkanClearFrameOwner).GetField("_mapSurfaceMap",
            BindingFlags.Instance | BindingFlags.NonPublic)!;
        field.SetValue(owner, Snap(7));

        owner.ClearMapSurface();

        var snapshot = (MapRenderSnapshot)field.GetValue(owner)!;
        Assert.True(snapshot.HasMap);
        Assert.Equal(100, snapshot.WidthMeters);
        Assert.Equal(200, snapshot.DepthMeters);
    }

    [Fact]
    public void Patch_build_consumes_the_current_snapshot()
    {
        var a = Snap(1, 100, 200);
        var b = Snap(2, 300, 400) with { BaseHeightMeters = 30 };
        var patch = new ReferencePlanePatchPlacement(-10, -20, 10, 20);
        var geometryA = MapSurfaceGeometryBuilder.BuildPatch(a, patch, Vector3d.Zero);
        var geometryB = MapSurfaceGeometryBuilder.BuildPatch(b, patch, Vector3d.Zero);

        Assert.Equal(10, geometryA.Vertices[0].Z);
        Assert.Equal(30, geometryB.Vertices[0].Z);
        Assert.NotEqual(geometryA.Vertices[0].Z, geometryB.Vertices[0].Z);
    }

    [Fact]
    public void New_snapshot_requires_new_resource_before_old_buffer_can_draw()
    {
        var oldSnapshot = Snap(10, 100, 200);
        var newSnapshot = Snap(11, 300, 400);
        var oldKey = MapSurfaceResourceKey.From(oldSnapshot);
        var update = MapSurfaceResourceUpdatePolicy.Decide(newSnapshot, 10, oldKey);

        Assert.Equal(MapSurfaceResourceUpdateKind.Recreate, update.Kind);
        Assert.NotEqual(oldKey, update.Key);
    }

    [Fact]
    public void Drawable_ground_keeps_snapshot_patch_and_buffer_generation_aligned()
    {
        var snapshot = Snap(4);
        var key = MapSurfaceResourceKey.From(snapshot);
        var patch = MapSurfaceGeometryBuilder.BuildPatch(snapshot,
            new ReferencePlanePatchPlacement(-10, -20, 10, 20), Vector3d.Zero);

        Assert.Equal(4, snapshot.SourceChangeSequence);
        Assert.Equal(key, MapSurfaceResourceKey.From(snapshot));
        Assert.Equal(10, patch.Vertices[0].Z);
    }
}

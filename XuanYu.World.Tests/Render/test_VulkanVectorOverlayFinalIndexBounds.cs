using XuanYu.Core.Math;
using XuanYu.Core.Spatial;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Render.VectorOverlay;

namespace XuanYu.World.Tests.Render;

public sealed class VulkanVectorOverlayFinalIndexBoundsTests
{
    [Theory]
    [InlineData(2u, 1)]
    [InlineData(0u, -1)]
    public void Validate_rejects_indices_outside_vertices_after_base_vertex(uint index, int baseVertex)
    {
        var resource = Resource(index, baseVertex);

        Assert.False(VulkanVectorOverlayValidator.Validate(resource, out var error));
        Assert.Equal("final index out of range", error);
    }

    [Theory]
    [InlineData(0u, 1)]
    [InlineData(1u, -1)]
    public void Validate_accepts_indices_inside_vertices_after_base_vertex(uint index, int baseVertex)
    {
        var resource = Resource(index, baseVertex);

        Assert.True(VulkanVectorOverlayValidator.Validate(resource, out var error), error);
    }

    static RenderVectorOverlayResource Resource(uint index, int baseVertex) => new(
        new RenderVectorOverlayKey("overlay"), 1,
        [new(default, default, 0, 0), new(default, default, 0, 0), new(default, default, 0, 0)],
        [index, 0, 0],
        [new(0, 1, baseVertex, RenderVectorOverlayPrimitiveKind.Fill,
            new RenderStaticModelColor(1, 1, 1, 1), 0, 0)],
        new SpatialAabb(new Vector3d(0, 0, 0), new Vector3d(1, 1, 1)));
}

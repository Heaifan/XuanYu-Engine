using XuanYu.Core.Math;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Render.VectorOverlay;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class VectorOverlayRenderOriginContractTests
{
    [Fact]
    public void Upload_vertex_positions_are_world_minus_render_origin()
    {
        var world = new RenderVectorOverlayVertex(
            new Vector3d(100_000, 20_000, 350),
            new Vector3d(100_250, 20_400, 360), 0, 1);
        var origin = new Vector3d(99_900, 19_800, 300);

        var actual = VulkanVectorOverlayVertex.From(world, origin);

        Assert.Equal(100, actual.X);
        Assert.Equal(200, actual.Y);
        Assert.Equal(50, actual.Z);
        Assert.Equal(350, actual.Sx);
        Assert.Equal(600, actual.Sy);
        Assert.Equal(60, actual.Sz);
    }

    [Fact]
    public void Upload_conversion_does_not_mutate_world_vertex()
    {
        var world = new RenderVectorOverlayVertex(
            new Vector3d(100_000, 20_000, 350),
            new Vector3d(100_250, 20_400, 360), 0, 1);
        var origin = new Vector3d(99_900, 19_800, 300);

        _ = VulkanVectorOverlayVertex.From(world, origin);

        Assert.Equal(new Vector3d(100_000, 20_000, 350), world.Position);
        Assert.Equal(new Vector3d(100_250, 20_400, 360), world.Secondary);
    }
}

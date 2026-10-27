using XuanYu.Core.Math;
using XuanYu.Core.Map;
using XuanYu.Core.Space;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Render;

namespace XuanYu.Core.Tests.Render.Vulkan;

public sealed class VulkanRenderStateTests
{
    [Fact]
    public void Frame_state_exposes_one_render_origin_for_projection_consumers()
    {
        var camera = new RenderCameraProjection(
            new Vector3d(20_000, 30_000, 40_000), new(0, 0, -1), new(0, 1, 0),
            60, 0.1, 100_000, 1);
        var state = new VulkanFrameState();
        state.Apply(Projection(MapRenderSnapshot.Empty) with { Camera = camera });

        var view = state.GetViewProjection(new(1920, 1080), 1);

        Assert.Equal(camera.Position, view.RenderOrigin);
        Assert.Equal(view.RenderOrigin, state.RenderOrigin);
    }

    static RenderProjection Projection(MapRenderSnapshot map) => new(
        new RenderCameraProjection(new(0, 0, 10), new(0, 0, -1), new(0, 1, 0),
            60, 0.1, 1000, 1), [], false, default, Map: map);
}

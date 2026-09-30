using XuanYu.Core.Map;
using XuanYu.Core.Math;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Render;

namespace XuanYu.World.Tests.Render;

public sealed class WorldGridIndependenceContractTests
{
    [Fact]
    public void Map_ground_draw_is_restored_while_world_grid_remains_independent()
    {
        var map = new MapRenderSnapshot("map", 100, 100, MapSurfaceKind.Flat, 0, 0, 1, 1, 1);
        var projection = new RenderProjection(default, [], false, Vector3d.Zero,
            Assist: new EditorViewportAssistState(ShowGrid: true), Map: map);
        var kinds = RenderDrawPlan.GetFrameDrawPlan(projection).Select(entry => entry.Kind).ToArray();

        Assert.Contains(RenderDrawKind.MapGround, kinds);
        Assert.Contains(RenderDrawKind.EditorReferenceGrid, kinds);
        Assert.NotEqual(
            VulkanClearFrameOwner.ResolveDrawOwner(RenderDrawKind.MapGround),
            VulkanClearFrameOwner.ResolveDrawOwner(RenderDrawKind.EditorReferenceGrid));
    }
}

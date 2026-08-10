namespace XuanYu.World.Tests.Render;

public sealed class WorldGridIndependenceContractTests
{
    static string RenderVulkanFile(params string[] path)
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        return File.ReadAllText(Path.Combine([root, "XuanYu.Render.Vulkan", .. path]));
    }

    [Fact]
    public void Map_ground_draw_is_restored_while_world_grid_remains_independent()
    {
        var draw = RenderVulkanFile("Render", "Scene", "VulkanClearFrameOwner.Draw.cs");
        var scale = RenderVulkanFile("Render", "Grid", "VulkanClearFrameOwner.GridScale.cs");
        Assert.Contains("draw.Kind == RenderDrawKind.MapGround", draw);
        Assert.Contains("DrawMapSurface(cb, pScene)", draw);
        Assert.DoesNotContain("MapGround) continue", draw);
        Assert.Contains("const double height = 0.0", scale);
        Assert.DoesNotContain("projection.Map.BaseHeightMeters", scale);
    }
}

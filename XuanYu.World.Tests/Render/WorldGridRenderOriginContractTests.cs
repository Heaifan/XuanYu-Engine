namespace XuanYu.World.Tests.Render;

public sealed class WorldGridRenderOriginContractTests
{
    [Fact]
    public void Grid_push_constants_publish_render_origin_for_large_worlds()
    {
        var source = Read("XuanYu.Render.Vulkan", "Render", "Grid",
            "VulkanClearFrameOwner.GridScale.cs");

        Assert.Contains("scene[32] = (float)state.RenderOrigin.X", source);
        Assert.Contains("scene[33] = (float)state.RenderOrigin.Y", source);
        Assert.Contains("scene[34] = (float)state.RenderOrigin.Z", source);
        Assert.DoesNotContain("scene[32] = (float)camera.Position.X", source);
    }

    [Fact]
    public void Grid_shader_reconstructs_absolute_world_xy_from_relative_view()
    {
        var shader = Read("XuanYu.Render.Vulkan", "Shaders",
            "editor_world_reference_grid.frag");

        Assert.Contains("renderOrigin", shader);
        Assert.Contains("-renderOrigin.z", shader);
        Assert.Contains("worldPosition.xy + renderOrigin.xy", shader);
    }

    static string Read(params string[] parts)
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var path = Path.Combine([dir.FullName, .. parts]);
            if (File.Exists(path)) return File.ReadAllText(path);
            dir = dir.Parent;
        }
        throw new FileNotFoundException(string.Join("/", parts));
    }
}

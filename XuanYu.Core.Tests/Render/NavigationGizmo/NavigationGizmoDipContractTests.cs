using System.IO;

namespace XuanYu.Core.Tests.Render.NavigationGizmo;

public sealed class NavigationGizmoDipContractTests
{
    static string Root => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
    static string Read(string path) => File.ReadAllText(Path.Combine(Root, path));

    [Fact]
    public void Cpu_pushes_render_scale_in_gizmo_params()
    {
        var source = Read("XuanYu.Render.Vulkan/Render/Grid/VulkanClearFrameOwner.NavGizmo.cs");
        Assert.Contains("scene[19] = (float)_renderProjection.ViewportDpiScale", source);
        Assert.Contains("scene[16] = 96.0f", source);
        Assert.Contains("scene[17] = 14.0f", source);
    }

    [Fact]
    public void Shader_computes_geometry_in_dip_space()
    {
        var shader = Read("XuanYu.Render.Vulkan/Shaders/editor_nav_gizmo.frag");
        Assert.Contains("w = RenderScaling", shader);
        Assert.Contains("float dpi = max(pc.gizmoParams.w, 0.5)", shader);
        Assert.Contains("AXIS_RADIUS_DIP = 27.0", shader);
        Assert.Contains("HUB_RADIUS_DIP = 9.5", shader);
        Assert.Contains("FRONT_RADIUS_DIP = 7.5", shader);
        Assert.Contains("BACK_RADIUS_DIP = 3.8", shader);
    }
}

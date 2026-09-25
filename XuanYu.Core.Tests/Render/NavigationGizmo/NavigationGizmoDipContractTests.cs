using System.IO;
using XuanYu.Editor.UI;

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
        Assert.Contains("AXIS_RADIUS_DIP = 24.0", shader);
        Assert.Contains("HUB_RADIUS_DIP = 14.0", shader);
        Assert.Contains("ENDPOINT_RADIUS_DIP = 9.0", shader);
    }

    [Fact]
    public void Frozen_a_visual_contract_keeps_visual_and_hit_sizes_separate()
    {
        Assert.Equal(96.0, NavigationGizmoLayout.GizmoSize);
        Assert.Equal(14.0, NavigationGizmoLayout.Margin);
        Assert.Equal(24.0, NavigationGizmoLayout.AxisRadius);
        Assert.Equal(14.0, NavigationGizmoLayout.CenterRadius);
        Assert.Equal(9.0, NavigationGizmoLayout.PositiveEndpointRadius);
        Assert.Equal(3.0, NavigationGizmoLayout.AxisWidth);
        Assert.InRange(NavigationGizmoLayout.HitRadius, 11.0, 12.0);
    }

    [Fact]
    public void Frozen_a_shader_contract_contains_light_panel_and_active_state()
    {
        var shader = Read("XuanYu.Render.Vulkan/Shaders/editor_nav_gizmo.frag");
        Assert.Contains("PANEL_RADIUS_DIP = 16.0", shader);
        Assert.Contains("ACTIVE_INDEX", shader);
        Assert.Contains("#D65252", shader);
        Assert.Contains("#4FAE72", shader);
        Assert.Contains("#6FA4F0", shader);
        Assert.Contains("HOVER_RADIUS_DIP = 10.5", shader);
        Assert.Contains("PRESSED_RADIUS_DIP = 11.0", shader);
    }
}

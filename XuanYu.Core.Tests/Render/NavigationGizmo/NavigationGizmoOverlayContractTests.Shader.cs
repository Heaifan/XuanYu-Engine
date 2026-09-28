namespace XuanYu.Core.Tests.Render;

public sealed partial class NavigationGizmoOverlayContractTests
{
    // 原点：屏幕空间标记，不再贴地求交。
    [Fact]
    public void Origin_shader_is_screen_space_not_ground_projected()
    {
        var frag = ShaderFile("editor_world_origin.frag");
        Assert.Contains("gl_FragCoord", frag);
        Assert.DoesNotContain("rayDirection", frag);
        Assert.DoesNotContain("nearWorld", frag);
        Assert.DoesNotContain("worldPosition", frag);
        Assert.Contains("#718096", frag);
    }

    // Gizmo shader：相机姿态投影、屏幕空间、悬停索引与分层合成。
    [Fact]
    public void Nav_gizmo_shader_uses_camera_basis_screen_space()
    {
        var frag = ShaderFile("editor_nav_gizmo.frag");
        Assert.Contains("cameraRight", frag);
        Assert.Contains("cameraUp", frag);
        Assert.Contains("cameraForward", frag);
        Assert.Contains("gl_FragCoord", frag);
        Assert.Contains("hover=int", frag);
        Assert.Contains("drawAxis", frag);
        Assert.Contains("compositeOver", frag);
    }

    // F3-F3：Blender 结构合同——正对、轴线、新配色与标签规则。
    [Fact]
    public void Nav_gizmo_shader_f3_f3_contract()
    {
        var frag = ShaderFile("editor_nav_gizmo.frag");
        Assert.DoesNotContain("roundedPanel", frag);
        Assert.Contains("TRANSPARENT_BACKGROUND", frag);
        Assert.Contains("ENDPOINT_COUNT = 6", frag);
        Assert.Contains("drawAxis", frag);
        Assert.Contains("AXIS_COLOR", frag);
        Assert.Contains("HUB_RADIUS_DIP=11.", frag);
        Assert.Contains("interactionParams", frag);
    }

    [Fact]
    public void Gizmo_shader_draws_positive_and_negative_labels()
    {
        var frag = ShaderFile("editor_nav_gizmo.frag");
        Assert.Contains("glyphMinus", frag);
        Assert.Contains("!e.positive", frag);
        Assert.Contains("drawLabel(acc", frag);
    }
}

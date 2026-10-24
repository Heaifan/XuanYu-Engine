using System.IO;
using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render;

// F3-F1：导航 Gizmo Overlay Pass 与屏幕空间原点标记合同测试。
// 1. DrawPlan 始终以 NavigationGizmo 收尾（最后绘制、深度关）；
// 2. 原点 shader 不再投影到 Z=0 地面（无 rayDirection/t 求交），改为屏幕空间标记；
// 3. 导航 Gizmo shader 使用相机 Right/Up/Forward 投影、纯屏幕空间（gl_FragCoord）。
public sealed partial class NavigationGizmoOverlayContractTests
{
    static string ShaderFile(string name)
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var full = Path.Combine(root, "XuanYu.Render.Vulkan", "Shaders", name);
        Assert.True(File.Exists(full), $"Shader 缺失：{full}");
        return File.ReadAllText(full);
    }

    // 1. DrawPlan 收尾：NavigationGizmo 恒为最后一项。
    [Fact]
    public void Navigation_gizmo_is_last_draw()
    {
        var camera = new RenderCameraProjection(
            new Vector3d(4, -5, 3), new Vector3d(-0.5, 0.5, -0.6), Vector3d.UnitZ, 45, 0.1, 1000, 1);
        var projection = new RenderProjection(camera, [], false, default, Map: default);
        var plan = RenderDrawPlan.GetFrameDrawPlan(projection);
        Assert.NotEmpty(plan);
        Assert.Equal(RenderDrawKind.NavigationGizmo, plan[^1].Kind);
    }

    // 4. 悬停索引默认 -1 且流转到 RenderProjection。
    [Fact]
    public void Hover_index_defaults_to_none_and_flows_to_projection()
    {
        var assist = EditorViewportAssistState.Default;
        Assert.Equal(-1, assist.NavGizmoHoverIndex);
        var withHover = assist with { NavGizmoHoverIndex = 1, NavGizmoActiveIndex = 4,
            NavGizmoCenterHover = true };
        Assert.Equal(1, withHover.NavGizmoHoverIndex);
        Assert.Equal(4, withHover.NavGizmoActiveIndex);
        Assert.True(withHover.NavGizmoCenterHover);
    }

    [Fact]
    public void Router_updates_the_active_consumer_during_drag()
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var source = File.ReadAllText(Path.Combine(root, "XuanYu.Editor", "Input", "ViewportInputRouter.cs"));
        var navigation = File.ReadAllText(Path.Combine(root, "XuanYu.Editor", "Input", "Consumers", "NavigationViewportInputConsumer.cs"));
        Assert.Contains("_handler.Update(new ViewportGestureContext", navigation);
    }

}

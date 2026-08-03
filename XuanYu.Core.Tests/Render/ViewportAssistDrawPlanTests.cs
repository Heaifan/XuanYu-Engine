using XuanYu.Core.Math;
using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render;

// F3-F1：导航 Gizmo 恒为最后一项（Overlay Pass 收尾）。
public sealed class ViewportAssistDrawPlanTests
{
    [Fact]
    public void Default_assist_does_not_draw_world_axes()
    {
        var projection = new RenderProjection(default, [], true, Vector3d.Zero,
            Assist: EditorViewportAssistState.Default);

        var plan = RenderDrawPlan.GetFrameDrawPlan(projection);

        // F2-R2 顺序：背景 → 网格 → 原点（无地图、无世界轴）；F3-F1：导航 Gizmo 收尾。
        Assert.Equal(RenderDrawKind.EditorBackground, plan[0].Kind);
        Assert.Equal(RenderDrawKind.EditorReferenceGrid, plan[1].Kind);
        Assert.Equal(RenderDrawKind.WorldOrigin, plan[2].Kind);
        Assert.DoesNotContain(plan, entry => entry.Kind == RenderDrawKind.WorldAxes);
        Assert.Equal(RenderDrawKind.NavigationGizmo, plan[^1].Kind);
    }

    [Fact]
    public void Enabled_world_axes_draws_after_grid_before_entities_and_gizmo()
    {
        var projection = new RenderProjection(default, [], true, Vector3d.Zero,
            Assist: new(true, true, true, true));

        var plan = RenderDrawPlan.GetFrameDrawPlan(projection);

        // F2-R2 顺序：背景 → 网格 → 原点 → 世界轴。
        var axes = plan[3];
        Assert.Equal(RenderDrawKind.WorldAxes, axes.Kind);
        Assert.Equal(-1, axes.EntityIndex);
        Assert.Null(axes.EntityType);
        Assert.Equal(RenderDrawKind.NavigationGizmo, plan[^1].Kind);
    }

    [Fact]
    public void Disabled_assist_does_not_enter_frame_plan()
    {
        var projection = new RenderProjection(default, [], false, Vector3d.Zero,
            Assist: new(false, false, false, false));

        var plan = RenderDrawPlan.GetFrameDrawPlan(projection);

        // F3-F1：导航 Gizmo 独立于视口辅助开关（始终显示覆盖层）。
        Assert.Single(plan);
        Assert.Equal(RenderDrawKind.NavigationGizmo, plan[0].Kind);
    }
}

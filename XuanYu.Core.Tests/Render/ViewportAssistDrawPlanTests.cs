using XuanYu.Core.Math;
using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render;

public sealed class ViewportAssistDrawPlanTests
{
    [Fact]
    public void Enabled_assist_draws_before_entities_and_gizmo()
    {
        var projection = new RenderProjection(default, [], true, Vector3d.Zero,
            Assist: EditorViewportAssistState.Default);

        var plan = RenderDrawPlan.GetFrameDrawPlan(projection);

        Assert.Equal(RenderDrawKind.EditorBackground, plan[0].Kind);
        Assert.Equal(RenderDrawKind.EditorGrid, plan[1].Kind);
        Assert.Equal(RenderDrawKind.WorldOrigin, plan[2].Kind);
        Assert.Equal(RenderDrawKind.WorldAxes, plan[3].Kind);
        Assert.Equal(RenderDrawKind.MoveGizmo, plan[^1].Kind);
    }

    [Fact]
    public void Disabled_assist_does_not_enter_frame_plan()
    {
        var projection = new RenderProjection(default, [], false, Vector3d.Zero,
            Assist: new(false, false, false, false));

        var plan = RenderDrawPlan.GetFrameDrawPlan(projection);

        Assert.Empty(plan);
    }
}

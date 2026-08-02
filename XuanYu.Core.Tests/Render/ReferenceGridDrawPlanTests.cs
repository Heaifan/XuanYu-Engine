using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render;

// MAP-A-R1-D5-R1-F2A：DrawPlan 网格存在性（有/无地图）。
public sealed class ReferenceGridDrawPlanTests
{
    [Fact]
    public void Draw_plan_keeps_reference_grid_with_and_without_map()
    {
        var projNoMap = new RenderProjection(default, [], false, default,
            Assist: EditorViewportAssistState.Default);
        var planNoMap = RenderDrawPlan.GetFrameDrawPlan(projNoMap);
        var gridIndex = System.Array.FindIndex(
            planNoMap.ToArray(), e => e.Kind == RenderDrawKind.EditorReferenceGrid);
        Assert.True(gridIndex >= 0, "无地图时网格必须存在");
        Assert.Equal(RenderDrawKind.EditorBackground, planNoMap[0].Kind);
        Assert.True(gridIndex > 0, "网格必须在天空之后");
    }
}

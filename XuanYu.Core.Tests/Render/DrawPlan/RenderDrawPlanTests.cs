using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render;

// R4-R3-R2：验证绘制计划——未选中仅 Fill(3)，选中 Fill(3) + OutlineRibbon(18)，
// 禁止第二个 Fill(3)，OutlineRibbon 总顶点数=18 且只覆盖三条边带。
public sealed class RenderDrawPlanTests
{
    [Fact]
    public void Unselected_entity_yields_Fill_3_only()
    {
        var entities = new[] { Entity(false) };

        var plan = RenderDrawPlan.GetDrawPlan(entities);

        Assert.Single(plan);
        Assert.Equal((3, false), plan[0]);
    }

    [Fact]
    public void Selected_entity_yields_Fill_3_then_OutlineRibbon_18()
    {
        var entities = new[] { Entity(true) };

        var plan = RenderDrawPlan.GetDrawPlan(entities);

        Assert.Equal(2, plan.Count);
        Assert.Equal((3, false), plan[0]);  // Fill
        Assert.Equal((18, true), plan[1]);  // OutlineRibbon
    }

    [Fact]
    public void Selected_entity_has_no_second_Fill_3()
    {
        var entities = new[] { Entity(true) };

        var plan = RenderDrawPlan.GetDrawPlan(entities);

        Assert.Equal(2, plan.Count);
        var fillCalls = plan.Where(p => !p.IsOutline).ToList();
        Assert.Single(fillCalls);
        Assert.Equal(3, fillCalls[0].VertexCount);
    }

    [Fact]
    public void OutlineRibbon_total_vertex_count_is_18()
    {
        var entities = new[] { Entity(true) };

        var plan = RenderDrawPlan.GetDrawPlan(entities);

        var outline = plan.Single(p => p.IsOutline);
        Assert.Equal(RenderDrawPlan.OutlineRibbonVertexCount, outline.VertexCount);
        Assert.Equal(18, outline.VertexCount);
    }

    [Fact]
    public void Mixed_selection_produces_correct_plan_for_each_entity()
    {
        var entities = new[]
        {
            Entity(true, 1),
            Entity(false, 2),
            Entity(true, 3)
        };

        var plan = RenderDrawPlan.GetDrawPlan(entities);

        // E1: Fill(3) + Outline(18), E2: Fill(3), E3: Fill(3) + Outline(18)
        Assert.Equal(5, plan.Count);
        Assert.Equal((3, false), plan[0]);  // E1 Fill
        Assert.Equal((18, true), plan[1]);  // E1 Outline
        Assert.Equal((3, false), plan[2]);  // E2 Fill
        Assert.Equal((3, false), plan[3]);  // E3 Fill
        Assert.Equal((18, true), plan[4]);  // E3 Outline
    }

    [Fact]
    public void No_selection_marks_all_entities_Fill_only()
    {
        var entities = new[]
        {
            Entity(false, 1),
            Entity(false, 2)
        };

        var plan = RenderDrawPlan.GetDrawPlan(entities);

        Assert.Equal(2, plan.Count);
        Assert.All(plan, p => Assert.False(p.IsOutline));
        Assert.All(plan, p => Assert.Equal(3, p.VertexCount));
    }

    static RenderEntityProjection Entity(bool isSelected, int id = 1) =>
        new(EntityId.FromInt(id), Vector3d.Zero, Vector3d.Zero, new Vector3d(1, 1, 1), isSelected);
}
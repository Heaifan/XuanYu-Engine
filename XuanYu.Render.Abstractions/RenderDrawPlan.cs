namespace XuanYu.Render.Abstractions;

// R4-R3-R2：实体绘制计划提取，纯逻辑函数，供 Vulkan 绘制层与测试共同使用。
// 未选中实体：Fill(3 顶点)；选中实体：Fill(3) + OutlineRibbon(18)。
// 禁止复制完整面（第二个 Fill(3)），禁止重心坐标内部边线。
public static class RenderDrawPlan
{
    public const int FillVertexCount = 3;
    public const int OutlineRibbonVertexCount = 18;

    public static IReadOnlyList<(int VertexCount, bool IsOutline)> GetDrawPlan(
        IReadOnlyList<RenderEntityProjection> entities)
    {
        var plan = new List<(int, bool)>(entities.Count * 2);
        foreach (var e in entities)
        {
            plan.Add((FillVertexCount, false));
            if (e.IsSelected)
                plan.Add((OutlineRibbonVertexCount, true));
        }
        return plan;
    }
}
namespace XuanYu.Render.Abstractions;

// R4-R3-R2：实体绘制计划提取，纯逻辑函数，供 Vulkan 绘制层与测试共同使用。
// 未选中实体：Fill(3 顶点)；选中实体：Fill(3) + OutlineRibbon(18)。
// 禁止复制完整面（第二个 Fill(3)），禁止重心坐标内部边线。
public static class RenderDrawPlan
{
    public const int FillVertexCount = 3;
    public const int OutlineRibbonVertexCount = 18;
    public const int CubeFillVertexCount = 36;
    public const int CubeOutlineRibbonVertexCount = 72;

    public readonly record struct Entry(
        RenderEntityType EntityType,
        int VertexCount,
        bool IsOutline);

    public static IReadOnlyList<Entry> GetTypedDrawPlan(
        IReadOnlyList<RenderEntityProjection> entities)
    {
        var plan = new List<Entry>(entities.Count * 2);
        foreach (var entity in entities)
        {
            var fill = entity.EntityType == RenderEntityType.Cube
                ? CubeFillVertexCount : FillVertexCount;
            var outline = entity.EntityType == RenderEntityType.Cube
                ? CubeOutlineRibbonVertexCount : OutlineRibbonVertexCount;
            plan.Add(new Entry(entity.EntityType, fill, false));
            if (entity.IsSelected) plan.Add(new Entry(entity.EntityType, outline, true));
        }
        return plan;
    }

    public static IReadOnlyList<(int VertexCount, bool IsOutline)> GetDrawPlan(
        IReadOnlyList<RenderEntityProjection> entities)
    {
        return GetTypedDrawPlan(entities)
            .Select(x => (x.VertexCount, x.IsOutline))
            .ToArray();
    }
}

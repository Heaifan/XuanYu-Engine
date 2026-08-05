namespace XuanYu.Render.Abstractions;

// R4-R3-R2：实体绘制计划提取（typed 部分），供 Vulkan 与测试共同使用。
public static partial class RenderDrawPlan
{
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
            var fill = FillVertices(entity);
            var outline = entity.EntityType == RenderEntityType.Cube
                ? CubeOutlineRibbonVertexCount : OutlineRibbonVertexCount;
            plan.Add(new Entry(entity.EntityType, fill, false));
            if (entity.IsSelected && entity.EntityType != RenderEntityType.StaticModel)
                plan.Add(new Entry(entity.EntityType, outline, true));
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

    static int FillVertices(RenderEntityProjection entity) =>
        entity.EntityType switch
        {
            RenderEntityType.Cube => CubeFillVertexCount,
            RenderEntityType.StaticModel => 0,
            _ => FillVertexCount
        };
}

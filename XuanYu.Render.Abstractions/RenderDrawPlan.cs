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
    public const int MoveGizmoVertexCount = 36;
    public const int RotateGizmoVertexCount = 864;
    public const int ScaleGizmoVertexCount = 252;

    public readonly record struct Entry(
        RenderEntityType EntityType,
        int VertexCount,
        bool IsOutline);

    public readonly record struct FrameEntry(
        RenderDrawKind Kind,
        int VertexCount,
        int EntityIndex = -1,
        RenderEntityType? EntityType = null);

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

    public static IReadOnlyList<FrameEntry> GetFrameDrawPlan(RenderProjection projection)
    {
        var plan = new List<FrameEntry>(projection.Entities.Count * 2 + 1);
        for (var i = 0; i < projection.Entities.Count; i++)
        {
            var entity = projection.Entities[i];
            var fill = entity.EntityType == RenderEntityType.Cube
                ? CubeFillVertexCount : FillVertexCount;
            var outline = entity.EntityType == RenderEntityType.Cube
                ? CubeOutlineRibbonVertexCount : OutlineRibbonVertexCount;
            plan.Add(new FrameEntry(RenderDrawKind.EntityFill, fill, i, entity.EntityType));
            if (entity.IsSelected)
                plan.Add(new FrameEntry(RenderDrawKind.EntityOutline, outline, i, entity.EntityType));
        }
        if (projection.ScaleGizmoVisible)
            plan.Add(new FrameEntry(RenderDrawKind.ScaleGizmo, ScaleGizmoVertexCount));
        else if (projection.RotateGizmoVisible)
            plan.Add(new FrameEntry(RenderDrawKind.RotateGizmo, RotateGizmoVertexCount));
        else if (projection.GizmoVisible)
            plan.Add(new FrameEntry(RenderDrawKind.MoveGizmo, MoveGizmoVertexCount));
        return plan;
    }
}

public enum RenderDrawKind
{
    EntityFill,
    EntityOutline,
    MoveGizmo,
    RotateGizmo,
    ScaleGizmo
}

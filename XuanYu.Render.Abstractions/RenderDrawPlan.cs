namespace XuanYu.Render.Abstractions;

// R4-R3-R2：实体绘制计划提取，供 Vulkan 与测试共同使用。
public static class RenderDrawPlan
{
    public const int FillVertexCount = 3;
    public const int OutlineRibbonVertexCount = 18;
    public const int CubeFillVertexCount = 36;
    public const int CubeOutlineRibbonVertexCount = 72;
    public const int MoveGizmoVertexCount = 216;
    public const int RotateGizmoVertexCount = 900;
    public const int ScaleGizmoVertexCount = 252;
    public const int BackgroundVertexCount = 3;
    // MAP-A-R1-D5-R1-F2：独立编辑器参考网格 Pass（全屏三角形，片元解析世界 Z=0 平面）。
    public const int ReferenceGridVertexCount = 3;
    public const int OriginVertexCount = 36;
    public const int WorldAxesVertexCount = 108;
    // D4：地图边界线（四条边 + 四角标识），CPU 生成细条四边形顶点。
    public const int MapBoundsVertexCount = 8 * 3 * 2;
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
    public static IReadOnlyList<FrameEntry> GetFrameDrawPlan(RenderProjection projection)
    {
        var assist = projection.AssistState;
        var plan = new List<FrameEntry>(projection.Entities.Count * 2 + 6);
        if (assist.ShowEditorBackground) plan.Add(new FrameEntry(RenderDrawKind.EditorBackground, BackgroundVertexCount));
        // F2-R2 顺序（方案 12）：地形 → 网格 → 原点 → 轴 → 实体填充 → 轮廓 → Gizmo。
        // 网格画在地形之上（深度偏移），原点/轴覆盖网格，实体最终遮挡一切。
        if (projection.HasMap) plan.Add(new FrameEntry(RenderDrawKind.MapBounds, MapBoundsVertexCount));
        if (assist.ViewPlaneGrid != EditorViewPlaneGridKind.None)
        {
            // F3-F4：正交标准视图的视图平面网格（±X→YZ / ±Y→XZ），画在地面网格同一层。
            plan.Add(new FrameEntry(RenderDrawKind.EditorViewPlaneGrid, ReferenceGridVertexCount));
        }
        else if (assist.ShowGrid) plan.Add(new FrameEntry(RenderDrawKind.EditorReferenceGrid, ReferenceGridVertexCount));
        if (assist.ShowOrigin) plan.Add(new FrameEntry(RenderDrawKind.WorldOrigin, OriginVertexCount));
        if (assist.ShowWorldAxes) plan.Add(new FrameEntry(RenderDrawKind.WorldAxes, WorldAxesVertexCount));
        for (var i = 0; i < projection.Entities.Count; i++)
        {
            var entity = projection.Entities[i];
            plan.Add(new FrameEntry(RenderDrawKind.EntityFill, FillVertices(entity), i, entity.EntityType));
        }
        for (var i = 0; i < projection.Entities.Count; i++)
        {
            var entity = projection.Entities[i];
            if (!entity.IsSelected || entity.EntityType == RenderEntityType.StaticModel) continue;
            plan.Add(new FrameEntry(RenderDrawKind.EntityOutline,
                entity.EntityType == RenderEntityType.Cube ? CubeOutlineRibbonVertexCount : OutlineRibbonVertexCount,
                i, entity.EntityType));
        }
        if (projection.ScaleGizmoVisible) plan.Add(new FrameEntry(RenderDrawKind.ScaleGizmo, ScaleGizmoVertexCount));
        else if (projection.RotateGizmoVisible) plan.Add(new FrameEntry(RenderDrawKind.RotateGizmo, RotateGizmoVertexCount));
        else if (projection.GizmoVisible) plan.Add(new FrameEntry(RenderDrawKind.MoveGizmo, MoveGizmoVertexCount));
        // F3-F1：导航 Gizmo Overlay 始终最后绘制（深度关、不受原生窗口遮挡）。
        plan.Add(new FrameEntry(RenderDrawKind.NavigationGizmo, ReferenceGridVertexCount));
        return plan;
    }
    static int FillVertices(RenderEntityProjection entity) =>
        entity.EntityType switch
        {
            RenderEntityType.Cube => CubeFillVertexCount,
            RenderEntityType.StaticModel => 0,
            _ => FillVertexCount
        };
}
public enum RenderDrawKind
{
    EditorBackground, EditorReferenceGrid, WorldOrigin, WorldAxes, MapBounds,
    EntityFill, EntityOutline, MoveGizmo, RotateGizmo, ScaleGizmo, NavigationGizmo, EditorViewPlaneGrid
}

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
    // D5-R1：视觉无限参考网格（shader 按相机位置动态生成，42 条线）。
    public const int GridVertexCount = 252;
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
        if (assist.ShowGrid) plan.Add(new FrameEntry(RenderDrawKind.EditorGrid, GridVertexCount));
        if (assist.ShowOrigin) plan.Add(new FrameEntry(RenderDrawKind.WorldOrigin, OriginVertexCount));
        if (assist.ShowWorldAxes) plan.Add(new FrameEntry(RenderDrawKind.WorldAxes, WorldAxesVertexCount));
        if (projection.HasMap)
        {
            // D5-R1：地图存在时参考网格保留（地图外继续延伸），地表绘制在地图矩形范围内，
            // 网格由 shader 按地图范围裁切避免穿透地表；卸载后网格继续存在。
            plan.Add(new FrameEntry(RenderDrawKind.MapBounds, MapBoundsVertexCount));
        }
        for (var i = 0; i < projection.Entities.Count; i++)
        {
            var entity = projection.Entities[i];
            var fill = FillVertices(entity);
            var outline = entity.EntityType == RenderEntityType.Cube
                ? CubeOutlineRibbonVertexCount : OutlineRibbonVertexCount;
            plan.Add(new FrameEntry(RenderDrawKind.EntityFill, fill, i, entity.EntityType));
            if (entity.IsSelected && entity.EntityType != RenderEntityType.StaticModel) plan.Add(new FrameEntry(RenderDrawKind.EntityOutline, outline, i, entity.EntityType));
        }
        if (projection.ScaleGizmoVisible) plan.Add(new FrameEntry(RenderDrawKind.ScaleGizmo, ScaleGizmoVertexCount));
        else if (projection.RotateGizmoVisible) plan.Add(new FrameEntry(RenderDrawKind.RotateGizmo, RotateGizmoVertexCount));
        else if (projection.GizmoVisible) plan.Add(new FrameEntry(RenderDrawKind.MoveGizmo, MoveGizmoVertexCount));
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
    EditorBackground,
    EditorGrid,
    WorldOrigin,
    WorldAxes,
    MapBounds,
    EntityFill,
    EntityOutline,
    MoveGizmo,
    RotateGizmo,
    ScaleGizmo
}

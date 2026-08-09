namespace XuanYu.Render.Abstractions;

// R4-R3-R2：实体绘制计划提取（帧级），供 Vulkan 与测试共同使用。
// D4：地图地面（MapGround）与边界（MapBounds）分项——地面/边界图层显隐分别过滤绘制项，
// 隐藏 = 跳过对应绘制项（渲染过滤，不删除领域数据）；网格/原点/轴/Gizmo 不受影响。
public static partial class RenderDrawPlan
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
    // D3：地图边界线（四条边细条四边形，CPU 生成），24 顶点。
    public const int MapBoundsVertexCount = MapBoundsGeometryBuilder.VertexCount;
    // D4：地图地面索引数（4 顶点 6 索引，两个三角形）。
    public const int MapGroundIndexCount = MapSurfaceGeometry.IndexCount;

    public readonly record struct FrameEntry(
        RenderDrawKind Kind,
        int VertexCount,
        int EntityIndex = -1,
        RenderEntityType? EntityType = null);

    public static IReadOnlyList<FrameEntry> GetFrameDrawPlan(RenderProjection projection)
    {
        var assist = projection.AssistState;
        var plan = new List<FrameEntry>(projection.Entities.Count * 2 + 6);
        if (assist.ShowEditorBackground) plan.Add(new FrameEntry(RenderDrawKind.EditorBackground, BackgroundVertexCount));
        // D5 Overlay 顺序：地形 → 网格/轴 → 实体 → 原点 → 变换 Gizmo → 导航 Gizmo。
        // 原点独立覆盖层不参与深度测试，并在实体之后绘制，避免模型或地面遮挡中心标记。
        if (projection.HasMap)
        {
            if (projection.Map.ShowGround) plan.Add(new FrameEntry(RenderDrawKind.MapGround, MapGroundIndexCount));
            if (projection.Map.ShowBoundary) plan.Add(new FrameEntry(RenderDrawKind.MapBounds, MapBoundsVertexCount));
        }
        if (assist.ViewPlaneGrid != EditorViewPlaneGridKind.None)
        {
            // F3-F4：正交标准视图的视图平面网格（±X→YZ / ±Y→XZ），画在地面网格同一层。
            plan.Add(new FrameEntry(RenderDrawKind.EditorViewPlaneGrid, ReferenceGridVertexCount));
        }
        else if (assist.ShowGrid) plan.Add(new FrameEntry(RenderDrawKind.EditorReferenceGrid, ReferenceGridVertexCount));
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
        for (var i = 0; i < projection.RegionModelResources.Count; i++)
            plan.Add(new FrameEntry(RenderDrawKind.MapRegion, 0, i));
        if (assist.ShowOrigin) plan.Add(new FrameEntry(RenderDrawKind.WorldOrigin, OriginVertexCount));
        if (projection.ScaleGizmoVisible) plan.Add(new FrameEntry(RenderDrawKind.ScaleGizmo, ScaleGizmoVertexCount));
        else if (projection.RotateGizmoVisible) plan.Add(new FrameEntry(RenderDrawKind.RotateGizmo, RotateGizmoVertexCount));
        else if (projection.GizmoVisible) plan.Add(new FrameEntry(RenderDrawKind.MoveGizmo, MoveGizmoVertexCount));
        // F3-F1：导航 Gizmo Overlay 始终最后绘制（深度关、不受原生窗口遮挡）。
        plan.Add(new FrameEntry(RenderDrawKind.NavigationGizmo, ReferenceGridVertexCount));
        return plan;
    }
}
public enum RenderDrawKind
{
    EditorBackground, EditorReferenceGrid, WorldOrigin, WorldAxes, MapGround, MapBounds,
    MapRegion, EntityFill, EntityOutline, MoveGizmo, RotateGizmo, ScaleGizmo, NavigationGizmo, EditorViewPlaneGrid
}

using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render;

// MAP-A-R1-D5-R1-F2-R2：DrawPlan 合同——顺序（方案 12）与开关独立（方案 11.2）。
// 顺序：地形(MapBounds) → 网格/世界轴 → 实体填充 → 轮廓 → 原点 Overlay → Gizmo。
public sealed class ReferenceGridDrawPlanTests
{
    static RenderProjection Projection(bool hasMap, bool withEntity, bool showGrid, bool showAxes = true, bool showOrigin = true)
    {
        var entities = withEntity
            ? new[] { new RenderEntityProjection(EntityId.FromInt(1), Vector3d.Zero, Vector3d.Zero, new(1.0, 1.0, 1.0), IsSelected: true) }
            : [];
        return new RenderProjection(default, entities, true, Vector3d.Zero,
            Assist: new EditorViewportAssistState(
                ShowGrid: showGrid, ShowOrigin: showOrigin, ShowWorldAxes: showAxes, ShowEditorBackground: false),
            Map: hasMap ? new MapRenderSnapshot(
                "21e4a2d34d4a4a1eb2539eac76d412a8", 2000, 2000,
                Core.Map.MapSurfaceKind.Flat, 0, 0, 1, 1, 1) : default);
    }

    [Theory]
    [InlineData(false, false), InlineData(true, false), InlineData(false, true), InlineData(true, true)]
    public void Grid_kept_with_map_and_entity_combinations(bool hasMap, bool withEntity)
    {
        var plan = RenderDrawPlan.GetFrameDrawPlan(Projection(hasMap, withEntity, true));
        Assert.Contains(plan, e => e.Kind == RenderDrawKind.EditorReferenceGrid);
    }

    // D5 Overlay 顺序：Terrain < Grid < WorldAxes < EntityFill < EntityOutline < Origin < Gizmo。
    [Fact]
    public void Draw_order_matches_scheme_twelve()
    {
        var plan = RenderDrawPlan.GetFrameDrawPlan(Projection(true, true, true)).ToList();
        int IndexOf(RenderDrawKind kind) => plan.FindIndex(e => e.Kind == kind);
        var terrain = IndexOf(RenderDrawKind.MapBounds);
        var grid = IndexOf(RenderDrawKind.EditorReferenceGrid);
        var origin = IndexOf(RenderDrawKind.WorldOrigin);
        var axes = IndexOf(RenderDrawKind.WorldAxes);
        var fill = IndexOf(RenderDrawKind.EntityFill);
        var outline = IndexOf(RenderDrawKind.EntityOutline);
        var gizmo = IndexOf(RenderDrawKind.MoveGizmo);
        Assert.True(terrain >= 0 && terrain < grid, "地形必须在网格之前");
        Assert.True(grid < axes && axes < fill && fill < outline, "网格→轴→实体顺序错误");
        Assert.True(outline < origin && origin < gizmo, "实体→原点 Overlay→Gizmo 顺序错误");
    }

    // 开关独立（方案 11.2）：网格关世界轴开 → 只有轴；网格开世界轴关 → 只有网格。
    [Fact]
    public void Switches_are_independent()
    {
        var planGridOff = RenderDrawPlan.GetFrameDrawPlan(Projection(false, false, false, showAxes: true, showOrigin: true)).ToList();
        Assert.DoesNotContain(planGridOff, e => e.Kind == RenderDrawKind.EditorReferenceGrid);
        Assert.Contains(planGridOff, e => e.Kind == RenderDrawKind.WorldAxes);
        Assert.Contains(planGridOff, e => e.Kind == RenderDrawKind.WorldOrigin);

        var planAxesOff = RenderDrawPlan.GetFrameDrawPlan(Projection(false, false, true, showAxes: false, showOrigin: true)).ToList();
        Assert.Contains(planAxesOff, e => e.Kind == RenderDrawKind.EditorReferenceGrid);
        Assert.DoesNotContain(planAxesOff, e => e.Kind == RenderDrawKind.WorldAxes);

        var planOriginOff = RenderDrawPlan.GetFrameDrawPlan(Projection(false, false, true, showAxes: true, showOrigin: false)).ToList();
        Assert.DoesNotContain(planOriginOff, e => e.Kind == RenderDrawKind.WorldOrigin);
        Assert.Contains(planOriginOff, e => e.Kind == RenderDrawKind.WorldAxes);

        var planAllOff = RenderDrawPlan.GetFrameDrawPlan(Projection(false, false, false, showAxes: false, showOrigin: false)).ToList();
        Assert.DoesNotContain(planAllOff, e => e.Kind is RenderDrawKind.EditorReferenceGrid or RenderDrawKind.WorldAxes or RenderDrawKind.WorldOrigin);
    }
    [Fact]
    public void Grid_absent_when_show_grid_disabled()
    {
        var plan = RenderDrawPlan.GetFrameDrawPlan(Projection(true, true, false));
        Assert.DoesNotContain(plan, e => e.Kind == RenderDrawKind.EditorReferenceGrid);
    }

    // F3-F4：正交标准视图启用视图平面网格时替代地面网格条目；非标准视图无视图平面网格。
    [Fact]
    public void View_plane_grid_entry_replaces_reference_grid()
    {
        var ortho = new RenderProjection(default, [], true, Vector3d.Zero,
            Assist: new EditorViewportAssistState(ShowGrid: true, ShowOrigin: false,
                ShowWorldAxes: false, ShowEditorBackground: false,
                ViewPlaneGrid: EditorViewPlaneGridKind.YZ));
        var plan = RenderDrawPlan.GetFrameDrawPlan(ortho);
        Assert.Contains(plan, e => e.Kind == RenderDrawKind.EditorViewPlaneGrid);
        Assert.DoesNotContain(plan, e => e.Kind == RenderDrawKind.EditorReferenceGrid);
    }
    [Fact]
    public void No_view_plane_grid_without_standard_view()
    {
        var perspective = new RenderProjection(default, [], true, Vector3d.Zero,
            Assist: new EditorViewportAssistState(ShowGrid: true, ShowOrigin: false,
                ShowWorldAxes: false, ShowEditorBackground: false));
        var plan = RenderDrawPlan.GetFrameDrawPlan(perspective);
        Assert.Contains(plan, e => e.Kind == RenderDrawKind.EditorReferenceGrid);
        Assert.DoesNotContain(plan, e => e.Kind == RenderDrawKind.EditorViewPlaneGrid);
    }
}

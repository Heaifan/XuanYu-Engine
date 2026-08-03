using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render;

// MAP-A-R1-D5-R1-F2：DrawPlan 网格合同——有/无地图、有/无实体时网格都在，
// 且顺序位于地形/实体之后、Gizmo 之前（实体可遮挡网格）。
public sealed class ReferenceGridDrawPlanTests
{
    static RenderProjection Projection(bool hasMap, bool withEntity, bool showGrid)
    {
        var entities = withEntity
            ? new[] { new RenderEntityProjection(EntityId.FromInt(1), Vector3d.Zero, Vector3d.Zero, new(1.0, 1.0, 1.0)) }
            : [];
        return new RenderProjection(default, entities, true, Vector3d.Zero,
            Assist: new EditorViewportAssistState(
                ShowGrid: showGrid, ShowOrigin: false, ShowWorldAxes: false, ShowEditorBackground: false),
            Map: hasMap ? new Core.Map.MapRenderSnapshot(
                "21e4a2d34d4a4a1eb2539eac76d412a8", "M", 2000, 2000,
                Core.Map.MapSurfaceKind.Flat, 0, 0, 1, 1,
                -0.35, -0.55, -0.75, 1.0, 0.35) : default);
    }

    [Theory]
    [InlineData(false, false), InlineData(true, false), InlineData(false, true), InlineData(true, true)]
    public void Grid_kept_with_map_and_entity_combinations(bool hasMap, bool withEntity)
    {
        var plan = RenderDrawPlan.GetFrameDrawPlan(Projection(hasMap, withEntity, true));
        var gridIndex = System.Array.FindIndex(
            plan.ToArray(), e => e.Kind == RenderDrawKind.EditorReferenceGrid);
        Assert.True(gridIndex >= 0, "网格必须存在");
    }

    [Fact]
    public void Grid_after_entities_before_gizmo()
    {
        var plan = RenderDrawPlan.GetFrameDrawPlan(Projection(false, true, true));
        var gridIndex = System.Array.FindIndex(
            plan.ToArray(), e => e.Kind == RenderDrawKind.EditorReferenceGrid);
        var entityIndex = System.Array.FindIndex(
            plan.ToArray(), e => e.Kind == RenderDrawKind.EntityFill);
        var gizmoIndex = System.Array.FindIndex(
            plan.ToArray(), e => e.Kind == RenderDrawKind.MoveGizmo);
        Assert.True(entityIndex >= 0 && entityIndex < gridIndex,
            "网格必须在实体之后");
        Assert.True(gridIndex < gizmoIndex, "网格必须在 Gizmo 之前");
    }

    [Fact]
    public void Grid_absent_when_show_grid_disabled()
    {
        var plan = RenderDrawPlan.GetFrameDrawPlan(Projection(true, true, false));
        Assert.DoesNotContain(plan, e => e.Kind == RenderDrawKind.EditorReferenceGrid);
    }
}

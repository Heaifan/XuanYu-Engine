using XuanYu.Core.Map;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Space;
using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render;

// MAP-A-R1-D4/D5-R1（F2-R2/D4）：RenderProjection 携带地图快照后，参考网格保留（无限参考平面，
// 不按地图矩形裁切，深度偏移保证共面稳定）；F2-R2 顺序：地形 → 网格 → 原点 → 轴。
// D4：地面/边界分项（MapGround/MapBounds），显隐分别过滤绘制项（R01～R05）。
public sealed class MapRenderDrawPlanTests
{
    static RenderProjection ProjectionWithMap(MapRenderSnapshot? map = null)
    {
        var camera = DefaultEditorCamera.Create(1);
        return new RenderProjection(
            new RenderCameraProjection(camera.Position, camera.Forward, camera.Up,
                camera.VerticalFovDegrees, camera.NearPlane, camera.FarPlane, camera.Revision),
            [],
            false,
            Vector3d.Zero,
            Assist: EditorViewportAssistState.Default,
            Map: map ?? default);
    }

    static MapRenderSnapshot DefaultSnapshot() => new(
        "21e4a2d34d4a4a1eb2539eac76d412a8", 2000, 2000,
        MapSurfaceKind.Flat, 0, 0, 1, 1, 1);

    [Fact]
    public void Without_map_grid_remains_and_no_bounds()
    {
        var plan = RenderDrawPlan.GetFrameDrawPlan(ProjectionWithMap());
        Assert.Contains(plan, x => x.Kind == RenderDrawKind.EditorReferenceGrid);
        Assert.DoesNotContain(plan, x => x.Kind == RenderDrawKind.MapBounds);
        Assert.DoesNotContain(plan, x => x.Kind == RenderDrawKind.MapGround);
    }

    [Fact]
    public void With_map_grid_kept_and_bounds_added()
    {
        // D5-R1/F2-R2：地图存在时参考网格保留（无限参考平面，不按地图矩形裁切），边界线添加。
        var plan = RenderDrawPlan.GetFrameDrawPlan(ProjectionWithMap(DefaultSnapshot()));
        Assert.Contains(plan, x => x.Kind == RenderDrawKind.EditorReferenceGrid);
        Assert.Contains(plan, x => x.Kind == RenderDrawKind.MapBounds
            && x.VertexCount == RenderDrawPlan.MapBoundsVertexCount);
        Assert.Contains(plan, x => x.Kind == RenderDrawKind.MapGround
            && x.VertexCount == RenderDrawPlan.MapGroundIndexCount);
    }

    [Fact]
    public void With_map_sky_still_drawn()
    {
        var plan = RenderDrawPlan.GetFrameDrawPlan(ProjectionWithMap(DefaultSnapshot()));
        Assert.Contains(plan, x => x.Kind == RenderDrawKind.EditorBackground);
    }

    [Fact]
    public void R01_R02_ground_visibility_controls_map_ground_entry()
    {
        var shown = RenderDrawPlan.GetFrameDrawPlan(ProjectionWithMap(DefaultSnapshot()));
        Assert.Contains(shown, x => x.Kind == RenderDrawKind.MapGround);
        var hidden = RenderDrawPlan.GetFrameDrawPlan(
            ProjectionWithMap(DefaultSnapshot() with { ShowGround = false }));
        Assert.DoesNotContain(hidden, x => x.Kind == RenderDrawKind.MapGround);
    }

    [Fact]
    public void R03_R04_boundary_visibility_controls_map_bounds_entry()
    {
        var shown = RenderDrawPlan.GetFrameDrawPlan(ProjectionWithMap(DefaultSnapshot()));
        Assert.Contains(shown, x => x.Kind == RenderDrawKind.MapBounds);
        var hidden = RenderDrawPlan.GetFrameDrawPlan(
            ProjectionWithMap(DefaultSnapshot() with { ShowBoundary = false }));
        Assert.DoesNotContain(hidden, x => x.Kind == RenderDrawKind.MapBounds);
    }

    [Fact]
    public void R05_hiding_system_layers_keeps_grid_origin_and_gizmo()
    {
        var plan = RenderDrawPlan.GetFrameDrawPlan(ProjectionWithMap(
            DefaultSnapshot() with { ShowGround = false, ShowBoundary = false }));
        Assert.Contains(plan, x => x.Kind == RenderDrawKind.EditorReferenceGrid);
        Assert.Contains(plan, x => x.Kind == RenderDrawKind.WorldOrigin);
        Assert.Contains(plan, x => x.Kind == RenderDrawKind.NavigationGizmo);
        Assert.DoesNotContain(plan, x => x.Kind == RenderDrawKind.MapGround);
        Assert.DoesNotContain(plan, x => x.Kind == RenderDrawKind.MapBounds);
    }
}

using XuanYu.Core.Map;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Space;
using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render;

// MAP-A-R1-D4：RenderProjection 携带地图快照后，绘制计划移除无限网格并添加边界线。
public sealed class MapRenderDrawPlanTests
{
    static RenderProjection ProjectionWithMap(bool hasMap)
    {
        var camera = DefaultEditorCamera.Create(1);
        return new RenderProjection(
            new RenderCameraProjection(camera.Position, camera.Forward, camera.Up,
                camera.VerticalFovDegrees, camera.NearPlane, camera.FarPlane, camera.Revision),
            [],
            false,
            Vector3d.Zero,
            Assist: EditorViewportAssistState.Default,
            Map: hasMap ? new MapRenderSnapshot(
                "21e4a2d34d4a4a1eb2539eac76d412a8", "M", 2000, 2000,
                MapSurfaceKind.Flat, 0, 0, 1, 1,
                -0.35, -0.55, -0.75, 1.0, 0.35)
                : default);
    }

    [Fact]
    public void Without_map_grid_remains_and_no_bounds()
    {
        var plan = RenderDrawPlan.GetFrameDrawPlan(ProjectionWithMap(false));
        Assert.Contains(plan, x => x.Kind == RenderDrawKind.EditorGrid);
        Assert.DoesNotContain(plan, x => x.Kind == RenderDrawKind.MapBounds);
    }

    [Fact]
    public void With_map_grid_removed_and_bounds_added()
    {
        var plan = RenderDrawPlan.GetFrameDrawPlan(ProjectionWithMap(true));
        Assert.DoesNotContain(plan, x => x.Kind == RenderDrawKind.EditorGrid);
        Assert.Contains(plan, x => x.Kind == RenderDrawKind.MapBounds
            && x.VertexCount == RenderDrawPlan.MapBoundsVertexCount);
    }

    [Fact]
    public void With_map_sky_still_drawn()
    {
        var plan = RenderDrawPlan.GetFrameDrawPlan(ProjectionWithMap(true));
        Assert.Contains(plan, x => x.Kind == RenderDrawKind.EditorBackground);
    }
}

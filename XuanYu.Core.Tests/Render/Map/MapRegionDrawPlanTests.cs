using XuanYu.Core.Math;
using XuanYu.Core.Spatial;
using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render.Map;

public sealed class MapRegionDrawPlanTests
{
    [Fact]
    public void Region_resources_are_drawn_before_entities()
    {
        var resource = new RenderStaticModelResource(
            new RenderStaticModelKey("region"), 1,
            [new(new Vector3d(0, 0, 0), new Vector3d(0, 0, 1), 0, 0)],
            [0, 0, 0], [new(0, 3, 0, RenderStaticModelColor.Neutral)],
            new SpatialAabb(Vector3d.Zero, Vector3d.Zero));
        var projection = new RenderProjection(default, [], false, Vector3d.Zero,
            RegionModels: [resource]);

        var plan = RenderDrawPlan.GetFrameDrawPlan(projection);

        Assert.Contains(plan, item => item.Kind == RenderDrawKind.MapRegion);
        var regionIndex = plan.Select((item, index) => (item, index))
            .First(x => x.item.Kind == RenderDrawKind.MapRegion).index;
        var gizmoIndex = plan.Select((item, index) => (item, index))
            .First(x => x.item.Kind == RenderDrawKind.NavigationGizmo).index;
        Assert.True(regionIndex < gizmoIndex);
    }
}

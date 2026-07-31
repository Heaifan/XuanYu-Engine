using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render;

public sealed class CubeRenderDrawPlanTests
{
    [Fact]
    public void Cube_fill_and_outline_keep_cube_type_and_vertex_contract()
    {
        var cube = new RenderEntityProjection(EntityId.FromInt(1), Vector3d.Zero,
            Vector3d.Zero, new(1, 1, 1), true, RenderEntityType.Cube);

        var plan = RenderDrawPlan.GetTypedDrawPlan([cube]);

        Assert.Equal(2, plan.Count);
        Assert.Equal(new RenderDrawPlan.Entry(RenderEntityType.Cube, 36, false), plan[0]);
        Assert.Equal(new RenderDrawPlan.Entry(RenderEntityType.Cube, 72, true), plan[1]);
    }
}

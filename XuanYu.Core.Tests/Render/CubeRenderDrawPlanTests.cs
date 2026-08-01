using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Space;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;
using XuanYu.World;

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

    [Theory]
    [InlineData(true, false, false, RenderDrawKind.MoveGizmo, 216)]
    [InlineData(false, true, false, RenderDrawKind.RotateGizmo, 900)]
    [InlineData(false, false, true, RenderDrawKind.ScaleGizmo, 252)]
    public void Selected_cube_frame_never_submits_legacy_triangle_for_any_transform_tool(
        bool move, bool rotate, bool scale, RenderDrawKind gizmoKind, int gizmoVertices)
    {
        var cube = new RenderEntityProjection(EntityId.FromInt(1), Vector3d.Zero,
            Vector3d.Zero, new(1, 1, 1), true, RenderEntityType.Cube);
        var projection = new RenderProjection(default, [cube], move, Vector3d.Zero,
            RotateGizmoVisible: rotate, ScaleGizmoVisible: scale);

        var plan = RenderDrawPlan.GetFrameDrawPlan(projection);

        Assert.DoesNotContain(plan, x => x.EntityType == RenderEntityType.LegacyMinimalTriangle);
        Assert.Contains(plan, x => x.Kind == RenderDrawKind.EntityFill &&
            x.EntityType == RenderEntityType.Cube && x.VertexCount == 36);
        Assert.Contains(plan, x => x.Kind == RenderDrawKind.EntityOutline &&
            x.EntityType == RenderEntityType.Cube && x.VertexCount == 72);
        Assert.Contains(plan, x => x.Kind == gizmoKind && x.VertexCount == gizmoVertices &&
            x.EntityType is null);
    }

    [Fact]
    public void Legacy_triangle_commands_require_explicit_legacy_entity_type()
    {
        var legacy = new RenderEntityProjection(EntityId.FromInt(2), Vector3d.Zero,
            Vector3d.Zero, new(1, 1, 1), true, RenderEntityType.LegacyMinimalTriangle);

        var plan = RenderDrawPlan.GetFrameDrawPlan(
            new RenderProjection(default, [legacy], false, Vector3d.Zero));

        Assert.Collection(plan,
            x => Assert.Equal((RenderDrawKind.EntityFill, 3, RenderEntityType.LegacyMinimalTriangle),
                (x.Kind, x.VertexCount, x.EntityType)),
            x => Assert.Equal((RenderDrawKind.EntityOutline, 18, RenderEntityType.LegacyMinimalTriangle),
                (x.Kind, x.VertexCount, x.EntityType)));
    }

    [Fact]
    public void Cube_world_type_reaches_the_final_frame_plan()
    {
        var entity = new SceneEntitySnapshot(EntityId.FromInt(3), "Cube",
            WorldEntityTypes.Cube, CommittedTransform.Identity);
        var snapshot = new SceneRenderSnapshot(entity, IsSelected: true,
            RenderEntities: [entity], Camera: DefaultEditorCamera.Create(1));

        var projection = SceneRenderProjectionAdapter.TryCreate(snapshot).Projection;
        var plan = RenderDrawPlan.GetFrameDrawPlan(projection);

        Assert.All(plan, x => Assert.Equal(RenderEntityType.Cube, x.EntityType));
    }
}

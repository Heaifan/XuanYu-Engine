using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Space;
using XuanYu.Core.Tests.Space;
using XuanYu.Core.Transform;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render;

public sealed partial class SceneRenderProjectionAdapterTests
{
    [Fact]
    public void Missing_camera_fails_without_default_projection()
    {
        var snapshot = new SceneRenderSnapshot(EntityAt(1, Vector3d.Zero));

        var result = SceneRenderProjectionAdapter.TryCreate(snapshot);

        Assert.False(result.Success);
        Assert.Contains("Camera", result.FailureReason);
        Assert.Equal(default, result.Projection);
    }

    [Fact]
    public void Explicit_camera_creates_projection_and_viewport_matrix()
    {
        var camera = TestCamera();
        var snapshot = new SceneRenderSnapshot(
            EntityAt(1, new Vector3d(1, 2, 3)),
            RenderEntities: [EntityAt(1, new Vector3d(1, 2, 3))],
            Camera: camera);

        var result = SceneRenderProjectionAdapter.TryCreate(snapshot);

        Assert.True(result.Success);
        Assert.Single(result.Projection.Entities);
        Assert.Equal(new Vector3d(1, 2, 3), result.Projection.Entities[0].Position);
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 7);
        var expected = ViewProjectionState.Create(camera, viewport).ViewProjection;
        var actual = result.Projection.Camera.ToViewProjection(viewport).ViewProjection;
        SpaceAssert.Near(expected.M11, actual.M11);
        SpaceAssert.Near(expected.M22, actual.M22);
    }

    [Fact]
    public void Preview_and_gizmo_are_resolved_before_render_boundary()
    {
        var active = EntityAt(1, Vector3d.Zero);
        var other = EntityAt(2, new Vector3d(5, 0, 0));
        var preview = new PreviewTransform(new Vector3d(2, 0, 0));
        var snapshot = new SceneRenderSnapshot(
            active, PreviewTransform: preview, ShowMoveGizmo: true,
            RenderEntities: [active, other], Camera: TestCamera());

        RenderProjection projection = SceneRenderProjectionAdapter.TryCreate(snapshot).Projection;

        Assert.True(projection.GizmoVisible);
        Assert.Equal(new Vector3d(2, 0, 0), projection.GizmoPosition);
        Assert.Equal(new Vector3d(2, 0, 0), projection.Entities[0].Position);
        Assert.Equal(new Vector3d(5, 0, 0), projection.Entities[1].Position);
    }

    static SceneEntitySnapshot EntityAt(int id, Vector3d position) =>
        new(EntityId.FromInt(id), $"E{id}", "Minimal",
            new CommittedTransform(position));

    static CameraState TestCamera() =>
        new(new Vector3d(0, 0, -5), Vector3d.UnitZ, Vector3d.UnitY, 60, 0.1, 100, 3);
}

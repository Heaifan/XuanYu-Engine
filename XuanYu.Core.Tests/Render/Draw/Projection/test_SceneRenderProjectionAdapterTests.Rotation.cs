using XuanYu.Core.Gizmo;
using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Space;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render;

public sealed partial class SceneRenderProjectionAdapterTests
{
    [Fact]
    public void Adapter_carries_entity_rotation_and_scale()
    {
        var rotation = new Vector3d(10, 20, 30);
        var scale = new Vector3d(2, 3, 4);
        var transform = new CommittedTransform(new Vector3d(1, 2, 3), rotation, scale);
        var entity = new SceneEntitySnapshot(EntityId.FromInt(1), "E1", "Minimal", transform);
        var snapshot = new SceneRenderSnapshot(entity, RenderEntities: [entity], Camera: TestCamera());

        RenderProjection projection = SceneRenderProjectionAdapter.TryCreate(snapshot).Projection;

        Assert.Equal(rotation, projection.Entities[0].Rotation);
        Assert.Equal(scale, projection.Entities[0].Scale);
        Assert.Equal(new Vector3d(1, 2, 3), projection.Entities[0].Position);
    }

    [Fact]
    public void Adapter_passes_explicit_rotate_gizmo_world_radius()
    {
        var camera = TestCamera();
        var snapshot = new SceneRenderSnapshot(
            EntityAt(1, Vector3d.Zero), ShowRotateGizmo: true, Camera: camera);

        var result = SceneRenderProjectionAdapter.TryCreate(snapshot, 2.5);

        Assert.True(result.Success);
        Assert.True(result.Projection.RotateGizmoVisible);
        Assert.Equal(2.5, result.Projection.RotateGizmoWorldRadius);
    }

    [Fact]
    public void Adapter_hides_rotate_gizmo_and_defaults_radius_when_not_shown()
    {
        var camera = TestCamera();
        var snapshot = new SceneRenderSnapshot(
            EntityAt(1, Vector3d.Zero), ShowRotateGizmo: false, Camera: camera);

        var result = SceneRenderProjectionAdapter.TryCreate(snapshot);

        Assert.True(result.Success);
        Assert.False(result.Projection.RotateGizmoVisible);
        Assert.Equal(RotateGizmoLayout.RingRadius, result.Projection.RotateGizmoWorldRadius);
    }
}

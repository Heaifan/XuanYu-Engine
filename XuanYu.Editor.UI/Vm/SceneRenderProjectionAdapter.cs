using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Render.Abstractions;

namespace XuanYu.Editor.UI;

public static class SceneRenderProjectionAdapter
{
    public static RenderProjectionResult TryCreate(
        SceneRenderSnapshot snapshot,
        double rotateGizmoWorldRadius = 1.2,
        double scaleGizmoWorldAxisLength = 1.2,
        Vector3d gizmoRotation = default)
    {
        if (snapshot.Camera is not { } camera)
        {
            return RenderProjectionResult.Fail("Render Projection 缺少显式 Camera。");
        }

        var selectedKey = snapshot.IsSelected ? snapshot.Entity.EntityKey : EntityId.None;
        var entities = snapshot.Entities
            .Select(e =>
            {
                var t = snapshot.TransformFor(e);
                return new RenderEntityProjection(e.EntityKey, t.Position, t.Rotation, t.Scale,
                    e.EntityKey == selectedKey);
            })
            .ToArray();
        var projection = new RenderProjection(
            new RenderCameraProjection(
                camera.Position, camera.Forward, camera.Up,
                camera.VerticalFovDegrees, camera.NearPlane,
                camera.FarPlane, camera.Revision),
            entities,
            snapshot.ShowMoveGizmo,
            snapshot.RenderPosition,
            RotateGizmoVisible: snapshot.ShowRotateGizmo,
            RotateGizmoWorldRadius: rotateGizmoWorldRadius,
            ScaleGizmoVisible: snapshot.ShowScaleGizmo,
            ScaleGizmoWorldRadius: scaleGizmoWorldAxisLength,
            GizmoRotation: gizmoRotation);
        return RenderProjectionResult.Ok(projection);
    }
}

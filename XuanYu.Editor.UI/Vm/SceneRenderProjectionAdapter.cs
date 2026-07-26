using XuanYu.Core.Scene;
using XuanYu.Render.Abstractions;

namespace XuanYu.Editor.UI;

public static class SceneRenderProjectionAdapter
{
    public static RenderProjectionResult TryCreate(SceneRenderSnapshot snapshot)
    {
        if (snapshot.Camera is not { } camera)
        {
            return RenderProjectionResult.Fail("Render Projection 缺少显式 Camera。");
        }

        var entities = snapshot.Entities
            .Select(e => new RenderEntityProjection(e.EntityKey, snapshot.PositionFor(e)))
            .ToArray();
        var projection = new RenderProjection(
            new RenderCameraProjection(
                camera.Position, camera.Forward, camera.Up,
                camera.VerticalFovDegrees, camera.NearPlane,
                camera.FarPlane, camera.Revision),
            entities,
            snapshot.ShowMoveGizmo,
            snapshot.RenderPosition,
            RotateGizmoVisible: snapshot.ShowRotateGizmo);
        return RenderProjectionResult.Ok(projection);
    }
}

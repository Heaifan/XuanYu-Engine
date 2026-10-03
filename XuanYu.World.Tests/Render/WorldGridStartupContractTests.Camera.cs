using XuanYu.Core.Space;
using XuanYu.Render.Abstractions;

namespace XuanYu.World.Tests.Render;

static class CameraProjectionContract
{
    public static RenderCameraProjection ToRenderProjection(this CameraState camera) =>
        new(camera.Position, camera.Forward, camera.Up, camera.VerticalFovDegrees,
            camera.NearPlane, camera.FarPlane, camera.Revision, camera.Mode,
            camera.OrthographicScale);
}

using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Render.Abstractions;

public readonly record struct RenderCameraProjection(
    Vector3d Position,
    Vector3d Forward,
    Vector3d Up,
    double VerticalFovDegrees,
    double NearPlane,
    double FarPlane,
    long Revision)
{
    public ViewProjectionState ToViewProjection(ViewportState viewport)
    {
        var camera = new CameraState(
            Position, Forward, Up,
            VerticalFovDegrees, NearPlane, FarPlane,
            Revision);
        return ViewProjectionState.Create(camera, viewport);
    }
}

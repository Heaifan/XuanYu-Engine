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
    long Revision,
    ProjectionMode Mode = ProjectionMode.Perspective,
    double OrthographicScale = 0.0)
{
    public ViewProjectionState ToViewProjection(ViewportState viewport)
    {
        var camera = new CameraState(
            Position, Forward, Up,
            VerticalFovDegrees, NearPlane, FarPlane,
            Revision, Mode, OrthographicScale);
        return ViewProjectionState.Create(camera, viewport);
    }

    // F3-F1：导航 Gizmo Overlay Pass 需要相机 Right（Up/Forward 合同同 CameraState）。
    public Vector3d Right => Forward.Cross(Up).Normalize();
}

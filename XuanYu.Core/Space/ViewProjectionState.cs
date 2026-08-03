using System.Numerics;
using XuanYu.Core.Gizmo;
using XuanYu.Core.Math;

namespace XuanYu.Core.Space;

public sealed class ViewProjectionState
{
    ViewProjectionState(
        CameraState camera,
        ViewportState viewport,
        Matrix4x4 view,
        Matrix4x4 projection,
        Matrix4x4 inverse)
    {
        Camera = camera;
        Viewport = viewport;
        View = view;
        Projection = projection;
        ViewProjection = view * projection;
        InverseViewProjection = inverse;
    }

    public CameraState Camera { get; }

    public ViewportState Viewport { get; }

    public Matrix4x4 View { get; }

    public Matrix4x4 Projection { get; }

    public Matrix4x4 ViewProjection { get; }

    public Matrix4x4 InverseViewProjection { get; }

    public static ViewProjectionState Create(CameraState camera, ViewportState viewport)
    {
        var eye = ToVector3(camera.Position);
        var target = ToVector3(camera.Position + camera.Forward);
        var up = ToVector3(camera.Up);
        var view = Matrix4x4.CreateLookAt(eye, target, up);
        var aspect = (float)(viewport.LogicalWidth / viewport.LogicalHeight);
        var projection = camera.Mode == ProjectionMode.Orthographic
            ? Matrix4x4.CreateOrthographic(
                (float)camera.OrthographicScale * aspect,
                (float)camera.OrthographicScale,
                (float)camera.NearPlane,
                (float)camera.FarPlane)
            : Matrix4x4.CreatePerspectiveFieldOfView(
                (float)(camera.VerticalFovDegrees * global::System.Math.PI / 180.0),
                aspect,
                (float)camera.NearPlane,
                (float)camera.FarPlane);
        var viewProjection = view * projection;

        if (!Matrix4x4.Invert(viewProjection, out var inverse))
        {
            throw new InvalidOperationException("ViewProjection 矩阵不可逆。");
        }

        return new ViewProjectionState(camera, viewport, view, projection, inverse);
    }

    public Vector3d TransformPointToWorld(double ndcX, double ndcY, double ndcZ)
    {
        if (!double.IsFinite(ndcX) || !double.IsFinite(ndcY) || !double.IsFinite(ndcZ))
        {
            throw new ArgumentOutOfRangeException(nameof(ndcX));
        }

        var clip = new Vector4((float)ndcX, (float)ndcY, (float)ndcZ, 1.0f);
        var world = Vector4.Transform(clip, InverseViewProjection);
        if (world.W == 0.0f) throw new InvalidOperationException("World 坐标 W 为 0。");

        return new Vector3d(world.X / world.W, world.Y / world.W, world.Z / world.W);
    }

    public ScreenPoint ProjectWorldPoint(Vector3d point)
    {
        var clip = Vector4.Transform(new Vector4(ToVector3(point), 1), ViewProjection);
        if (!float.IsFinite(clip.W) || clip.W <= 0) throw new InvalidOperationException("世界点位于相机后方。");
        var ndcX = clip.X / clip.W;
        var ndcY = clip.Y / clip.W;
        return new ScreenPoint(
            Viewport.LogicalX + ((ndcX + 1.0) * 0.5 * Viewport.LogicalWidth),
            Viewport.LogicalY + ((1.0 - ndcY) * 0.5 * Viewport.LogicalHeight));
    }

    static Vector3 ToVector3(Vector3d vector)
    {
        return new Vector3((float)vector.X, (float)vector.Y, (float)vector.Z);
    }
}

using System.Numerics;
using XuanYu.Core.Gizmo;
using XuanYu.Core.Math;

namespace XuanYu.Core.Space;

public sealed partial class ViewProjectionState
{
    public ScreenPoint ProjectWorldPoint(Vector3d point)
    {
        if (!TryProjectWorldPoint(point, out var screen))
            throw new InvalidOperationException("世界点位于相机后方。");
        return screen;
    }

    public bool TryProjectWorldPoint(Vector3d point, out ScreenPoint screen)
    {
        if (!RequiresDoublePrecision(point))
        {
            var clip = Vector4.Transform(new Vector4(ToVector3(point), 1), ViewProjection);
            if (!float.IsFinite(clip.W) || clip.W <= 0)
            {
                screen = default;
                return false;
            }

            var legacyNdcX = clip.X / clip.W;
            var legacyNdcY = clip.Y / clip.W;
            if (!float.IsFinite(legacyNdcX) || !float.IsFinite(legacyNdcY))
            {
                screen = default;
                return false;
            }

            screen = ToScreenPoint(legacyNdcX, legacyNdcY);
            return true;
        }

        var offset = point - Camera.Position;
        var depth = offset.Dot(Camera.Forward);
        if (!double.IsFinite(depth) || depth <= 0.0)
        {
            screen = default;
            return false;
        }

        var horizontal = offset.Dot(Camera.Right);
        var vertical = offset.Dot(Camera.Up);
        var aspect = Viewport.LogicalWidth / Viewport.LogicalHeight;
        double ndcX;
        double ndcY;
        if (Camera.Mode == ProjectionMode.Orthographic)
        {
            var halfHeight = Camera.OrthographicScale * 0.5;
            ndcX = horizontal / (halfHeight * aspect);
            ndcY = vertical / halfHeight;
        }
        else
        {
            var tangent = global::System.Math.Tan(Camera.VerticalFovDegrees * global::System.Math.PI / 360.0);
            ndcX = horizontal / (depth * tangent * aspect);
            ndcY = vertical / (depth * tangent);
        }

        if (!double.IsFinite(ndcX) || !double.IsFinite(ndcY))
        {
            screen = default;
            return false;
        }
        screen = ToScreenPoint(ndcX, ndcY);
        return true;
    }

    ScreenPoint ToScreenPoint(double ndcX, double ndcY) => new(
        Viewport.LogicalX + ((ndcX + 1.0) * 0.5 * Viewport.LogicalWidth),
        Viewport.LogicalY + ((1.0 - ndcY) * 0.5 * Viewport.LogicalHeight));

    bool RequiresDoublePrecision(Vector3d point) =>
        global::System.Math.Max(
            global::System.Math.Max(global::System.Math.Abs(point.X), global::System.Math.Abs(point.Y)),
            global::System.Math.Max(global::System.Math.Abs(point.Z), Camera.FarPlane)) >= 10_000.0;
}

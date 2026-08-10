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
        screen = new ScreenPoint(
            Viewport.LogicalX + ((ndcX + 1.0) * 0.5 * Viewport.LogicalWidth),
            Viewport.LogicalY + ((1.0 - ndcY) * 0.5 * Viewport.LogicalHeight));
        return true;
    }
}

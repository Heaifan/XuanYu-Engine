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
        var clip = Vector4.Transform(new Vector4(ToVector3(point), 1), ViewProjection);
        if (!float.IsFinite(clip.W) || clip.W <= 0)
        {
            screen = default;
            return false;
        }
        var ndcX = clip.X / clip.W;
        var ndcY = clip.Y / clip.W;
        if (!float.IsFinite(ndcX) || !float.IsFinite(ndcY))
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

using XuanYu.Core.Gizmo;

namespace XuanYu.Editor.MapEditing;

public static class RegionEdgeSnapGeometry
{
    public static bool TryClosestPoint(
        ScreenPoint point,
        ScreenPoint start,
        ScreenPoint end,
        out ScreenPoint closest,
        out double parameter)
    {
        closest = default;
        parameter = 0;
        var dx = end.X - start.X;
        var dy = end.Y - start.Y;
        var lengthSquared = dx * dx + dy * dy;
        if (!double.IsFinite(lengthSquared) || lengthSquared <= double.Epsilon)
            return false;
        parameter = Math.Clamp(((point.X - start.X) * dx + (point.Y - start.Y) * dy) / lengthSquared, 0, 1);
        closest = new(start.X + dx * parameter, start.Y + dy * parameter);
        return double.IsFinite(closest.X) && double.IsFinite(closest.Y);
    }
}

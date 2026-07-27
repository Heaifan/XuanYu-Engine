using XuanYu.Core.Gizmo;
using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Core.Tests.Gizmo;

public sealed partial class ScaleGizmoTests
{
    static ((ScaleGizmoDrag drag, ScreenPoint handlePoint, ScreenPoint axisDir) result,
        ScaleGizmoLayout layout) XDrag(ScaleGizmoHandle handle, Vector3d startScale)
    {
        var layout = Layout(Vector3d.Zero);
        ScreenPoint handlePoint;
        ScreenPoint axisDir;
        if (handle == ScaleGizmoHandle.Uniform)
        {
            handlePoint = layout.Center;
            axisDir = default;
        }
        else
        {
            var i = handle == ScaleGizmoHandle.X ? 0 : (handle == ScaleGizmoHandle.Y ? 1 : 2);
            handlePoint = layout.AxisEnd[i];
            var dx = layout.AxisEnd[i].X - layout.Center.X;
            var dy = layout.AxisEnd[i].Y - layout.Center.Y;
            var len = System.Math.Sqrt(dx * dx + dy * dy);
            axisDir = new ScreenPoint(dx / len, dy / len);
        }
        var drag = new ScaleGizmoDrag(handle, startScale, handlePoint.X, handlePoint.Y, axisDir);
        return ((drag, handlePoint, axisDir), layout);
    }

    static ScreenPoint Pull(ScreenPoint from, ScreenPoint dir, double d)
        => new(from.X + dir.X * d, from.Y + dir.Y * d);
}

using XuanYu.Engine.Core.Math;

namespace XuanYu.Engine.Editor.Transform.Translation.Axis;

/// <summary>
/// 轴向平移求解器。X/Y/Z 共用此 Solver，仅 Axis 不同。
/// ScreenProjection：将鼠标屏幕位移映射到世界轴位移。
/// DragPlane：从射线-约束平面交点投影到轴。
/// </summary>
public static class AxisTranslationSolver
{
    /// <summary>ScreenProjection：从鼠标总位移计算目标位置。</summary>
    public static Vector3d Solve(AxisTranslationAnchor anchor, double pointerX, double pointerY)
    {
        if (anchor.Mode == AxisTranslationMode.Disabled)
            return anchor.InitialPosition;
        if (anchor.Mode == AxisTranslationMode.ScreenProjection)
            return SolveScreenProj(anchor, pointerX, pointerY);
        return anchor.InitialPosition;
    }

    /// <summary>DragPlane：从当前射线-平面交点计算目标位置。</summary>
    public static Vector3d SolveDragPlane(AxisTranslationAnchor anchor, Vector3d currentIntersection)
    {
        if (anchor.Mode != AxisTranslationMode.DragPlane)
            return anchor.InitialPosition;
        var delta = currentIntersection - anchor.StartIntersection;
        var axisDelta = delta.Dot(anchor.Axis);
        return anchor.InitialPosition + anchor.Axis * axisDelta;
    }

    static Vector3d SolveScreenProj(AxisTranslationAnchor a, double px, double py)
    {
        if (!a.IsValid) return a.InitialPosition;
        var dx = px - a.StartPointerX;
        var dy = py - a.StartPointerY;
        var pixelDist = dx * a.ScreenDirection.X + dy * a.ScreenDirection.Y;
        var worldDist = pixelDist / a.PixelsPerWorldUnit;
        return a.InitialPosition + a.Axis * worldDist;
    }
}

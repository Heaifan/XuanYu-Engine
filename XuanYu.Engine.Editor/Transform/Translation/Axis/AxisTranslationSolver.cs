using XuanYu.Engine.Core.Math;

namespace FluidWarfare.Editor.Transform.Translation.Axis;

/// <summary>
/// 轴向平移求解器。X/Y/Z 共用此 Solver，仅 Axis 不同。
/// 使用屏幕投影算法：将鼠标在屏幕上的总位移映射到世界轴位移。
/// 不使用上一帧鼠标位置，不使用逐帧累计。
/// </summary>
public static class AxisTranslationSolver
{
    /// <summary>
    /// 从鼠标总位移计算目标位置。
    /// target = InitialPosition + Axis × worldDistance
    /// worldDistance = Dot(currentPointer - startPointer, screenDirection) / pixelsPerWorldUnit
    /// </summary>
    public static Vector3d Solve(AxisTranslationAnchor anchor, double pointerX, double pointerY)
    {
        if (!anchor.IsValid || anchor.Mode == AxisTranslationMode.Disabled)
            return anchor.InitialPosition;

        var dx = pointerX - anchor.StartPointerX;
        var dy = pointerY - anchor.StartPointerY;
        var pixelDist = dx * anchor.ScreenDirection.X + dy * anchor.ScreenDirection.Y;
        var worldDist = pixelDist / anchor.PixelsPerWorldUnit;
        return anchor.InitialPosition + anchor.Axis * worldDist;
    }
}

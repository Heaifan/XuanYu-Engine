using XuanYu.Engine.Core.Math;
using FluidWarfare.Editor.Transform.Translation.Constraint;

namespace FluidWarfare.Editor.Transform.Translation.Axis;

/// <summary>
/// 从相机快照和实体位置创建 AxisTranslationAnchor 的辅助方法。
/// 负责投影、退化检测和回退灵敏度计算。
/// </summary>
public static class AxisTranslationStart
{
    private const double FallbackSampleLength = 10.0;

    public static AxisTranslationAnchor? TryCreateAnchor(
        Vector3d pivot,
        Vector3d axis,
        double pointerX,
        double pointerY,
        Vector3d initialPosition,
        float[] viewProjection,
        int viewportWidth,
        int viewportHeight,
        double cameraDistance,
        double fieldOfViewDegrees,
        bool isOrthographic,
        double orthographicHeight)
    {
        if (AxisScreenMetric.TryCompute(pivot, axis, viewProjection,
                viewportWidth, viewportHeight,
                out var dir, out var ppu))
        {
            return new AxisTranslationAnchor(
                initialPosition, axis, pivot, ppu, dir,
                pointerX, pointerY, AxisTranslationMode.ScreenProjection);
        }

        // 轴正对相机：使用屏幕垂直方向 + 深度灵敏度
        var vh = Math.Max(1, viewportHeight);
        var fallbackPpu = isOrthographic
            ? orthographicHeight / vh
            : 2.0 * cameraDistance * Math.Tan(fieldOfViewDegrees * Math.PI / 360.0) / vh;
        if (fallbackPpu <= 0) return null;

        return new AxisTranslationAnchor(
            initialPosition, axis, pivot,
            1.0 / (fallbackPpu * FallbackSampleLength),
            new Vector2d(0, -1),
            pointerX, pointerY, AxisTranslationMode.ScreenProjection);
    }
}

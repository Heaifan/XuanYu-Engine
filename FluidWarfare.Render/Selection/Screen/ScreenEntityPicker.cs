using FluidWarfare.Core.Math;
using FluidWarfare.Render.Selection.Presented;

namespace FluidWarfare.Render.Selection.Screen;

/// <summary>
/// 精确 Ray-AABB Miss 后的屏幕空间容错 Picking。
/// 将 PresentedEntityBounds 投影到屏幕，5px 容差，选 ViewDepth 最近者。
/// 零分配（不含投影内部临时变量）。
/// </summary>
public static class ScreenEntityPicker
{
    /// <summary>命中候选。</summary>
    public readonly record struct Candidate(int EntityId, float ViewDepth);

    /// <summary>
    /// 在精确 Picking 无命中时，用屏幕空间容差查找最近的实体。
    /// </summary>
    public static Candidate? Pick(
        float pixelX, float pixelY,
        float[] viewProjection, int viewportWidth, int viewportHeight,
        ReadOnlySpan<PresentedEntityBounds> entities,
        int tolerancePx = ScreenPickTolerance.DefaultPixels)
    {
        Candidate? best = null;

        foreach (ref readonly var e in entities)
        {
            if (!ScreenBoundsProjection.TryProject(
                    e.Bounds, viewProjection, viewportWidth, viewportHeight,
                    out var minX, out var minY, out var maxX, out var maxY))
                continue;

            if (pixelX >= minX - tolerancePx && pixelX <= maxX + tolerancePx &&
                pixelY >= minY - tolerancePx && pixelY <= maxY + tolerancePx)
            {
                if (best is null || e.ViewDepth < best.Value.ViewDepth)
                    best = new Candidate(e.EntityId, e.ViewDepth);
            }
        }

        return best;
    }
}

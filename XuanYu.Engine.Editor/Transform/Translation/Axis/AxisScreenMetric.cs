using XuanYu.Engine.Core.Math;

namespace FluidWarfare.Editor.Transform.Translation.Axis;

/// <summary>
/// 计算轴在屏幕上的投影方向和每像素对应世界单位数。
/// 将 Pivot 和 Pivot + Axis × SampleLength 投影到屏幕。
/// 当轴投影小于 6px 时标记为退化。
/// </summary>
public static class AxisScreenMetric
{
    private const double MinProjectedPixels = 6.0;
    private const double SampleLength = 10.0;

    public static bool TryCompute(
        Vector3d pivot, Vector3d axis,
        float[] viewProjection, int width, int height,
        out Vector2d screenDirection, out double pixelsPerWorldUnit)
    {
        screenDirection = default;
        pixelsPerWorldUnit = 0;

        if (!TryProject(pivot, viewProjection, width, height, out var pp))
            return false;
        if (!TryProject(pivot + axis * SampleLength, viewProjection, width, height, out var ap))
            return false;

        var dx = ap.X - pp.X;
        var dy = ap.Y - pp.Y;
        var len = Math.Sqrt(dx * dx + dy * dy);
        if (len < MinProjectedPixels) return false;

        screenDirection = new Vector2d(dx / len, dy / len);
        pixelsPerWorldUnit = len / SampleLength;
        return true;
    }

    private static bool TryProject(Vector3d world, float[] vp, int w, int h, out Vector2d pixel)
    {
        pixel = default;
        if (w <= 0 || h <= 0 || vp is not { Length: 16 }) return false;
        var cw = vp[3] * world.X + vp[7] * world.Y + vp[11] * world.Z + vp[15];
        if (!double.IsFinite(cw) || Math.Abs(cw) < 1e-6) return false;
        var nx = (vp[0] * world.X + vp[4] * world.Y + vp[8] * world.Z + vp[12]) / cw;
        var ny = (vp[1] * world.X + vp[5] * world.Y + vp[9] * world.Z + vp[13]) / cw;
        if (!double.IsFinite(nx) || !double.IsFinite(ny)) return false;
        pixel = new Vector2d((nx * 0.5 + 0.5) * w, (ny * 0.5 + 0.5) * h);
        return true;
    }
}

public readonly record struct Vector2d(double X, double Y);

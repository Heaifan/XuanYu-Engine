using XuanYu.Engine.Core.Math;

namespace FluidWarfare.Render.Selection.Screen;

/// <summary>
/// 将世界空间 AABB 投影到屏幕空间，获取屏幕矩形。
/// 零分配分段投影（无 LINQ 无临时 List）。
/// </summary>
public static class ScreenBoundsProjection
{
    public static bool TryProject(
        SceneAxisAlignedBounds bounds,
        float[] vp, int vw, int vh,
        out float minX, out float minY,
        out float maxX, out float maxY)
    {
        minX = float.MaxValue; minY = float.MaxValue;
        maxX = float.MinValue; maxY = float.MinValue;
        var valid = false;
        var corners = GetCorners(bounds);
        foreach (var c in corners)
        {
            if (!TryPoint(c.X, c.Y, c.Z, vp, vw, vh, out var sx, out var sy))
                continue;
            if (sx < minX) minX = sx; if (sy < minY) minY = sy;
            if (sx > maxX) maxX = sx; if (sy > maxY) maxY = sy;
            valid = true;
        }
        return valid;
    }

    /// <summary>单点世界→屏幕投影。</summary>
    public static bool TryPoint(
        double wx, double wy, double wz,
        float[] vp, int vw, int vh,
        out float sx, out float sy)
    {
        sx = sy = 0;
        var cw = vp[3] * wx + vp[7] * wy + vp[11] * wz + vp[15];
        if (!double.IsFinite(cw) || Math.Abs(cw) < 1e-6) return false;
        var nx = (vp[0] * wx + vp[4] * wy + vp[8] * wz + vp[12]) / cw;
        var ny = (vp[1] * wx + vp[5] * wy + vp[9] * wz + vp[13]) / cw;
        if (!double.IsFinite(nx) || !double.IsFinite(ny)) return false;
        sx = (float)((nx * 0.5 + 0.5) * vw);
        sy = (float)((ny * 0.5 + 0.5) * vh);
        return true;
    }

    private static Vector3d[] GetCorners(SceneAxisAlignedBounds b)
    {
        var mn = b.Minimum;
        var mx = b.Maximum;
        return
        [
            new(mn.X, mn.Y, mn.Z), new(mx.X, mn.Y, mn.Z),
            new(mn.X, mx.Y, mn.Z), new(mn.X, mn.Y, mx.Z),
            new(mx.X, mx.Y, mn.Z), new(mx.X, mn.Y, mx.Z),
            new(mn.X, mx.Y, mx.Z), new(mx.X, mx.Y, mx.Z),
        ];
    }
}

using XuanYu.Engine.Core.Math;

namespace XuanYu.Engine.Render.Coordinates;

/// <summary>
/// 旧 Y-Up 到 Z-Up 的一次性迁移工具。
/// 遵守映射：new = (oldX, -oldZ, oldY)
/// 只应在迁移阶段和迁移测试中使用，不得进入每帧渲染路径。
/// </summary>
public static class YUpToZUpPosition
{
    /// <summary>
    /// 将 Y-Up 坐标转换为 Z-Up 坐标。
    /// 转换：newX = oldX, newY = -oldZ, newZ = oldY
    /// </summary>
    public static (double X, double Y, double Z) Convert(double oldX, double oldY, double oldZ)
    {
        return (oldX, -oldZ, oldY);
    }

    /// <summary>
    /// 将 Y-Up Vector3d 转换为 Z-Up Vector3d。
    /// </summary>
    public static Vector3d Convert(Vector3d oldPosition)
    {
        var (nx, ny, nz) = Convert(oldPosition.X, oldPosition.Y, oldPosition.Z);
        return new Vector3d(nx, ny, nz);
    }

    /// <summary>
    /// 将 Z-Up 坐标逆转换回 Y-Up 坐标。
    /// 仅用于验证和测试。
    /// </summary>
    public static (double X, double Y, double Z) Inverse(double newX, double newY, double newZ)
    {
        return (newX, newZ, -newY);
    }

    /// <summary>
    /// 验证转换是否保持距离（可逆性检查）。
    /// </summary>
    public static bool VerifyRoundTrip(double x, double y, double z, double tolerance = 1e-10)
    {
        var (nx, ny, nz) = Convert(x, y, z);
        var (rx, ry, rz) = Inverse(nx, ny, nz);
        return Math.Abs(rx - x) < tolerance &&
               Math.Abs(ry - y) < tolerance &&
               Math.Abs(rz - z) < tolerance;
    }
}

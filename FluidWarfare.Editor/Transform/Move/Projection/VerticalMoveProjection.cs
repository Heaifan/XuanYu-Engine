using FluidWarfare.Core.Math;

namespace FluidWarfare.Editor.Transform.Move.Projection;

/// <summary>
/// Z 轴垂直移动映射。不使用射线求交。
/// 使用屏幕垂直位移 × world-per-pixel 计算 Z 增量。
/// </summary>
public static class VerticalMoveProjection
{
    /// <summary>
    /// 计算当前帧的 world-per-pixel 值。
    /// EditorShell 传入相机参数（无需 Render 项目引用）。
    /// </summary>
    public static double ComputeWorldPerPixel(
        bool isOrthographic,
        double orthographicHeight,
        double cameraDistance,
        double fieldOfViewDegrees,
        int viewportHeight)
    {
        var vh = Math.Max(1, viewportHeight);
        if (isOrthographic)
            return orthographicHeight / vh;

        var fovRad = fieldOfViewDegrees * Math.PI / 180.0;
        return 2.0 * cameraDistance * Math.Tan(fovRad * 0.5) / vh;
    }

    /// <summary>
    /// Z 轴垂直移动映射。累加式：
    ///   NewZ = CurrentZ + (-deltaPixelY * worldPerPixel)
    /// </summary>
    public static Vector3d ApplyToPosition(
        Vector3d currentPosition,
        Vector3d entityPosition,
        double deltaPixelY,
        double worldPerPixel)
    {
        var dz = -deltaPixelY * worldPerPixel;
        return new Vector3d(entityPosition.X, entityPosition.Y, currentPosition.Z + dz);
    }
}

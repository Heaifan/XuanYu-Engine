namespace FluidWarfare.Editor.Transform.Move.Projection;

/// <summary>
/// 实体移动安全阈值。
/// 当 PlaneIntersection 结果超出这些限制时，应拒绝该交点
/// 并切换到 ScreenDeltaFallback 模式。
/// </summary>
public static class EntityMoveSafetyLimits
{
    /// <summary>
    /// 射线方向与平面法线的最小点积绝对值。
    /// 低于此值视为射线近似平行于平面，PlaneIntersection 不可靠。
    /// </summary>
    public const double MinPlaneNormalDot = 0.05;

    /// <summary>
    /// 从相机到交点的最大允许距离倍数（相对于相机到初始交点的距离）。
    /// 防止异常放大导致实体飞到远处。
    /// </summary>
    public const double MaxIntersectionDistanceFactor = 20.0;

    /// <summary>
    /// 单帧预览最大允许世界位移倍数（相对于预期位移）。
    /// 预期位移 = worldPerPixel × |pixelDelta|。
    /// 实际位移超过此倍数时拒绝 PlaneIntersection 结果。
    /// </summary>
    public const double MaxWorldDeltaMultiplier = 4.0;
}

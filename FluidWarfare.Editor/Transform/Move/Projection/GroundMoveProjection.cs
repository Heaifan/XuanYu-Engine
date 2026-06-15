using FluidWarfare.Core.Math;

namespace FluidWarfare.Editor.Transform.Move.Projection;

/// <summary>
/// 地面移动映射。所有平面求交模式使用相对差值。
/// 当 PlaneIntersection 不稳定时切换到 ScreenDeltaFallback。
/// </summary>
public enum MoveMappingMode
{
    /// <summary>射线与平面求交，使用相对差值。</summary>
    PlaneIntersection,

    /// <summary>屏幕像素增量映射到地面，用于低俯角或异常情况。</summary>
    ScreenDeltaFallback,
}

/// <summary>
/// 地面移动投射计算。核心公式：
///   TargetPosition = InitialEntityPosition + (CurrentPlaneHit - InitialPlaneHit)
/// </summary>
public static class GroundMoveProjection
{
    /// <summary>
    /// 平面求交模式映射。以锚点为基准计算相对位移。
    /// </summary>
    public static Vector3d Map(
        GroundMoveAnchor anchor,
        Vector3d currentPlaneHit,
        EntityMoveAxis axis)
    {
        var delta = currentPlaneHit - anchor.InitialPlaneHit;
        var worldDelta = axis switch
        {
            EntityMoveAxis.GroundPlane => new Vector3d(delta.X, delta.Y, 0),
            EntityMoveAxis.X => new Vector3d(delta.X, 0, 0),
            EntityMoveAxis.Y => new Vector3d(0, delta.Y, 0),
            EntityMoveAxis.Z => new Vector3d(0, 0, 0),
            _ => new Vector3d(delta.X, delta.Y, 0),
        };
        return anchor.InitialEntityPosition + worldDelta;
    }

    /// <summary>
    /// 屏幕增量回退模式映射。使用相机地面方向的像素增量。
    /// </summary>
    public static Vector3d MapScreenDelta(
        GroundMoveAnchor anchor,
        Vector3d cameraRight,
        Vector3d cameraForward,
        double deltaPixelX,
        double deltaPixelY,
        double worldPerPixel,
        EntityMoveAxis axis)
    {
        // 相机方向在 XY 平面的投影
        var rightLen = Math.Sqrt(cameraRight.X * cameraRight.X + cameraRight.Y * cameraRight.Y);
        var fwdLen = Math.Sqrt(cameraForward.X * cameraForward.X + cameraForward.Y * cameraForward.Y);
        if (rightLen < 1e-10 || fwdLen < 1e-10)
            return anchor.InitialEntityPosition;

        var groundRight = new Vector3d(cameraRight.X / rightLen, cameraRight.Y / rightLen, 0);
        var groundForward = new Vector3d(cameraForward.X / fwdLen, cameraForward.Y / fwdLen, 0);

        var worldDelta = new Vector3d(
            groundRight.X * deltaPixelX * worldPerPixel + groundForward.X * -deltaPixelY * worldPerPixel,
            groundRight.Y * deltaPixelX * worldPerPixel + groundForward.Y * -deltaPixelY * worldPerPixel,
            0);

        var constrained = axis switch
        {
            EntityMoveAxis.GroundPlane => worldDelta,
            EntityMoveAxis.X => new Vector3d(worldDelta.X, 0, 0),
            EntityMoveAxis.Y => new Vector3d(0, worldDelta.Y, 0),
            EntityMoveAxis.Z => Vector3d.Zero,
            _ => worldDelta,
        };

        return anchor.InitialEntityPosition + constrained;
    }

    /// <summary>
    /// 判断 PlaneIntersection 是否可靠。
    /// </summary>
    public static bool IsPlaneIntersectionReliable(
        Vector3d rayDirection,
        Vector3d planeNormal,
        double t,
        Vector3d worldPosition,
        double expectedWorldPerPixelDelta,
        double initialDistance)
    {
        if (!double.IsFinite(t) || t <= 0) return false;

        var dot = Math.Abs(rayDirection.X * planeNormal.X + rayDirection.Y * planeNormal.Y + rayDirection.Z * planeNormal.Z);
        if (dot < EntityMoveSafetyLimits.MinPlaneNormalDot) return false;

        if (!double.IsFinite(worldPosition.X) || !double.IsFinite(worldPosition.Y) || !double.IsFinite(worldPosition.Z))
            return false;

        // 交点过远
        if (initialDistance > 0 && t > initialDistance * EntityMoveSafetyLimits.MaxIntersectionDistanceFactor)
            return false;

        // 单帧位移异常放大
        if (expectedWorldPerPixelDelta > 0)
        {
            var actualDelta = Math.Sqrt(
                worldPosition.X * worldPosition.X +
                worldPosition.Y * worldPosition.Y +
                worldPosition.Z * worldPosition.Z);
            if (actualDelta > expectedWorldPerPixelDelta * EntityMoveSafetyLimits.MaxWorldDeltaMultiplier)
                return false;
        }

        return true;
    }
}

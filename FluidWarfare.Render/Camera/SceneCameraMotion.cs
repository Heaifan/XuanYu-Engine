namespace FluidWarfare.Render.Camera;

/// <summary>
/// RTS 相机运动计算。
/// 纯数学方法，不依赖 Vulkan、Avalonia、Win32 或 Editor。
/// </summary>
public static class SceneCameraMotion
{
    /// <summary>
    /// 沿 XZ 地面平移相机 Target。
    /// </summary>
    /// <param name="state">当前相机状态。</param>
    /// <param name="deltaPixelX">鼠标水平移动像素增量（正 = 右）。</param>
    /// <param name="deltaPixelY">鼠标垂直移动像素增量（正 = 下）。</param>
    /// <param name="viewportHeight">视口高度（像素）。</param>
    /// <returns>平移后的新相机状态。</returns>
    public static SceneCameraState Pan(
        SceneCameraState state,
        float deltaPixelX, float deltaPixelY,
        float viewportHeight)
    {
        if (viewportHeight <= 0) return state;

        // 将像素增量转换为世界单位增量
        // worldUnitsPerPixel = 2 × distance × tan(FOV/2) / viewportHeight
        var fovRad = state.FieldOfViewDegrees * MathF.PI / 180f;
        var worldPerPixel = 2.0f * state.Distance * MathF.Tan(fovRad * 0.5f) / viewportHeight;

        // 水平方向：沿 X 轴移动
        // 垂直 (屏幕Y) 方向：沿 Z 轴移动（俯视视角下屏幕上下 = 世界 Z）
        // 鼠标向右拖 → Target 向右移；鼠标向上拖 → Target 向上移（在 XZ 平面上）
        // 即 Target 移动方向与画面拖动方向相反（"抓住地图"交互）
        var deltaWorldX = -deltaPixelX * worldPerPixel;
        var deltaWorldZ = deltaPixelY * worldPerPixel;  // 屏幕 Y 正向（向下）→ 世界 Z 正向

        var newTargetX = Math.Clamp(
            state.TargetX + deltaWorldX,
            SceneCameraLimits.MinTargetX,
            SceneCameraLimits.MaxTargetX);
        var newTargetZ = Math.Clamp(
            state.TargetZ + deltaWorldZ,
            SceneCameraLimits.MinTargetZ,
            SceneCameraLimits.MaxTargetZ);

        return state with
        {
            TargetX = newTargetX,
            TargetZ = newTargetZ
        };
    }

    /// <summary>
    /// 缩放：使用指数缩放改变相机距离。
    /// </summary>
    /// <param name="state">当前相机状态。</param>
    /// <param name="wheelNotches">滚轮格数（向上=正，向下=负）。</param>
    /// <returns>缩放后的新相机状态。</returns>
    public static SceneCameraState Zoom(SceneCameraState state, float wheelNotches)
    {
        var newDistance = Math.Clamp(
            state.Distance * MathF.Pow(0.9f, wheelNotches),
            SceneCameraLimits.MinDistance,
            SceneCameraLimits.MaxDistance);

        return state with { Distance = newDistance };
    }

    /// <summary>
    /// 重置相机到默认状态。
    /// </summary>
    public static SceneCameraState Reset()
    {
        return SceneCameraDefaults.CreateDefault();
    }

    /// <summary>
    /// 判断两个相机状态是否在 Target 上不同。
    /// </summary>
    public static bool TargetChanged(SceneCameraState a, SceneCameraState b)
    {
        return a.TargetX != b.TargetX || a.TargetZ != b.TargetZ;
    }

    /// <summary>
    /// 判断两个相机状态是否在 Distance 上不同。
    /// </summary>
    public static bool DistanceChanged(SceneCameraState a, SceneCameraState b)
    {
        return a.Distance != b.Distance;
    }
}

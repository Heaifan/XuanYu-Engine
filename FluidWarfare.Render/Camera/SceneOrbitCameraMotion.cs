namespace FluidWarfare.Render.Camera;

/// <summary>
/// Blender 风格轨道相机运动计算。
/// 纯数学方法，不依赖 Vulkan/Avalonia/Win32/Editor。
/// </summary>
public static class SceneOrbitCameraMotion
{
    private const float MinPitch = 5f;
    private const float MaxPitch = 89f;
    private const float MinDistance = 2f;
    private const float MaxDistance = 200f;
    private const float MinPivot = -500f;
    private const float MaxPivot = 500f;

    /// <summary>
    /// 创建默认轨道相机状态（从右上俯视原点）。
    /// </summary>
    public static SceneOrbitCameraState CreateDefault()
    {
        return new SceneOrbitCameraState
        {
            PivotX = 0, PivotY = 0, PivotZ = 0,
            Yaw = 135f,    // 看向 -Z 方向偏 135° = 从 +X/+Z 象限看
            Pitch = 45f,   // 45° 俯角
            Distance = 40f,
            FieldOfViewDegrees = 55f,
            NearPlane = 0.1f,
            FarPlane = 1000f
        };
    }

    /// <summary>
    /// 绕 Pivot 旋转（中键拖动）。
    /// </summary>
    public static SceneOrbitCameraState Orbit(
        SceneOrbitCameraState state,
        float deltaYaw,
        float deltaPitch)
    {
        var newYaw = state.Yaw + deltaYaw;
        // Yaw 可循环
        if (newYaw > 360f) newYaw -= 360f;
        if (newYaw < 0f) newYaw += 360f;

        var newPitch = Math.Clamp(
            state.Pitch + deltaPitch, MinPitch, MaxPitch);

        return state with { Yaw = newYaw, Pitch = newPitch };
    }

    /// <summary>
    /// 沿相机视平面平移 Pivot（Shift+中键拖动）。
    /// </summary>
    /// <param name="state">当前相机状态。</param>
    /// <param name="deltaX">水平像素增量。</param>
    /// <param name="deltaY">垂直像素增量。</param>
    /// <param name="viewportHeight">视口高度。</param>
    /// <returns>平移后的新状态。</returns>
    public static SceneOrbitCameraState Pan(
        SceneOrbitCameraState state,
        float deltaX, float deltaY,
        float viewportHeight)
    {
        if (viewportHeight <= 0) return state;

        var fovRad = state.FieldOfViewDegrees * MathF.PI / 180f;
        var worldPerPixel = 2.0f * state.Distance * MathF.Tan(fovRad * 0.5f) / viewportHeight;
        var yawRad = state.Yaw * MathF.PI / 180f;

        // Camera right vector (in XZ plane, perpendicular to view direction)
        var rightX = MathF.Cos(yawRad);
        var rightZ = MathF.Sin(yawRad);

        // Camera up vector (in XZ plane, perpendicular to right)
        var upX = -rightZ;
        var upZ = rightX;

        // Pan: negative deltaX = move right (drag left), positive deltaY = move down (drag up)
        var moveX = (-deltaX * rightX + deltaY * upX) * worldPerPixel;
        var moveZ = (-deltaX * rightZ + deltaY * upZ) * worldPerPixel;

        return state with
        {
            PivotX = Math.Clamp(state.PivotX + moveX, MinPivot, MaxPivot),
            PivotY = state.PivotY,
            PivotZ = Math.Clamp(state.PivotZ + moveZ, MinPivot, MaxPivot)
        };
    }

    /// <summary>
    /// Dolly 推拉（Ctrl+中键垂直拖动）。
    /// </summary>
    public static SceneOrbitCameraState Dolly(
        SceneOrbitCameraState state,
        float deltaPixels)
    {
        var factor = 1f + deltaPixels * 0.005f;
        var newDistance = Math.Clamp(
            state.Distance * factor, MinDistance, MaxDistance);
        return state with { Distance = newDistance };
    }

    /// <summary>
    /// 滚轮缩放。
    /// </summary>
    public static SceneOrbitCameraState Zoom(
        SceneOrbitCameraState state,
        float wheelNotches)
    {
        var newDistance = Math.Clamp(
            state.Distance * MathF.Pow(0.9f, wheelNotches),
            MinDistance, MaxDistance);
        return state with { Distance = newDistance };
    }

    /// <summary>
    /// 聚焦到指定包围盒中心。
    /// </summary>
    /// <param name="state">当前状态。</param>
    /// <param name="centerX">包围盒中心 X。</param>
    /// <param name="centerY">包围盒中心 Y。</param>
    /// <param name="centerZ">包围盒中心 Z。</param>
    /// <param name="radius">包围盒半径（用于计算合适距离）。</param>
    /// <returns>聚焦后的新状态。</returns>
    public static SceneOrbitCameraState FrameSelected(
        SceneOrbitCameraState state,
        float centerX, float centerY, float centerZ,
        float radius)
    {
        // 距离根据包围盒半径计算，确保实体完整可见
        var fitDist = Math.Clamp(
            radius * 3.5f, MinDistance, MaxDistance);

        return state with
        {
            PivotX = centerX,
            PivotY = centerY,
            PivotZ = centerZ,
            Distance = fitDist
        };
    }

    /// <summary>
    /// 显示全部（重置到默认状态）。
    /// </summary>
    public static SceneOrbitCameraState FrameAll() => CreateDefault();
}

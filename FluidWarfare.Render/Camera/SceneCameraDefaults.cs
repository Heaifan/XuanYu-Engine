namespace FluidWarfare.Render.Camera;

/// <summary>
/// 默认 Scene3D 相机配置（与当前 VulkanCameraInfo.DefaultBattlefield 等价的 Target+Distance 表示）。
/// </summary>
public static class SceneCameraDefaults
{
    /// <summary>
    /// 默认 Target 在原点。
    /// </summary>
    public const float DefaultTargetX = 0f;
    public const float DefaultTargetZ = 0f;

    /// <summary>
    /// 默认距离（等价于 Position (0,32,24) 到目标 (0,0,0) 的 Euclidean 距离）。
    /// sqrt(32² + 24²) = 40
    /// </summary>
    public const float DefaultDistance = 40f;

    /// <summary>
    /// 默认 FOV。
    /// </summary>
    public const float DefaultFov = 55f;

    /// <summary>
    /// 默认近/远裁剪面。
    /// </summary>
    public const float DefaultNear = 0.1f;
    public const float DefaultFar = 1000f;

    /// <summary>
    /// 创建默认 SceneCameraState。
    /// </summary>
    public static SceneCameraState CreateDefault()
    {
        return new SceneCameraState
        {
            TargetX = DefaultTargetX,
            TargetZ = DefaultTargetZ,
            Distance = DefaultDistance,
            FieldOfViewDegrees = DefaultFov,
            NearPlane = DefaultNear,
            FarPlane = DefaultFar
        };
    }
}

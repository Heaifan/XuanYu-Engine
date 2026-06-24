namespace FluidWarfare.Render.Vulkan.Camera;

/// <summary>
/// 固定 3D 相机参数。
/// 本轮暂不做相机移动、旋转、缩放。
/// </summary>
public sealed record VulkanCameraInfo(
    float PositionX,
    float PositionY,
    float PositionZ,
    float TargetX,
    float TargetY,
    float TargetZ,
    float UpX,
    float UpY,
    float UpZ,
    float FieldOfViewDegrees,
    float NearPlane,
    float FarPlane)
{
    /// <summary>
    /// 默认战场相机（8.3.1 调整：更俯视，改善 RTS 构图）。
    /// </summary>
    public static VulkanCameraInfo DefaultBattlefield { get; } =
        new(
            PositionX: 0,
            PositionY: 32,
            PositionZ: 24,
            TargetX: 0,
            TargetY: 0,
            TargetZ: 0,
            UpX: 0,
            UpY: 1,
            UpZ: 0,
            FieldOfViewDegrees: 55,
            NearPlane: 0.1f,
            FarPlane: 1000f);

    public string ToSummary() =>
        $"Position ({PositionX:F0},{PositionY:F0},{PositionZ:F0}), " +
        $"Target ({TargetX:F0},{TargetY:F0},{TargetZ:F0}), " +
        $"FOV {FieldOfViewDegrees:F0}°, " +
        $"Near {NearPlane}, Far {FarPlane}";
}

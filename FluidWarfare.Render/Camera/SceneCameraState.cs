namespace FluidWarfare.Render.Camera;

/// <summary>
/// RTS 战场相机状态。
/// 使用 Target (XZ 平面) + Distance 表示，相机始终从固定俯角观察战场。
/// Position 由 Target、固定视线方向和 Distance 计算得出。
/// Target 的 Y 始终为 0（地面平面）。
/// </summary>
public sealed record SceneCameraState
{
    /// <summary>观察目标 X 坐标（世界单位）。</summary>
    public float TargetX { get; init; }

    /// <summary>观察目标 Z 坐标（世界单位）。</summary>
    public float TargetZ { get; init; }

    /// <summary>相机与目标之间的距离。</summary>
    public float Distance { get; init; }

    /// <summary>垂直视场角（度）。</summary>
    public float FieldOfViewDegrees { get; init; }

    /// <summary>近裁剪面。</summary>
    public float NearPlane { get; init; }

    /// <summary>远裁剪面。</summary>
    public float FarPlane { get; init; }

    /// <summary>
    /// 从默认位置 (0,22,32) → 目标 (0,0,0) 归一化的视线方向。
    /// 相机始终沿此方向观察目标。
    /// </summary>
    /// <summary>
    /// 获取默认视线方向（从 (0,22,32) 指向 (0,0,0) 的归一化向量）。
    /// </summary>
    public static (float X, float Y, float Z) DefaultViewDirection() => ViewDirection;

    private static readonly (float X, float Y, float Z) ViewDirection = ComputeViewDirection();

    private static (float X, float Y, float Z) ComputeViewDirection()
    {
        var len = (float)Math.Sqrt(22.0 * 22.0 + 32.0 * 32.0);
        return (0, -22.0f / len, -32.0f / len);
    }

    /// <summary>
    /// 根据 Target 和 Distance 计算相机世界坐标。
    /// Position = Target - viewDirection × Distance
    /// </summary>
    public (float X, float Y, float Z) ComputePosition()
    {
        return (
            TargetX - ViewDirection.X * Distance,
            0 - ViewDirection.Y * Distance,
            TargetZ - ViewDirection.Z * Distance
        );
    }

    /// <summary>
    /// 返回诊断摘要。
    /// </summary>
    public string ToSummary()
    {
        var (px, py, pz) = ComputePosition();
        return $"Target ({TargetX:F1},{TargetZ:F1}), " +
               $"Distance {Distance:F1}, " +
               $"Position ({px:F1},{py:F1},{pz:F1}), " +
               $"FOV {FieldOfViewDegrees:F0}°";
    }
}

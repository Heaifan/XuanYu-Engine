namespace FluidWarfare.Render.Camera;

/// <summary>
/// Blender 风格轨道相机状态。
/// Position = Pivot + OrbitDirection(Yaw, Pitch) × Distance。
/// Target (LookAt 目标) = Pivot。
/// </summary>
public sealed record SceneOrbitCameraState
{
    /// <summary>轨道中心 X。</summary>
    public float PivotX { get; init; }

    /// <summary>轨道中心 Y。</summary>
    public float PivotY { get; init; }

    /// <summary>轨道中心 Z。</summary>
    public float PivotZ { get; init; }

    /// <summary>水平旋转角（度）。0 = 看向 -Z。</summary>
    public float Yaw { get; init; }

    /// <summary>垂直旋转角（度）。0 = 水平，限制 5..89。</summary>
    public float Pitch { get; init; }

    /// <summary>相机到 Pivot 的距离。</summary>
    public float Distance { get; init; }

    /// <summary>垂直视场角（度）。</summary>
    public float FieldOfViewDegrees { get; init; }

    /// <summary>近裁剪面。</summary>
    public float NearPlane { get; init; }

    /// <summary>远裁剪面。</summary>
    public float FarPlane { get; init; }

    /// <summary>
    /// 计算相机世界位置。
    /// </summary>
    public (float X, float Y, float Z) ComputePosition()
    {
        var yawRad = Yaw * MathF.PI / 180f;
        var pitchRad = Pitch * MathF.PI / 180f;
        var cp = MathF.Cos(pitchRad);
        var dirX = MathF.Sin(yawRad) * cp;
        var dirY = MathF.Sin(pitchRad);
        var dirZ = -MathF.Cos(yawRad) * cp;

        return (
            PivotX + dirX * Distance,
            PivotY + dirY * Distance,
            PivotZ + dirZ * Distance
        );
    }

    /// <summary>
    /// 返回诊断摘要。
    /// </summary>
    public string ToSummary()
    {
        var (px, py, pz) = ComputePosition();
        return $"Pivot ({PivotX:F1},{PivotY:F1},{PivotZ:F1}), " +
               $"Yaw {Yaw:F1}° Pitch {Pitch:F1}° " +
               $"Distance {Distance:F1}, " +
               $"Position ({px:F1},{py:F1},{pz:F1}), " +
               $"FOV {FieldOfViewDegrees:F0}°";
    }
}

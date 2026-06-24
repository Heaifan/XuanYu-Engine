namespace XuanYu.Engine.Render.Camera;

using Navigation;

/// <summary>
/// Blender 风格轨道相机状态（Z-Up）。
/// Position = Pivot + OrbitDirection(Yaw, Pitch) × Distance。
/// Target (LookAt 目标) = Pivot。
///
/// Yaw 绕 Z 轴旋转（改变水平方向），Pitch 改变俯仰角。
/// 支持透视和正交投影模式。
/// </summary>
public sealed record SceneOrbitCameraState
{
    /// <summary>轨道中心 X。</summary>
    public float PivotX { get; init; }

    /// <summary>轨道中心 Y。</summary>
    public float PivotY { get; init; }

    /// <summary>轨道中心 Z。</summary>
    public float PivotZ { get; init; }

    /// <summary>水平旋转角（度）。0 = 看向 +Y 方向。</summary>
    public float Yaw { get; init; }

    /// <summary>垂直旋转角（度）。0 = 水平，限制 -89..89，支持底视图。</summary>
    public float Pitch { get; init; }

    /// <summary>相机到 Pivot 的距离。</summary>
    public float Distance { get; init; }

    /// <summary>垂直视场角（度）。</summary>
    public float FieldOfViewDegrees { get; init; }

    /// <summary>近裁剪面。</summary>
    public float NearPlane { get; init; }

    /// <summary>远裁剪面。</summary>
    public float FarPlane { get; init; }

    /// <summary>投影模式（透视 / 正交）。</summary>
    public SceneProjectionMode ProjectionMode { get; init; } = SceneProjectionMode.Perspective;

    /// <summary>正交投影高度范围（仅 Orthographic 模式使用）。</summary>
    public float OrthographicHeight { get; init; } = 40f;

    /// <summary>
    /// 计算相机世界位置（Z-Up）。
    /// Yaw = 绕 Z 旋转，Pitch = 俯仰角。
    ///
    /// offsetX = sin(yaw) * cos(pitch)
    /// offsetY = -cos(yaw) * cos(pitch)
    /// offsetZ = sin(pitch)
    ///
    /// Yaw=0° 时看向 +Y 方向。
    /// </summary>
    public (float X, float Y, float Z) ComputePosition()
    {
        var yawRad = Yaw * MathF.PI / 180f;
        var pitchRad = Pitch * MathF.PI / 180f;
        var cp = MathF.Cos(pitchRad);

        // Z-Up 轨道公式：X/Y 构成水平面，Z 为高度
        var offsetX = MathF.Sin(yawRad) * cp;
        var offsetY = -MathF.Cos(yawRad) * cp;
        var offsetZ = MathF.Sin(pitchRad);

        return (
            PivotX + offsetX * Distance,
            PivotY + offsetY * Distance,
            PivotZ + offsetZ * Distance
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

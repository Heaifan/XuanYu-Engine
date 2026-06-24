namespace XuanYu.Engine.Render.Camera;

/// <summary>
/// RTS 相机边界常量。
/// </summary>
public static class SceneCameraLimits
{
    /// <summary>相机与目标之间的最小距离（拉近限制）。</summary>
    public const float MinDistance = 8f;

    /// <summary>相机与目标之间的最大距离（拉远限制）。</summary>
    public const float MaxDistance = 120f;

    /// <summary>Target X 坐标最小值。</summary>
    public const float MinTargetX = -100f;

    /// <summary>Target X 坐标最大值。</summary>
    public const float MaxTargetX = 100f;

    /// <summary>Target Z 坐标最小值。</summary>
    public const float MinTargetZ = -100f;

    /// <summary>Target Z 坐标最大值。</summary>
    public const float MaxTargetZ = 100f;
}

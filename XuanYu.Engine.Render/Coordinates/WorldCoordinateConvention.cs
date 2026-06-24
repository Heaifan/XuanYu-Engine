namespace XuanYu.Engine.Render.Coordinates;

/// <summary>
/// XuanYu Engine 世界坐标宪法。
/// 右手坐标系，Z-Up：
///
///     +X：世界横向（右）
///     +Y：世界纵向（前）
///     +Z：高度（上）
///     XY：地面平面
///     Z = 0：默认地面高程
///     WorldUp = (0, 0, 1)
///
/// 所有渲染、Picking、物理、导航和编辑器子系统必须遵守此约定。
/// 不得在每帧渲染路径中进行坐标转换。
/// </summary>
public static class WorldCoordinateConvention
{
    // ─── 轴 ─────────────────────────────────────────────────────
    /// <summary>世界右方向（红）。</summary>
    public const float RightX = 1f, RightY = 0f, RightZ = 0f;

    /// <summary>世界前方向 / 第二水平轴（绿）。</summary>
    public const float ForwardX = 0f, ForwardY = 1f, ForwardZ = 0f;

    /// <summary>世界上方向（蓝，Z-Up）。</summary>
    public const float UpX = 0f, UpY = 0f, UpZ = 1f;

    // ─── 地面 ───────────────────────────────────────────────────
    /// <summary>地面法线指向 +Z。</summary>
    public const float GroundNormalX = 0f, GroundNormalY = 0f, GroundNormalZ = 1f;

    /// <summary>默认地面高程。</summary>
    public const double DefaultElevationZ = 0.0;

    // ─── 轴颜色 ────────────────────────────────────────────────
    /// <summary>X 轴颜色（红）。</summary>
    public const float AxisRedR = 1f, AxisRedG = 0f, AxisRedB = 0f;

    /// <summary>Y 轴颜色（绿）。</summary>
    public const float AxisGreenR = 0f, AxisGreenG = 1f, AxisGreenB = 0f;

    /// <summary>Z 轴颜色（蓝）。</summary>
    public const float AxisBlueR = 0f, AxisBlueG = 0f, AxisBlueB = 1f;

    // ─── 显示名称 ──────────────────────────────────────────────
    public const string ConventionName = "右手 Z-Up";
    public const string GroundPlaneName = "XY";
    public const string UpAxisName = "Z";

    /// <summary>
    /// 返回坐标宪法的摘要描述。
    /// </summary>
    public static string ToConventionSummary() =>
        $"坐标系统：{ConventionName}，地面：{GroundPlaneName}，向上：{UpAxisName}";
}

namespace XuanYu.Engine.Render.Camera.Navigation;

/// <summary>
/// 标准视图方向标识。
/// 对应六个基本轴向视图。
/// </summary>
public enum SceneNavigationView
{
    /// <summary>自由视角（非标准轴向）。</summary>
    Free,

    /// <summary>从 +X 方向看向 Pivot。</summary>
    PositiveX,

    /// <summary>从 -X 方向看向 Pivot。</summary>
    NegativeX,

    /// <summary>从 +Y 方向看向 Pivot。</summary>
    PositiveY,

    /// <summary>从 -Y 方向看向 Pivot。</summary>
    NegativeY,

    /// <summary>顶视图：从 +Z 方向看向 Pivot。</summary>
    PositiveZ,

    /// <summary>底视图：从 -Z 方向看向 Pivot。</summary>
    NegativeZ
}

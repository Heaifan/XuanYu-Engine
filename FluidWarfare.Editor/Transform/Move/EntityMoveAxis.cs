namespace FluidWarfare.Editor.Transform.Move;

/// <summary>
/// 实体移动工具的轴向约束。
/// </summary>
public enum EntityMoveAxis
{
    /// <summary>XY 地面平面自由移动，Z 保持初始值。</summary>
    GroundPlane,

    /// <summary>仅改变 X 轴。</summary>
    X,

    /// <summary>仅改变 Y 轴。</summary>
    Y,

    /// <summary>屏幕垂直方向改变 Z 轴。</summary>
    Z,
}

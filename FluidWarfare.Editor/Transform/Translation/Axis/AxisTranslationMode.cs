namespace FluidWarfare.Editor.Transform.Translation.Axis;

/// <summary>
/// 轴向平移求解器的执行模式。
/// ScreenProjection: 将轴投影到屏幕，从鼠标像素增量计算世界位移。
/// DragPlane: 使用射线-拖动平面求交。
/// Disabled: 轴正对相机，无法可靠求解。
/// </summary>
public enum AxisTranslationMode
{
    ScreenProjection,
    DragPlane,
    Disabled,
}

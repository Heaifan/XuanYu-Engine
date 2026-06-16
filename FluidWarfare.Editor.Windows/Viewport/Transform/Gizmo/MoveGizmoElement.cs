namespace FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo;

/// <summary>
/// Move Gizmo 的可交互元素。
/// 对应三根轴、三个平面块和中心视图平面。
/// </summary>
public enum MoveGizmoElement
{
    None,
    AxisX,
    AxisY,
    AxisZ,
    PlaneXY,
    PlaneXZ,
    PlaneYZ,
    ViewPlane,
}

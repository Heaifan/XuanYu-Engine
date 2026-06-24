using XuanYu.Engine.Core.Math;

namespace XuanYu.Engine.Editor.Transform.Translation.Axis;

/// <summary>
/// 轴向平移拖动锚点。一次拖动期间不可变。
/// ScreenProjection 模式：从 StartPointer 和当前 Pointer 计算位移。
/// DragPlane 模式：从 startIntersection 和当前射线-平面交点计算位移。
/// </summary>
public readonly record struct AxisTranslationAnchor(
    Vector3d InitialPosition,
    Vector3d Axis,
    Vector3d Pivot,
    double PixelsPerWorldUnit,
    Vector2d ScreenDirection,
    double StartPointerX,
    double StartPointerY,
    AxisTranslationMode Mode)
{
    // ── DragPlane 模式字段 ────────────────────────────
    public Vector3d StartIntersection { get; init; }
    public Vector3d DragPlaneNormal { get; init; }
    public Vector3d CameraForward { get; init; }
    public Vector3d CameraRight { get; init; }
    public Vector3d CameraUp { get; init; }

    public bool IsValid =>
        Mode switch
        {
            AxisTranslationMode.ScreenProjection =>
                double.IsFinite(PixelsPerWorldUnit) && PixelsPerWorldUnit > 0 &&
                double.IsFinite(ScreenDirection.X) && double.IsFinite(ScreenDirection.Y),
            AxisTranslationMode.DragPlane =>
                double.IsFinite(StartIntersection.X),
            _ => false,
        };
}

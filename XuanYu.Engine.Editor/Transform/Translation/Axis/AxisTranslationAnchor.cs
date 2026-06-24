using FluidWarfare.Core.Math;
using FluidWarfare.Editor.Transform.Translation.Constraint;

namespace FluidWarfare.Editor.Transform.Translation.Axis;

/// <summary>
/// 轴向平移拖动锚点。一次拖动期间不可变。
/// 使用屏幕投影算法时从 StartPointer 和当前 Pointer 计算位移。
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
    public bool IsValid =>
        double.IsFinite(PixelsPerWorldUnit) && PixelsPerWorldUnit > 0 &&
        double.IsFinite(ScreenDirection.X) && double.IsFinite(ScreenDirection.Y);
}

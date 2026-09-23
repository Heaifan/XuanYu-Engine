using Avalonia;
using Avalonia.Input;
using Avalonia.VisualTree;
using XuanYu.Editor.Input;

namespace XuanYu.Editor.UI;

public readonly record struct AvaloniaPointerSample(
    EditorPointerEventKind Kind, EditorPointerPosition Position,
    EditorPointerButtons Buttons, EditorPointerModifiers Modifiers,
    double WheelDelta, long PointerId);

public static class AvaloniaPointerEventAdapter
{
    public static EditorPointerEvent Convert(
        AvaloniaPointerSample sample, ViewportPointerSource source, double dpiScale) =>
        new(sample.Kind, sample.Position, sample.Buttons, sample.Modifiers,
            sample.WheelDelta, sample.PointerId, source, dpiScale > 0 ? dpiScale : 1);

    public static EditorPointerEvent FromMove(
        PointerEventArgs args, Visual relativeTo, ViewportPointerSource source, double dpiScale) =>
        Convert(Sample(args, relativeTo, EditorPointerEventKind.Move), source, dpiScale);

    public static EditorPointerEvent FromPressed(
        PointerPressedEventArgs args, Visual relativeTo, ViewportPointerSource source, double dpiScale) =>
        Convert(Sample(args, relativeTo, EditorPointerEventKind.Pressed), source, dpiScale);

    public static EditorPointerEvent FromReleased(
        PointerReleasedEventArgs args, Visual relativeTo, ViewportPointerSource source, double dpiScale) =>
        Convert(Sample(args, relativeTo, EditorPointerEventKind.Released), source, dpiScale);

    public static EditorPointerEvent FromWheel(
        PointerWheelEventArgs args, Visual relativeTo, ViewportPointerSource source, double dpiScale) =>
        Convert(Sample(args, relativeTo, EditorPointerEventKind.Wheel), source, dpiScale);

    static AvaloniaPointerSample Sample(PointerEventArgs args, Visual relativeTo, EditorPointerEventKind kind)
    {
        var point = args.GetCurrentPoint(relativeTo);
        return new(kind, new(point.Position.X, point.Position.Y), MapButtons(point.Properties),
            MapModifiers(args.KeyModifiers), kind == EditorPointerEventKind.Wheel
                ? ((PointerWheelEventArgs)args).Delta.Y : 0, (long)args.Pointer.Id);
    }

    static EditorPointerButtons MapButtons(PointerPointProperties properties) =>
        (properties.IsLeftButtonPressed ? EditorPointerButtons.Left : 0) |
        (properties.IsRightButtonPressed ? EditorPointerButtons.Right : 0) |
        (properties.IsMiddleButtonPressed ? EditorPointerButtons.Middle : 0);

    static EditorPointerModifiers MapModifiers(KeyModifiers modifiers) =>
        (modifiers.HasFlag(KeyModifiers.Shift) ? EditorPointerModifiers.Shift : 0) |
        (modifiers.HasFlag(KeyModifiers.Control) ? EditorPointerModifiers.Control : 0) |
        (modifiers.HasFlag(KeyModifiers.Alt) ? EditorPointerModifiers.Alt : 0) |
        (modifiers.HasFlag(KeyModifiers.Meta) ? EditorPointerModifiers.Meta : 0);
}

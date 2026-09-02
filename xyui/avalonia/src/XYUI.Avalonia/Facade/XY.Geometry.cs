using Avalonia;
using Avalonia.Controls;
using XYUI.Avalonia.Facade;

namespace XYUI.Avalonia;

public sealed partial class XY
{
    public static readonly AttachedProperty<string?> RadiusProperty = AvaloniaProperty.RegisterAttached<XY, AvaloniaObject, string?>("Radius");
    public static readonly AttachedProperty<string?> BorderProperty = AvaloniaProperty.RegisterAttached<XY, AvaloniaObject, string?>("Border");
    public static void SetRadius(AvaloniaObject target, string? value) => target.SetValue(RadiusProperty, value);
    public static string? GetRadius(AvaloniaObject target) => target.GetValue(RadiusProperty);
    public static void SetBorder(AvaloniaObject target, string? value) => target.SetValue(BorderProperty, value);
    public static string? GetBorder(AvaloniaObject target) => target.GetValue(BorderProperty);

    static void RegisterGeometryHandlers()
    {
        RadiusProperty.Changed.AddClassHandler<Control>((target, change) => XyuiFacadeRuntime.Radius(target, change.GetNewValue<string?>()));
        BorderProperty.Changed.AddClassHandler<Control>((target, change) => XyuiFacadeRuntime.Border(target, change.GetNewValue<string?>()));
    }
}

using Avalonia;
using Avalonia.Controls;
using XYUI.Avalonia.Facade;

namespace XYUI.Avalonia;

public sealed partial class XY
{
    public static readonly AttachedProperty<string?> ForegroundProperty =
        AvaloniaProperty.RegisterAttached<XY, AvaloniaObject, string?>("Foreground", null, inherits: true);
    public static readonly AttachedProperty<string?> SurfaceProperty =
        AvaloniaProperty.RegisterAttached<XY, AvaloniaObject, string?>("Surface");
    public static void SetForeground(AvaloniaObject target, string? value) => target.SetValue(ForegroundProperty, value);
    public static string? GetForeground(AvaloniaObject target) => target.GetValue(ForegroundProperty);
    public static void SetSurface(AvaloniaObject target, string? value) => target.SetValue(SurfaceProperty, value);
    public static string? GetSurface(AvaloniaObject target) => target.GetValue(SurfaceProperty);

    static XY()
    {
        ForegroundProperty.Changed.AddClassHandler<Control>((target, change) => XyuiFacadeRuntime.Foreground(target, change.GetNewValue<string?>()));
        SurfaceProperty.Changed.AddClassHandler<Control>((target, change) => XyuiFacadeRuntime.Surface(target, change.GetNewValue<string?>()));
        RegisterTypographyHandlers();
        RegisterSpacingHandlers();
        RegisterGeometryHandlers();
    }
}

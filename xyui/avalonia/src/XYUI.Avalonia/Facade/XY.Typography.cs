using Avalonia;
using Avalonia.Controls;
using XYUI.Avalonia.Facade;

namespace XYUI.Avalonia;

public sealed partial class XY
{
    public static readonly AttachedProperty<string?> FontProperty =
        AvaloniaProperty.RegisterAttached<XY, AvaloniaObject, string?>("Font", null, inherits: true);
    public static readonly AttachedProperty<string?> TypographyProperty =
        AvaloniaProperty.RegisterAttached<XY, AvaloniaObject, string?>("Typography", null, inherits: true);
    public static void SetFont(AvaloniaObject target, string? value) => target.SetValue(FontProperty, value);
    public static string? GetFont(AvaloniaObject target) => target.GetValue(FontProperty);
    public static void SetTypography(AvaloniaObject target, string? value) => target.SetValue(TypographyProperty, value);
    public static string? GetTypography(AvaloniaObject target) => target.GetValue(TypographyProperty);

    static void RegisterTypographyHandlers()
    {
        FontProperty.Changed.AddClassHandler<Control>((target, change) => XyuiFacadeRuntime.Font(target, change.GetNewValue<string?>()));
        TypographyProperty.Changed.AddClassHandler<Control>((target, change) => XyuiFacadeRuntime.Typography(target, change.GetNewValue<string?>()));
    }
}

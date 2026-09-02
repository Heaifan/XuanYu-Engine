using Avalonia;
using Avalonia.Controls;
using XYUI.Avalonia.Facade;

namespace XYUI.Avalonia;

public sealed partial class XY
{
    public static readonly AttachedProperty<string?> PaddingProperty = AvaloniaProperty.RegisterAttached<XY, AvaloniaObject, string?>("Padding");
    public static readonly AttachedProperty<string?> GapProperty = AvaloniaProperty.RegisterAttached<XY, AvaloniaObject, string?>("Gap");
    public static readonly AttachedProperty<string?> MarginProperty = AvaloniaProperty.RegisterAttached<XY, AvaloniaObject, string?>("Margin");
    public static void SetPadding(AvaloniaObject target, string? value) => target.SetValue(PaddingProperty, value);
    public static string? GetPadding(AvaloniaObject target) => target.GetValue(PaddingProperty);
    public static void SetGap(AvaloniaObject target, string? value) => target.SetValue(GapProperty, value);
    public static string? GetGap(AvaloniaObject target) => target.GetValue(GapProperty);
    public static void SetMargin(AvaloniaObject target, string? value) => target.SetValue(MarginProperty, value);
    public static string? GetMargin(AvaloniaObject target) => target.GetValue(MarginProperty);

    static void RegisterSpacingHandlers()
    {
        PaddingProperty.Changed.AddClassHandler<Control>((target, change) => XyuiFacadeRuntime.Padding(target, change.GetNewValue<string?>()));
        GapProperty.Changed.AddClassHandler<Control>((target, change) => XyuiFacadeRuntime.Gap(target, change.GetNewValue<string?>()));
        MarginProperty.Changed.AddClassHandler<Control>((target, change) => XyuiFacadeRuntime.Margin(target, change.GetNewValue<string?>()));
    }
}

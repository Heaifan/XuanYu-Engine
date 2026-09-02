using Avalonia;
using XYUI.Avalonia.Density;
using XYUI.Avalonia.Sizing;

namespace XYUI.Avalonia;

public enum XYDensity
{
    Compact,
    Default,
    Comfortable,
}

public sealed partial class XY
{
    private XY() { }

    public static readonly AttachedProperty<XYSize> SizeProperty =
        AvaloniaProperty.RegisterAttached<XY, AvaloniaObject, XYSize>(
            "Size", XYSize.Default, inherits: true);

    public static readonly AttachedProperty<XYDensity> DensityProperty =
        AvaloniaProperty.RegisterAttached<XY, AvaloniaObject, XYDensity>(
            "Density", XYDensity.Default, inherits: true);

    public static void SetSize(AvaloniaObject target, XYSize value) => target.SetValue(SizeProperty, value);
    public static XYSize GetSize(AvaloniaObject target) => target.GetValue(SizeProperty);
    public static void SetDensity(AvaloniaObject target, XYDensity value) => target.SetValue(DensityProperty, value);
    public static XYDensity GetDensity(AvaloniaObject target) => target.GetValue(DensityProperty);
}

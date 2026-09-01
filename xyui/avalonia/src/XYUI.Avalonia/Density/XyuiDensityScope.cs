using Avalonia;
using Avalonia.Controls;

namespace XYUI.Avalonia.Density;

public sealed class XyuiDensityScope
{
    private XyuiDensityScope() { }
    public static readonly AttachedProperty<XyuiDensity> DensityProperty =
        AvaloniaProperty.RegisterAttached<XyuiDensityScope, Control, XyuiDensity>(
            "Density", XyuiDensity.Default, inherits: true);

    public static void SetDensity(Control element, XyuiDensity value) =>
        element.SetValue(DensityProperty, value);

    public static XyuiDensity GetDensity(Control element) =>
        element.GetValue(DensityProperty);

    public static XyuiDensityMetrics GetMetrics(Control element) =>
        XyuiDensityMetrics.For(GetDensity(element));
}

using Avalonia;
using Avalonia.Controls;

namespace XYUI.Avalonia.Density;

public sealed class XyuiDensityScope
{
    private XyuiDensityScope() { }
    public static readonly AttachedProperty<XyuiDensityMode> ModeProperty =
        AvaloniaProperty.RegisterAttached<XyuiDensityScope, Control, XyuiDensityMode>(
            "Mode", XyuiDensityMode.Comfortable, inherits: true);

    public static readonly AttachedProperty<XyuiDensityPolicy> PolicyProperty =
        AvaloniaProperty.RegisterAttached<XyuiDensityScope, Control, XyuiDensityPolicy>(
            "Policy", XyuiDensityPolicy.Auto, inherits: true);

    public static XyuiDensityMode GetMode(Control element) => element.GetValue(ModeProperty);
    public static void SetMode(Control element, XyuiDensityMode value) => element.SetValue(ModeProperty, value);
    public static XyuiDensityPolicy GetPolicy(Control element) => element.GetValue(PolicyProperty);
    public static void SetPolicy(Control element, XyuiDensityPolicy value) => element.SetValue(PolicyProperty, value);

    public static bool TryGetMetrics(Control element, out XyuiDensityMetrics metrics) =>
        XyuiDensity.TryGetMetrics(GetMode(element), out metrics);

    public static bool TryGetSemanticMetrics(Control element, out XyuiDensitySemanticMetrics metrics) =>
        XyuiDensity.TryGetSemanticMetrics(GetMode(element), out metrics);

    public static ResourceDictionary CreateSemanticResources(Control element) =>
        XyuiDensity.CreateResolvedSemanticResources(GetMode(element));
}

using Avalonia;
using Avalonia.Controls;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Foundation;

public enum XyuiSizeRole { Compact, Default, Comfortable, Touch }

public readonly record struct XyuiSizingMetrics(
    double ControlHeight, double IconSize, double MinimumHitTarget);

public sealed class XyuiSizingScope
{
    private XyuiSizingScope() { }
    public static readonly AttachedProperty<XyuiSizeRole> SizeRoleProperty =
        AvaloniaProperty.RegisterAttached<XyuiSizingScope, Control, XyuiSizeRole>(
            "SizeRole", XyuiSizeRole.Default, inherits: true);

    public static XyuiSizeRole GetSizeRole(Control element) => element.GetValue(SizeRoleProperty);
    public static void SetSizeRole(Control element, XyuiSizeRole value)
    {
        element.SetValue(SizeRoleProperty, value);
        Apply(element, element is XYIconButton);
    }

    public static void Attach(Control element, bool iconOnly = false)
    {
        element.PropertyChanged += (_, args) =>
        {
            if (args.Property == SizeRoleProperty) Apply(element, iconOnly);
        };
    }

    public static bool TryGetMetrics(XyuiSizeRole role, out XyuiSizingMetrics metrics)
    {
        metrics = role switch
        {
            XyuiSizeRole.Compact => new(28, 14, 28),
            XyuiSizeRole.Default => new(32, 16, 32),
            XyuiSizeRole.Comfortable => new(36, 20, 36),
            XyuiSizeRole.Touch => new(44, 24, 44),
            _ => default,
        };
        return Enum.IsDefined(role);
    }

    public static XyuiSizingMetrics GetMetrics(XyuiSizeRole role) =>
        TryGetMetrics(role, out var metrics) ? metrics : throw new ArgumentOutOfRangeException(nameof(role));

    static void Apply(Control element, bool iconOnly)
    {
        if (!TryGetMetrics(GetSizeRole(element), out var metrics)) return;
        element.MinHeight = metrics.MinimumHitTarget;
        if (iconOnly)
        {
            element.Width = metrics.ControlHeight;
            element.Height = metrics.ControlHeight;
            element.MinWidth = metrics.MinimumHitTarget;
            return;
        }
        element.Height = metrics.ControlHeight;
    }
}

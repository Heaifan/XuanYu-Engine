using XYUI.Avalonia;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Sizing;

public readonly record struct XyuiSizingMetrics(double ControlHeight, double IconSize)
{
    public static XyuiIconSize IconFor(XYSize size) => size switch
    {
        XYSize.Compact => XyuiIconSize.Compact,
        XYSize.Comfortable => XyuiIconSize.Comfortable,
        XYSize.Touch => XyuiIconSize.Touch,
        _ => XyuiIconSize.Default,
    };

    public static XyuiSizingMetrics For(XYSize size) => size switch
    {
        XYSize.Compact => new(28, 14),
        XYSize.Comfortable => new(36, 20),
        XYSize.Touch => new(44, 24),
        _ => new(32, 16),
    };
}

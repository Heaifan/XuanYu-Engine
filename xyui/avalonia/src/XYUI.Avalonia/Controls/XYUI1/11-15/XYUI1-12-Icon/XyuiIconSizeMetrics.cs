namespace XYUI.Avalonia.Controls;

public readonly record struct XyuiIconSizeMetrics(double SizeDip, double StrokeWidth)
{
    public static XyuiIconSizeMetrics For(XyuiIconSize size) => size switch
    {
        XyuiIconSize.Tiny => new(12, 1),
        XyuiIconSize.Small => new(14, 1.25),
        XyuiIconSize.Large => new(20, 1.75),
        XyuiIconSize.Touch => new(24, 2),
        _ => new(16, 1.5),
    };
}

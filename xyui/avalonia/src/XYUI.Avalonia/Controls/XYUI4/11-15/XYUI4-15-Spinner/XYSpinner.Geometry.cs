using Avalonia.Media;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYSpinner
{
    static readonly StreamGeometry ArcGeometry = StreamGeometry.Parse("M 0 -1 A 1 1 0 0 1 0.866 0.5");

    public static double SizeFor(XyuiSpinnerSize size) => size switch
    {
        XyuiSpinnerSize.Compact => 14d,
        XyuiSpinnerSize.Large => 24d,
        _ => 18d
    };

    public static double StrokeFor(XyuiSpinnerSize size) => size switch
    {
        XyuiSpinnerSize.Compact => 2d,
        XyuiSpinnerSize.Large => 3d,
        _ => 2.5d
    };
}

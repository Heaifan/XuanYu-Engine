namespace XYUI.Avalonia.Controls;

internal static class XyuiStatusStateTokens
{
    public static string Foreground(XyuiStatusState state) => state switch
    {
        XyuiStatusState.Success => "XY.Brush.Semantic.Success.Text",
        XyuiStatusState.Warning => "XY.Brush.Semantic.Warning.Text",
        XyuiStatusState.Error => "XY.Brush.Semantic.Error.Text",
        XyuiStatusState.Info => "XY.Brush.Semantic.Info.Text",
        _ => "XY.Brush.Text.Secondary",
    };

    public static string Indicator(XyuiStatusState state) => state switch
    {
        XyuiStatusState.Neutral => "XY.Brush.Text.Tertiary",
        _ => Foreground(state),
    };
}

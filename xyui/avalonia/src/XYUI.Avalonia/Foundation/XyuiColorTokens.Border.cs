namespace XYUI.Avalonia.Foundation;

public static partial class XyuiColorTokens
{
    // XY.Border.Color.*（0.2-D）+ XY.Divider.*（0.2-D 分割线）
    public static readonly XyuiColorToken[] Border =
    [
        XyuiColorToken.Parse("XY.Border.Color.Subtle", "#D7E0E6/#303C45"),
        XyuiColorToken.Parse("XY.Border.Color.Default", "#BFCBD3/#465966"),
        XyuiColorToken.Parse("XY.Border.Color.Strong", "#95A7B3/#687B88"),
        XyuiColorToken.Parse("XY.Border.Color.Focus", "#5C8FB4/#699CC0"),
        XyuiColorToken.Parse("XY.Border.Color.Selected", "#3E78A4/#80B1D5"),
        XyuiColorToken.Parse("XY.Divider.Default", "#CAD5DC/#3A4852"),
    ];
}

namespace XYUI.Avalonia.Foundation;

public static partial class XyuiColorTokens
{
    // XY.Semantic.* —— Success/Warning/Error/Info 三通道（Text/Border/Background）
    public static readonly XyuiColorToken[] Semantic =
    [
        XyuiColorToken.Parse("XY.Semantic.Success.Text", "#4E7B66/#76A58A"),
        XyuiColorToken.Parse("XY.Semantic.Success.Border", "#80A58E/#5E856E"),
        XyuiColorToken.Parse("XY.Semantic.Success.Background", "#E1EEE6/#263A2E"),
        XyuiColorToken.Parse("XY.Semantic.Warning.Text", "#A57634/#D0A05C"),
        XyuiColorToken.Parse("XY.Semantic.Warning.Border", "#C09B62/#8D6D45"),
        XyuiColorToken.Parse("XY.Semantic.Warning.Background", "#F5ECDB/#3D3021"),
        XyuiColorToken.Parse("XY.Semantic.Error.Text", "#B34F58/#D4767D"),
        XyuiColorToken.Parse("XY.Semantic.Error.Border", "#CA8087/#92555B"),
        XyuiColorToken.Parse("XY.Semantic.Error.Background", "#F8E4E6/#42282C"),
        XyuiColorToken.Parse("XY.Semantic.Info.Text", "#4C7597/#82AAC8"),
        XyuiColorToken.Parse("XY.Semantic.Info.Border", "#82A4BC/#5F7D93"),
        XyuiColorToken.Parse("XY.Semantic.Info.Background", "#E3EEF5/#273946"),
    ];
}

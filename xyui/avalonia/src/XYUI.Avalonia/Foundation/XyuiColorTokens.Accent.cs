namespace XYUI.Avalonia.Foundation;

public static partial class XyuiColorTokens
{
    // XY.Accent.*（0.2-E）+ 同值引用家族：Tool/Button/Tag
    public static readonly XyuiColorToken[] Accent =
    [
        XyuiColorToken.Parse("XY.Accent.Default", "#4A789E/#82A9C5"),
        XyuiColorToken.Parse("XY.Accent.Soft", "#D8E7F2/#35536A"),
        XyuiColorToken.Parse("XY.Accent.Strong", "#356C99/#7FB0D5"),
        XyuiColorToken.Parse("XY.Tool.Active", "#356C99/#7FB0D5"),
        XyuiColorToken.Parse("XY.Button.Primary", "#356C99/#7FB0D5"),
        XyuiColorToken.Parse("XY.Tag.Accent", "#D8E7F2/#35536A"),
    ];
}

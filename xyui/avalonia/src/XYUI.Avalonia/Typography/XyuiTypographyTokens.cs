namespace XYUI.Avalonia.Typography;

// XYUI-0 Foundation Typography token 权威表（转录 token-canonical-map.json，来源 XYUI0-0.3/0.4/0.5）
public static class XyuiTypographyTokens
{
    // Font Family（enum_or_string，可商用 + 随包 License）
    public const string FontUi = "Source Han Sans SC";
    public const string FontDefault = FontUi;
    public const string FontMono = "Source Code Pro";
    public const string FontTechnical = FontMono;
    public const string FontFallbackCjk = "Noto Sans CJK SC";
    public const string FontFallbackMono = "Noto Sans Mono";

    // Font Size（DIP，八档）
    public const double FontSizeCaption = 12;
    public const double FontSizeAuxiliary = 13;
    public const double FontSizeBody = 14;
    public const double FontSizeLabel = 15;
    public const double FontSizeSection = 17;
    public const double FontSizePanelTitle = 20;
    public const double FontSizePageTitle = 24;
    public const double FontSizeMono = 13;

    // Font Weight（OpenType 数值，四档）
    public const int WeightRegular = 400;
    public const int WeightMedium = 500;
    public const int WeightSemibold = 600;
    public const int WeightBold = 700;

    // Line Height（DIP，成对 "size/line" 中的行高值）
    public const double LineHeightCaption = 16;
    public const double LineHeightAuxiliary = 18;
    public const double LineHeightBody = 20;
    public const double LineHeightLabel = 20;
    public const double LineHeightSection = 22;
    public const double LineHeightPanelTitle = 26;
    public const double LineHeightPageTitle = 30;
    public const double LineHeightMono = 20;

    // Letter Spacing（五档）
    public const double LetterSpacingBody = 0;
    public const double LetterSpacingLabel = -0.10;
    public const double LetterSpacingTitle = 0;
    public const double LetterSpacingCaps = 0.40;
    public const double LetterSpacingMono = 0;
}

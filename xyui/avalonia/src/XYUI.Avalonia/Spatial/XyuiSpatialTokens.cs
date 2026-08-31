namespace XYUI.Avalonia.Spatial;

// XYUI-0 Foundation Spatial/Shape token 权威表（转录 token-canonical-map.json：0.6/0.9/0.10/0.13）
public static class XyuiSpatialTokens
{
    // Spacing（4 DIP 基础单位，八档；0.6）
    public const double Space1 = 4;
    public const double Space2 = 8;
    public const double Space3 = 12;
    public const double Space4 = 16;
    public const double Space6 = 24;
    public const double Space8 = 32;
    public const double Space10 = 40;
    public const double Space12 = 48;
    public const double PanelPadding = 8;
    public const double FieldRowGap = 4;
    public const double SectionGap = 8;
    public const double IndentPerLevel = 16;
    public const double IndentIconTextGap = 4;

    // Radius（圆角表达组件角色，Panel/Row 方正；0.9）
    public const double RadiusNone = 0;
    public const double RadiusToolbar = 2;
    public const double RadiusControl = 4;
    public const double RadiusInput = 4;
    public const double RadiusButton = 4;
    public const double RadiusPopup = 6;
    public const double RadiusPanel = 0;
    public const double RadiusRow = 0;
    public const double RadiusFull = 999;

    // Border Width（0.10：Control 1 DIP 完整边框，关键结构 2 DIP）
    public const double BorderWidthNone = 0;
    public const double BorderWidthDefault = 1;
    public const double BorderWidthStrong = 2;
    public const double BorderWidthFocus = 2;
    public const double BorderWidthSelected = 2;

    // Elevation / Shadow（0.13：仅表达 Z 轴脱离；格式 x/y/blur/alpha 黑阴影）
    public const string ShadowNone = "None";
    public const string ShadowTooltip = "0/3/10/0.12";
    public const string ShadowPopup = "0/6/18/0.14";
    public const string ShadowDragPreview = "0/6/18/0.14";
}

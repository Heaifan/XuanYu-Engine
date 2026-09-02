using Avalonia;
using Avalonia.Media;
using XYUI.Avalonia.Foundation;
using XYUI.Avalonia.Spatial;
using XYUI.Avalonia.Typography;

namespace XYUI.Avalonia.Facade;

internal sealed record XyuiTypographyRole(string Font, double Size, double LineHeight, int Weight);

internal static class XyuiFacadeResolver
{
    internal static bool TryColor(string name, out string key)
    {
        var valid = XyuiColorTokens.TryFind(name, out _);
        key = valid ? XyuiColorTokens.BrushKey(name) : "";
        return valid;
    }

    internal static bool TryFont(string name, out FontFamily value)
    {
        value = name switch
        {
            "XY.Font.UI" or "XY.Font.Default" => new(XyuiTypographyTokens.FontUi),
            "XY.Font.Mono" or "XY.Font.Technical" => new(XyuiTypographyTokens.FontMono),
            "XY.Font.Fallback.CJK" => new(XyuiTypographyTokens.FontFallbackCjk),
            "XY.Font.Fallback.Mono" => new(XyuiTypographyTokens.FontFallbackMono),
            _ => new FontFamily(XyuiTypographyTokens.FontUi)
        };
        return name is "XY.Font.UI" or "XY.Font.Default" or "XY.Font.Mono" or "XY.Font.Technical" or "XY.Font.Fallback.CJK" or "XY.Font.Fallback.Mono";
    }

    internal static bool TryTypography(string name, out XyuiTypographyRole role)
    {
        role = name switch
        {
            "XY.Type.Caption" => new(XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeCaption, XyuiTypographyTokens.LineHeightCaption, XyuiTypographyTokens.WeightRegular),
            "XY.Type.Auxiliary" => new(XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeAuxiliary, XyuiTypographyTokens.LineHeightAuxiliary, XyuiTypographyTokens.WeightRegular),
            "XY.Type.Body" => new(XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeBody, XyuiTypographyTokens.LineHeightBody, XyuiTypographyTokens.WeightRegular),
            "XY.Type.Label" => new(XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeLabel, XyuiTypographyTokens.LineHeightLabel, XyuiTypographyTokens.WeightMedium),
            "XY.Type.Section" => new(XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizeSection, XyuiTypographyTokens.LineHeightSection, XyuiTypographyTokens.WeightSemibold),
            "XY.Type.PanelTitle" => new(XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizePanelTitle, XyuiTypographyTokens.LineHeightPanelTitle, XyuiTypographyTokens.WeightSemibold),
            "XY.Type.PageTitle" => new(XyuiTypographyTokens.FontUi, XyuiTypographyTokens.FontSizePageTitle, XyuiTypographyTokens.LineHeightPageTitle, XyuiTypographyTokens.WeightBold),
            "XY.Type.Mono" => new(XyuiTypographyTokens.FontMono, XyuiTypographyTokens.FontSizeMono, XyuiTypographyTokens.LineHeightMono, XyuiTypographyTokens.WeightRegular),
            _ => new("", 0, 0, 0)
        };
        return role.Size > 0;
    }

    internal static bool TrySpace(string name, out double value) => (value = name switch
    {
        "XY.Space.1" => XyuiSpatialTokens.Space1, "XY.Space.2" => XyuiSpatialTokens.Space2,
        "XY.Space.3" => XyuiSpatialTokens.Space3, "XY.Space.4" => XyuiSpatialTokens.Space4,
        "XY.Space.6" => XyuiSpatialTokens.Space6, "XY.Space.8" => XyuiSpatialTokens.Space8,
        "XY.Space.10" => XyuiSpatialTokens.Space10, "XY.Space.12" => XyuiSpatialTokens.Space12,
        "XY.Panel.Padding" => XyuiSpatialTokens.PanelPadding, _ => -1
    }) >= 0;

    internal static bool TryRadius(string name, out CornerRadius value) => (value = name switch
    {
        "XY.Radius.None" => new(XyuiSpatialTokens.RadiusNone), "XY.Radius.Toolbar" => new(XyuiSpatialTokens.RadiusToolbar),
        "XY.Radius.Control" => new(XyuiSpatialTokens.RadiusControl), "XY.Radius.Input" => new(XyuiSpatialTokens.RadiusInput),
        "XY.Radius.Button" => new(XyuiSpatialTokens.RadiusButton), "XY.Radius.Popup" => new(XyuiSpatialTokens.RadiusPopup),
        "XY.Radius.Panel" => new(XyuiSpatialTokens.RadiusPanel), "XY.Radius.Row" => new(XyuiSpatialTokens.RadiusRow),
        "XY.Radius.Full" => new(XyuiSpatialTokens.RadiusFull), _ => default
    }) != default;

    internal static bool TryBorder(string name, out (string Brush, double Width) value) => (value = name switch
    {
        "XY.Border.Subtle" => ("XY.Brush.Border.Color.Subtle", XyuiSpatialTokens.BorderWidthDefault),
        "XY.Border.Default" => ("XY.Brush.Border.Color.Default", XyuiSpatialTokens.BorderWidthDefault),
        "XY.Border.Strong" => ("XY.Brush.Border.Color.Strong", XyuiSpatialTokens.BorderWidthStrong), _ => ("", -1)
    }).Width >= 0;
}

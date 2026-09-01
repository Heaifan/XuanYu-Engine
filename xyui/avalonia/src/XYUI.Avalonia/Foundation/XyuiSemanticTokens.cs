using Avalonia;
using Avalonia.Controls;
using XYUI.Avalonia.Spatial;

namespace XYUI.Avalonia.Foundation;

// Foundation 语义资源：只映射已存在的 Primitive/Canonical 数值，不建立第二套真源。
public static class XyuiSemanticTokens
{
    public static ResourceDictionary CreateResources()
    {
        return new ResourceDictionary
        {
            ["XY.Gap.None"] = 0d,
            ["XY.Gap.Base"] = XyuiSpatialTokens.Space1,
            ["XY.Gap.Field"] = XyuiSpatialTokens.FieldRowGap,
            ["XY.Gap.IconText"] = XyuiSpatialTokens.IndentIconTextGap,
            ["XY.Gap.Section"] = XyuiSpatialTokens.SectionGap,
            ["XY.Padding.Panel"] = new Thickness(XyuiSpatialTokens.PanelPadding),
            ["XY.Size.Control.XS"] = XyuiSizeTokens.ControlXs,
            ["XY.Size.Control.S"] = XyuiSizeTokens.ControlS,
            ["XY.Size.Control.M"] = XyuiSizeTokens.ControlM,
            ["XY.Size.Control.L"] = XyuiSizeTokens.ControlL,
            ["XY.Size.TreeRow"] = XyuiSizeTokens.TreeRow,
            ["XY.Size.Toolbar"] = XyuiSizeTokens.Toolbar,
            ["XY.Size.Input"] = XyuiSizeTokens.Input,
            ["XY.Size.Icon.S"] = XyuiSizeTokens.IconS,
            ["XY.Size.Icon.M"] = XyuiSizeTokens.IconM,
            ["XY.Size.Icon.L"] = XyuiSizeTokens.IconL,
            ["XY.Size.Checkbox"] = XyuiSizeTokens.Checkbox,
            ["XY.Size.Radio"] = XyuiSizeTokens.Radio,
            ["XY.Size.Switch"] = new Size(XyuiSizeTokens.SwitchWidth, XyuiSizeTokens.SwitchHeight),
            ["XY.Size.Switch.Width"] = XyuiSizeTokens.SwitchWidth,
            ["XY.Size.Switch.Height"] = XyuiSizeTokens.SwitchHeight,
            ["XY.Size.Scrollbar"] = XyuiSizeTokens.Scrollbar,
            ["XY.Size.DragHandle"] = XyuiSizeTokens.DragHandle,
            ["XY.Icon.Size.S"] = XyuiSizeTokens.IconS,
            ["XY.Icon.Size.M"] = XyuiSizeTokens.IconM,
            ["XY.Icon.Size.L"] = XyuiSizeTokens.IconL,
            ["XY.Icon.Stroke"] = XyuiSizeTokens.IconStroke,
            ["XY.Icon.Style.Default"] = "Outline",
            ["XY.Icon.Style.Active"] = "Outline+LocalFill",
            ["XY.Icon.LineCap"] = "Round",
            ["XY.Icon.LineJoin"] = "Round",
            ["XY.Icon.State.Active"] = "XY.Accent.Strong",
            ["XY.Icon.State.Disabled"] = "XY.Text.Disabled",
            ["XY.Border.Width.None"] = new Thickness(XyuiSpatialTokens.BorderWidthNone),
            ["XY.Border.Width.Default"] = new Thickness(XyuiSpatialTokens.BorderWidthDefault),
            ["XY.Border.Width.Strong"] = new Thickness(XyuiSpatialTokens.BorderWidthStrong),
            ["XY.Border.Width.Focus"] = new Thickness(XyuiSpatialTokens.BorderWidthFocus),
            ["XY.Border.Width.Selected"] = new Thickness(XyuiSpatialTokens.BorderWidthSelected),
            ["XY.Border.Style.Default"] = "Solid",
            ["XY.Border.Container"] = "0 DIP/UseDivider",
            ["XY.Border.Control"] = "1 DIP/Solid",
            ["XY.Border.Strong"] = "2 DIP/Solid",
            ["XY.Border.Focus"] = "2 DIP/Solid",
            ["XY.Border.Selected"] = "2 DIP/Solid",
            ["XY.Separator.Header"] = new Thickness(0, XyuiSpatialTokens.BorderWidthDefault, 0, 0),
            ["XY.Separator.Panel"] = new Thickness(0, XyuiSpatialTokens.BorderWidthDefault, 0, 0),
            ["XY.Separator.Section"] = new Thickness(0, XyuiSpatialTokens.BorderWidthDefault, 0, 0),
            ["XY.Separator.ListRow"] = new Thickness(0, XyuiSpatialTokens.BorderWidthDefault, 0, 0),
            ["XY.Separator.VerticalSplit"] = new Thickness(XyuiSpatialTokens.BorderWidthDefault, 0, 0, 0),
        };
    }
}

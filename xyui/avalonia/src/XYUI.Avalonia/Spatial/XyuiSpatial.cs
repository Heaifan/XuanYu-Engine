using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using XYUI.Avalonia.Density;
using XYUI.Avalonia.Foundation;

namespace XYUI.Avalonia.Spatial;

// Spatial/Shape 基础资源：间距/圆角/边框宽度/浮层阴影（主题相关色由 R3-F1 Brush 提供）
public static class XyuiSpatial
{
    public static ResourceDictionary CreateResources()
    {
        var d = new ResourceDictionary();
        d["XY.Space.1"] = XyuiSpatialTokens.Space1;
        d["XY.Space.2"] = XyuiSpatialTokens.Space2;
        d["XY.Space.3"] = XyuiSpatialTokens.Space3;
        d["XY.Space.4"] = XyuiSpatialTokens.Space4;
        d["XY.Space.6"] = XyuiSpatialTokens.Space6;
        d["XY.Space.8"] = XyuiSpatialTokens.Space8;
        d["XY.Space.10"] = XyuiSpatialTokens.Space10;
        d["XY.Space.12"] = XyuiSpatialTokens.Space12;
        d["XY.Panel.Padding"] = new Thickness(XyuiSpatialTokens.PanelPadding);
        d["XY.Panel.Field.RowGap"] = XyuiSpatialTokens.FieldRowGap;
        d["XY.Panel.SectionGap"] = XyuiSpatialTokens.SectionGap;
        d["XY.Indent.PerLevel"] = XyuiSpatialTokens.IndentPerLevel;
        d["XY.Indent.IconTextGap"] = XyuiSpatialTokens.IndentIconTextGap;
        d["XY.Radius.None"] = new CornerRadius(XyuiSpatialTokens.RadiusNone);
        d["XY.Radius.Toolbar"] = new CornerRadius(XyuiSpatialTokens.RadiusToolbar);
        d["XY.Radius.Control"] = new CornerRadius(XyuiSpatialTokens.RadiusControl);
        d["XY.Radius.Input"] = new CornerRadius(XyuiSpatialTokens.RadiusInput);
        d["XY.Radius.Button"] = new CornerRadius(XyuiSpatialTokens.RadiusButton);
        d["XY.Radius.Popup"] = new CornerRadius(XyuiSpatialTokens.RadiusPopup);
        d["XY.Radius.Panel"] = new CornerRadius(XyuiSpatialTokens.RadiusPanel);
        d["XY.Radius.Row"] = new CornerRadius(XyuiSpatialTokens.RadiusRow);
        d["XY.Radius.Full"] = new CornerRadius(XyuiSpatialTokens.RadiusFull);
        d["XY.Border.Width.None"] = new Thickness(XyuiSpatialTokens.BorderWidthNone);
        d["XY.Border.Width.Default"] = new Thickness(XyuiSpatialTokens.BorderWidthDefault);
        d["XY.Border.Width.Strong"] = new Thickness(XyuiSpatialTokens.BorderWidthStrong);
        d["XY.Border.Width.Focus"] = new Thickness(XyuiSpatialTokens.BorderWidthFocus);
        d["XY.Border.Width.Selected"] = new Thickness(XyuiSpatialTokens.BorderWidthSelected);
        d["XY.Shadow.None"] = default(BoxShadows);
        d["XY.Shadow.Tooltip"] = ParseShadow(XyuiSpatialTokens.ShadowTooltip);
        d["XY.Shadow.Popup"] = ParseShadow(XyuiSpatialTokens.ShadowPopup);
        d["XY.Shadow.DragPreview"] = ParseShadow(XyuiSpatialTokens.ShadowDragPreview);
        Merge(d, XyuiSemanticTokens.CreateResources());
        Merge(d, XyuiDensity.CreateResources());
        return d;
    }

    static void Merge(ResourceDictionary target, ResourceDictionary source)
    {
        foreach (var key in source.Keys.Cast<string>()) target[key] = source[key];
    }

    // 解析 canonical "x/y/blur/alpha"（黑色阴影；x=水平偏移 y=垂直偏移 blur=模糊 alpha=透明度）
    public static BoxShadows ParseShadow(string spec)
    {
        if (spec == XyuiSpatialTokens.ShadowNone)
        {
            return default;
        }
        var parts = spec.Split('/');
        return new BoxShadows(BoxShadow.Parse(
            $"{parts[0]} {parts[1]} {parts[2]} 0 rgba(0,0,0,{parts[3]})"));
    }
}

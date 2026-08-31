using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;

namespace XYUI.Avalonia.Spatial;

// 语义形状样式类（xyui-border-* / xyui-surface-* / xyui-shadow-*）：Border 组合层
public static class XyuiShapeStyles
{
    public static Styles Create()
    {
        var styles = new Styles();
        styles.Add(BorderStyle("xyui-border-subtle", "XY.Brush.Border.Color.Subtle",
            "XY.Border.Width.Default", "XY.Radius.Control"));
        styles.Add(BorderStyle("xyui-border-default", "XY.Brush.Border.Color.Default",
            "XY.Border.Width.Default", "XY.Radius.Control"));
        styles.Add(BorderStyle("xyui-border-strong", "XY.Brush.Border.Color.Strong",
            "XY.Border.Width.Strong", "XY.Radius.Control"));
        styles.Add(BorderStyle("xyui-border-focus", "XY.Brush.Border.Color.Focus",
            "XY.Border.Width.Focus", "XY.Radius.Control"));
        styles.Add(BorderStyle("xyui-border-selected", "XY.Brush.Border.Color.Selected",
            "XY.Border.Width.Selected", "XY.Radius.Control"));
        styles.Add(SurfaceStyle("xyui-surface-panel", "XY.Brush.Surface.Panel", "XY.Radius.Panel",
            border: false));
        styles.Add(SurfaceStyle("xyui-surface-raised", "XY.Brush.Surface.Raised", "XY.Radius.Control",
            border: true));
        styles.Add(ShadowStyle("xyui-shadow-tooltip", "XY.Shadow.Tooltip"));
        styles.Add(ShadowStyle("xyui-shadow-popup", "XY.Shadow.Popup"));
        return styles;
    }

    private static Style BorderStyle(string cls, string brushKey, string widthKey, string radiusKey)
    {
        var style = new Style(x => x.OfType<Border>().Class(cls));
        style.Setters.Add(new Setter(Border.BorderBrushProperty, new DynamicResourceExtension(brushKey)));
        style.Setters.Add(new Setter(Border.BorderThicknessProperty, new DynamicResourceExtension(widthKey)));
        style.Setters.Add(new Setter(Border.CornerRadiusProperty, new DynamicResourceExtension(radiusKey)));
        return style;
    }

    private static Style SurfaceStyle(string cls, string brushKey, string radiusKey, bool border)
    {
        var style = new Style(x => x.OfType<Border>().Class(cls));
        style.Setters.Add(new Setter(Border.BackgroundProperty, new DynamicResourceExtension(brushKey)));
        style.Setters.Add(new Setter(Border.CornerRadiusProperty, new DynamicResourceExtension(radiusKey)));
        style.Setters.Add(new Setter(Border.PaddingProperty, new DynamicResourceExtension("XY.Panel.Padding")));
        if (border)
        {
            style.Setters.Add(new Setter(Border.BorderBrushProperty,
                new DynamicResourceExtension("XY.Brush.Border.Color.Default")));
            style.Setters.Add(new Setter(Border.BorderThicknessProperty,
                new DynamicResourceExtension("XY.Border.Width.Default")));
        }
        return style;
    }

    private static Style ShadowStyle(string cls, string shadowKey)
    {
        var style = new Style(x => x.OfType<Border>().Class(cls));
        style.Setters.Add(new Setter(Border.BoxShadowProperty, new DynamicResourceExtension(shadowKey)));
        return style;
    }
}

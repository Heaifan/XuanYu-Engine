using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;
using XYUI.Avalonia.Typography;

namespace XYUI.Avalonia.Controls;

public static partial class XyuiComponentStyles
{
    public static Styles Create()
    {
        var styles = new Styles();
        AddTypography(styles);
        AddSurfaces(styles);
        AddSemantic(styles);
        AddXYUI3(styles);
        return styles;
    }

    static Style Text(Type type, string cls, string family, double size, int weight, double line, string brush)
    {
        var style = new Style(x => x.OfType(type).Class(cls));
        style.Setters.Add(new Setter(TextBlock.FontFamilyProperty, new FontFamily(family)));
        style.Setters.Add(new Setter(TextBlock.FontSizeProperty, size));
        style.Setters.Add(new Setter(TextBlock.FontWeightProperty, Weight(weight)));
        style.Setters.Add(new Setter(TextBlock.LineHeightProperty, line));
        style.Setters.Add(new Setter(TextBlock.ForegroundProperty, new DynamicResourceExtension(brush)));
        return style;
    }

    static Style SurfaceText(string cls, string family, double size, int weight, double line, string brush)
    {
        var style = Text(typeof(TextBlock), $"{cls}-text", family, size, weight, line, brush);
        return style;
    }

    static FontWeight Weight(int value) => value switch { 500 => FontWeight.Medium, 600 => FontWeight.SemiBold, 700 => FontWeight.Bold, _ => FontWeight.Normal };
    static void Brush(Style style, AvaloniaProperty property, string key) => style.Setters.Add(new Setter(property, new DynamicResourceExtension(key)));
}

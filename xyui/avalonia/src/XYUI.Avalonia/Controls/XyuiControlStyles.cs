using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;

namespace XYUI.Avalonia.Controls;

public static partial class XyuiControlStyles
{
    public static Styles Create()
    {
        var styles = new Styles();
        AddButtonFamily(styles);
        AddInput(styles);
        AddCheckBox(styles);
        return styles;
    }

    static void AddInput(Styles styles)
    {
        var selector = new Style(x => x.OfType<XYTextField>().Class("xyui-text-field"));
        Set(selector, TemplatedControl.BackgroundProperty, "XY.Brush.Surface.Input");
        Set(selector, TemplatedControl.ForegroundProperty, "XY.Brush.Text.Primary");
        Set(selector, TemplatedControl.BorderBrushProperty, "XY.Brush.Border.Color.Default");
        selector.Setters.Add(new Setter(TemplatedControl.BorderThicknessProperty, new Thickness(1)));
        selector.Setters.Add(new Setter(TemplatedControl.CornerRadiusProperty, new CornerRadius(4)));
        selector.Setters.Add(new Setter(Control.HeightProperty, 34d));
        styles.Add(selector);
        State(styles, typeof(XYTextField), "xyui-text-field", ":focus",
            TemplatedControl.BorderBrushProperty, "XY.Brush.Border.Color.Focus");
    }

    static void AddCheckBox(Styles styles)
    {
        var selector = new Style(x => x.OfType<XYCheckbox>().Class("xyui-checkbox"));
        Set(selector, TemplatedControl.ForegroundProperty, "XY.Brush.Text.Primary");
        Set(selector, TemplatedControl.BorderBrushProperty, "XY.Brush.Border.Color.Default");
        styles.Add(selector);
        State(styles, typeof(XYCheckbox), "xyui-checkbox", ":checked",
            TemplatedControl.BorderBrushProperty, "XY.Brush.Border.Color.Selected");
        State(styles, typeof(XYCheckbox), "xyui-checkbox", ":focus",
            TemplatedControl.BorderBrushProperty, "XY.Brush.Border.Color.Focus");
    }

    static void State(Styles styles, Type type, string cls, string state,
        AvaloniaProperty property, string resource)
    {
        var style = new Style(x => x.OfType(type).Class(cls).Class(state));
        Set(style, property, resource);
        styles.Add(style);
    }

    static void Set(Style style, AvaloniaProperty property, string resource) =>
        style.Setters.Add(new Setter(property, new DynamicResourceExtension(resource)));
}

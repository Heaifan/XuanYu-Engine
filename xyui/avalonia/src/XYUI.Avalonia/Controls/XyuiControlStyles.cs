using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;

namespace XYUI.Avalonia.Controls;

public static class XyuiControlStyles
{
    public static Styles Create()
    {
        var styles = new Styles();
        AddButton(styles, "xyui-button", typeof(XYButton));
        AddButton(styles, "xyui-icon-button", typeof(XYIconButton));
        AddToggle(styles);
        AddInput(styles);
        AddCheckBox(styles);
        return styles;
    }

    static void AddButton(Styles styles, string cls, Type type)
    {
        var selector = new Style(x => x.OfType(type).Class(cls));
        Set(selector, TemplatedControl.BackgroundProperty, "XY.Brush.Surface.Raised");
        Set(selector, TemplatedControl.ForegroundProperty, "XY.Brush.Text.Primary");
        Set(selector, TemplatedControl.BorderBrushProperty, "XY.Brush.Border.Color.Default");
        selector.Setters.Add(new Setter(TemplatedControl.BorderThicknessProperty, new Thickness(1)));
        selector.Setters.Add(new Setter(TemplatedControl.CornerRadiusProperty, new CornerRadius(4)));
        selector.Setters.Add(new Setter(Control.HeightProperty, 34d));
        styles.Add(selector);
        State(styles, type, cls, ":pointerover", TemplatedControl.BackgroundProperty, "XY.Brush.State.Color.Hover");
        State(styles, type, cls, ":pressed", TemplatedControl.BackgroundProperty, "XY.Brush.State.Color.Pressed");
        State(styles, type, cls, ":disabled", TemplatedControl.ForegroundProperty, "XY.Brush.State.Disabled.Text");
        State(styles, type, cls, ":focus", TemplatedControl.BorderBrushProperty, "XY.Brush.Border.Color.Focus");
    }

    static void AddToggle(Styles styles)
    {
        var selector = new Style(x => x.OfType<XYToggleButton>().Class("xyui-toggle-button"));
        Set(selector, TemplatedControl.BackgroundProperty, "XY.Brush.Surface.Raised");
        Set(selector, TemplatedControl.ForegroundProperty, "XY.Brush.Text.Primary");
        Set(selector, TemplatedControl.BorderBrushProperty, "XY.Brush.Border.Color.Default");
        selector.Setters.Add(new Setter(TemplatedControl.BorderThicknessProperty, new Thickness(1)));
        selector.Setters.Add(new Setter(TemplatedControl.CornerRadiusProperty, new CornerRadius(4)));
        selector.Setters.Add(new Setter(Control.HeightProperty, 34d));
        styles.Add(selector);
        State(styles, typeof(XYToggleButton), "xyui-toggle-button", ":checked",
            TemplatedControl.BackgroundProperty, "XY.Brush.State.Color.Active");
        State(styles, typeof(XYToggleButton), "xyui-toggle-button", ":pointerover",
            TemplatedControl.BackgroundProperty, "XY.Brush.State.Color.Hover");
        State(styles, typeof(XYToggleButton), "xyui-toggle-button", ":focus",
            TemplatedControl.BorderBrushProperty, "XY.Brush.Border.Color.Focus");
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

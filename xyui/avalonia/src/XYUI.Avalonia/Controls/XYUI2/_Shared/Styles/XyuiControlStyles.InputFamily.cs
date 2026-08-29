using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Styling;

namespace XYUI.Avalonia.Controls;

public static partial class XyuiControlStyles
{
    static void AddInputFamily(Styles styles)
    {
        TextField(styles); NumberField(styles); TextArea(styles);
    }

    static void TextField(Styles styles)
    {
        var root = Input(styles, typeof(XYTextField), "xyui-text-field", 32);
        root.Setters.Add(new Setter(TemplatedControl.TemplateProperty, XYTextField.CreateTemplate()));
        root.Setters.Add(new Setter(TextBox.TextAlignmentProperty, TextAlignment.Left));
        Set(root, TextBox.SelectionBrushProperty, "XY.Brush.Editor.Selection");
        Set(root, TextBox.SelectionForegroundBrushProperty, "XY.Brush.Surface.Raised");
        Set(root, TemplatedControl.FontFamilyProperty, "XY.Font.UI"); Set(root, TemplatedControl.FontSizeProperty, "XY.FontSize.Body"); Set(root, TemplatedControl.FontWeightProperty, "XY.FontWeight.Normal");
        Set(root, TextBox.CaretBrushProperty, "XY.Brush.Accent.Strong");
        State(styles, typeof(XYTextField), "xyui-text-field", ":focus", TemplatedControl.BackgroundProperty, "XY.Brush.Surface.Raised");
        State(styles, typeof(XYTextField), "xyui-text-field", ":disabled", TemplatedControl.BackgroundProperty, "XY.Brush.State.Disabled.Background");
        State(styles, typeof(XYTextField), "xyui-text-field", ":disabled", TemplatedControl.ForegroundProperty, "XY.Brush.State.Disabled.Text");
        State(styles, typeof(XYTextField), "xyui-text-field", ":disabled", TemplatedControl.BorderBrushProperty, "XY.Brush.State.Disabled.Border");
        var edge = new Style(x => x.OfType<XYTextField>().Class("xyui-text-field").Template().Name("PART_FocusEdge"));
        edge.Setters.Add(new Setter(Control.OpacityProperty, 0d)); edge.Setters.Add(new Setter(Border.BackgroundProperty, new DynamicResourceExtension("XY.Brush.Accent.Strong"))); styles.Add(edge);
        var focusEdge = new Style(x => x.OfType<XYTextField>().Class("xyui-text-field").Class(":focus").Template().Name("PART_FocusEdge"));
        focusEdge.Setters.Add(new Setter(Control.OpacityProperty, 1d)); styles.Add(focusEdge);
        var focusPlaceholder = new Style(x => x.OfType<XYTextField>().Class("xyui-text-field").Class(":focus").Template().Name("PART_Placeholder"));
        focusPlaceholder.Setters.Add(new Setter(Control.IsVisibleProperty, false)); styles.Add(focusPlaceholder);
        var errorFocus = new Style(x => x.OfType<XYTextField>().Class("xyui-text-field").Class(":focus").Class(":error"));
        Set(errorFocus, TemplatedControl.BorderBrushProperty, "XY.Brush.Semantic.Error.Border"); styles.Add(errorFocus);
    }
    static void TextArea(Styles styles) => Input(styles, typeof(XYTextArea), "xyui-text-area", 54);

    static Style Input(Styles styles, Type type, string cls, double height)
    {
        var s = new Style(x => x.OfType(type).Class(cls));
        Set(s, TemplatedControl.BackgroundProperty, "XY.Brush.Surface.Input"); Set(s, TemplatedControl.ForegroundProperty, "XY.Brush.Text.Primary");
        Set(s, TemplatedControl.BorderBrushProperty, "XY.Brush.Border.Color.Default"); s.Setters.Add(new Setter(TemplatedControl.BorderThicknessProperty, new Thickness(1)));
        s.Setters.Add(new Setter(TemplatedControl.CornerRadiusProperty, new CornerRadius(3))); s.Setters.Add(new Setter(Control.HeightProperty, height)); styles.Add(s);
        State(styles, type, cls, ":focus", TemplatedControl.BorderBrushProperty, "XY.Brush.Border.Color.Focus");
        State(styles, type, cls, ":error", TemplatedControl.BorderBrushProperty, "XY.Brush.Semantic.Error.Border");
        State(styles, type, cls, ":readonly", TemplatedControl.BackgroundProperty, "XY.Brush.Surface.PanelAlt");
        return s;
    }
}

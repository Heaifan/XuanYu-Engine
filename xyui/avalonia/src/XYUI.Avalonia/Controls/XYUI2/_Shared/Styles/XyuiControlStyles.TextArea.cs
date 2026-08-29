using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Styling;

namespace XYUI.Avalonia.Controls;

public static partial class XyuiControlStyles
{
    static void TextArea(Styles styles)
    {
        var root = Input(styles, typeof(XYTextArea), "xyui-text-area", 54); root.Setters.Add(new Setter(TemplatedControl.TemplateProperty, XYTextArea.CreateTemplate())); Set(root, TemplatedControl.FontFamilyProperty, "XY.Font.UI"); Set(root, TemplatedControl.FontSizeProperty, "XY.FontSize.Body"); Set(root, TextBox.SelectionBrushProperty, "XY.Brush.Editor.Selection"); Set(root, TextBox.SelectionForegroundBrushProperty, "XY.Brush.Surface.Raised"); Set(root, TextBox.CaretBrushProperty, "XY.Brush.Accent.Strong");
        var bar = new Style(x => x.OfType<XYTextArea>().Template().Name("PART_EditorBar")); Set(bar, Border.BackgroundProperty, "XY.Brush.Surface.PanelAlt"); Set(bar, TemplatedControl.ForegroundProperty, "XY.Brush.Text.Secondary"); styles.Add(bar);
        var type = new Style(x => x.OfType<XYTextArea>().Template().Name("PART_EditorType")); Set(type, TemplatedControl.ForegroundProperty, "XY.Brush.Text.Primary"); styles.Add(type);
        var placeholder = new Style(x => x.OfType<XYTextArea>().Template().Name("PART_Placeholder")); Set(placeholder, TextBlock.ForegroundProperty, "XY.Brush.Text.Placeholder"); styles.Add(placeholder);
        var edge = new Style(x => x.OfType<XYTextArea>().Class("xyui-text-area").Template().Name("PART_FocusEdge")); edge.Setters.Add(new Setter(Control.OpacityProperty, 0d)); edge.Setters.Add(new Setter(Border.BackgroundProperty, new DynamicResourceExtension("XY.Brush.Accent.Strong"))); styles.Add(edge);
        var focusEdge = new Style(x => x.OfType<XYTextArea>().Class("xyui-text-area").Class(":focus").Template().Name("PART_FocusEdge")); focusEdge.Setters.Add(new Setter(Control.OpacityProperty, 1d)); styles.Add(focusEdge);
        State(styles, typeof(XYTextArea), "xyui-text-area", ":focus", TemplatedControl.BorderBrushProperty, "XY.Brush.Border.Color.Focus"); State(styles, typeof(XYTextArea), "xyui-text-area", ":disabled", TemplatedControl.BackgroundProperty, "XY.Brush.State.Disabled.Background"); State(styles, typeof(XYTextArea), "xyui-text-area", ":disabled", TemplatedControl.ForegroundProperty, "XY.Brush.State.Disabled.Text"); State(styles, typeof(XYTextArea), "xyui-text-area", ":disabled", TemplatedControl.BorderBrushProperty, "XY.Brush.State.Disabled.Border"); State(styles, typeof(XYTextArea), "xyui-text-area", ":error", TemplatedControl.BorderBrushProperty, "XY.Brush.Semantic.Error.Border");
        var errorEdge = new Style(x => x.OfType<XYTextArea>().Class("xyui-text-area").Class(":error").Template().Name("PART_FocusEdge")); errorEdge.Setters.Add(new Setter(Border.BackgroundProperty, new DynamicResourceExtension("XY.Brush.Semantic.Error.Border"))); styles.Add(errorEdge);
    }
}

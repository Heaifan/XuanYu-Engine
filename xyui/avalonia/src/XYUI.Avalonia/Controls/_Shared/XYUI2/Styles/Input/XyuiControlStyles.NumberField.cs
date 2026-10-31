using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Styling;

namespace XYUI.Avalonia.Controls;

public static partial class XyuiControlStyles
{
    static void NumberField(Styles styles)
    {
        var root = Input(styles, typeof(XYNumberField), "xyui-number-field", 32);
        root.Setters.Add(new Setter(TemplatedControl.TemplateProperty, XYNumberField.CreateNumberTemplate()));
        root.Setters.Add(new Setter(TextBox.TextAlignmentProperty, TextAlignment.Left));
        Set(root, TemplatedControl.FontFamilyProperty, "XY.Font.UI"); Set(root, TemplatedControl.FontSizeProperty, "XY.FontSize.Body"); Set(root, TextBox.CaretBrushProperty, "XY.Brush.Accent.Strong");
        Set(root, TextBox.SelectionBrushProperty, "XY.Brush.Editor.Selection"); Set(root, TextBox.SelectionForegroundBrushProperty, "XY.Brush.Surface.Raised");
        State(styles, typeof(XYNumberField), "xyui-number-field", ":focus", TemplatedControl.BackgroundProperty, "XY.Brush.Surface.Raised");
        State(styles, typeof(XYNumberField), "xyui-number-field", ":focus", TemplatedControl.BorderBrushProperty, "XY.Brush.Border.Color.Default");
        var cell = new Style(x => x.OfType<XYNumberField>().Class("xyui-number-field").Template().Name("PART_StepperCell"));
        cell.Setters.Add(new Setter(Control.OpacityProperty, 0d)); cell.Setters.Add(new Setter(Control.IsHitTestVisibleProperty, false)); cell.Setters.Add(new Setter(Border.BackgroundProperty, new DynamicResourceExtension("XY.Brush.Surface.PanelAlt"))); styles.Add(cell);
        var visible = new Style(x => x.OfType<XYNumberField>().Class("xyui-number-field").Class(":pointerover").Template().Name("PART_StepperCell")); visible.Setters.Add(new Setter(Control.OpacityProperty, 1d)); visible.Setters.Add(new Setter(Control.IsHitTestVisibleProperty, true)); visible.Setters.Add(new Setter(Border.BackgroundProperty, new DynamicResourceExtension("XY.Brush.State.Color.Hover"))); styles.Add(visible);
        var focused = new Style(x => x.OfType<XYNumberField>().Class("xyui-number-field").Class(":focus").Template().Name("PART_StepperCell")); focused.Setters.Add(new Setter(Control.OpacityProperty, 1d)); focused.Setters.Add(new Setter(Control.IsHitTestVisibleProperty, true)); styles.Add(focused);
        var buttons = new Style(x => x.OfType<XYNumberField>().Class("xyui-number-field").Template().OfType<Button>());
        Set(buttons, TemplatedControl.ForegroundProperty, "XY.Brush.Text.Secondary"); buttons.Setters.Add(new Setter(TemplatedControl.BackgroundProperty, Brushes.Transparent)); buttons.Setters.Add(new Setter(TemplatedControl.BorderBrushProperty, Brushes.Transparent)); styles.Add(buttons);
        var edge = new Style(x => x.OfType<XYNumberField>().Class("xyui-number-field").Template().Name("PART_FocusEdge")); edge.Setters.Add(new Setter(Control.OpacityProperty, 0d)); edge.Setters.Add(new Setter(Border.BackgroundProperty, new DynamicResourceExtension("XY.Brush.Accent.Strong"))); styles.Add(edge);
        var focusEdge = new Style(x => x.OfType<XYNumberField>().Class("xyui-number-field").Class(":focus").Template().Name("PART_FocusEdge")); focusEdge.Setters.Add(new Setter(Control.OpacityProperty, 1d)); styles.Add(focusEdge);
    }
}

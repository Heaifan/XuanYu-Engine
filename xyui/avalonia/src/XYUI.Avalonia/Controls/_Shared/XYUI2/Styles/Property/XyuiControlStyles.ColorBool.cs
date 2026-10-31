using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Styling;

namespace XYUI.Avalonia.Controls;

public static partial class XyuiControlStyles
{
    static void ColorPicker(Styles styles)
    {
        var root = Input(styles, typeof(XYColorPicker), "xyui-color-picker", 32, false); root.Setters.Add(new Setter(TemplatedControl.TemplateProperty, XYColorPicker.CreateTemplate())); Typography(root); InputStates(styles, typeof(XYColorPicker), "xyui-color-picker"); FocusEdge(styles, typeof(XYColorPicker), "xyui-color-picker");
        var field = new Style(x => x.OfType<XYColorPicker>().Class("xyui-color-picker").Template().Name("PART_FieldButton")); field.Setters.Add(new Setter(Button.TemplateProperty, ActionCellTemplate())); field.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.Transparent)); field.Setters.Add(new Setter(Button.BorderThicknessProperty, new Thickness(0))); styles.Add(field);
        var light = new Style(x => x.OfType<XYColorPicker>().Template().Name("PART_Swatch").Descendant().Class("xyui-checker-light")); Set(light, Border.BackgroundProperty, "XY.Brush.Surface.Raised"); styles.Add(light);
        var dark = new Style(x => x.OfType<XYColorPicker>().Template().Name("PART_Swatch").Descendant().Class("xyui-checker-dark")); Set(dark, Border.BackgroundProperty, "XY.Brush.Border.Color.Subtle"); styles.Add(dark);
        PanelStyle(styles, typeof(XYColorPicker), "xyui-color-picker", "PART_ColorPanelSurface");
    }
    static void BoolProperty(Styles styles)
    {
        var root = new Style(x => x.OfType<XYBoolProperty>().Class("xyui-bool-property")); Set(root, TemplatedControl.ForegroundProperty, "XY.Brush.Text.Primary"); root.Setters.Add(new Setter(Control.HeightProperty, XyuiComponentTokens.BoolPropertyRowHeight)); root.Setters.Add(new Setter(TemplatedControl.TemplateProperty, XYBoolProperty.CreateTemplate())); root.Setters.Add(new Setter(TemplatedControl.FontSizeProperty, new DynamicResourceExtension("XY.FontSize.Body"))); root.Setters.Add(new Setter(TemplatedControl.FontFamilyProperty, new DynamicResourceExtension("XY.Font.UI"))); styles.Add(root);
        var hover = new Style(x => x.OfType<XYBoolProperty>().Class("xyui-bool-property").Class(":pointerover")); Set(hover, TemplatedControl.BackgroundProperty, "XY.Brush.State.Color.Hover"); styles.Add(hover);
        var label = new Style(x => x.OfType<XYBoolProperty>().Template().Name("PART_Label")); Set(label, TextBlock.ForegroundProperty, "XY.Brush.Text.Primary"); styles.Add(label);
    }
    static void PanelStyle(Styles styles, Type type, string cls, string name) { var panel = new Style(x => x.OfType(type).Class(cls).Template().Name(name)); Set(panel, Border.BackgroundProperty, "XY.Brush.Surface.PanelAlt"); Set(panel, Border.BorderBrushProperty, "XY.Brush.Border.Color.Default"); panel.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(1))); panel.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(3))); styles.Add(panel); }
}

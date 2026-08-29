using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Styling;

namespace XYUI.Avalonia.Controls;

public static partial class XyuiControlStyles
{
    static void PropertyControls(Styles styles) { NumberProperty(styles); VectorProperty(styles); EnumProperty(styles); ReferenceProperty(styles); }
    static void NumberProperty(Styles styles)
    {
        var root = PropertyRoot<XYNumberProperty>(styles, "xyui-number-property", XYNumberProperty.CreateTemplate(), double.NaN);
        var label = TemplateText<XYNumberProperty>(styles, "xyui-number-property", "PART_LabelText");
        var active = new Style(x => x.OfType<XYNumberProperty>().Class("xyui-number-property-scrubbing").Template().Name("PART_Label")); Set(active, Border.BackgroundProperty, "XY.Brush.State.Color.Hover"); styles.Add(active);
    }
    static void VectorProperty(Styles styles)
    {
        PropertyRoot<XYVectorProperty>(styles, "xyui-vector-property", XYVectorProperty.CreateTemplate(), double.NaN);
        TemplateText<XYVectorProperty>(styles, "xyui-vector-property", "PART_Label");
        var host = new Style(x => x.OfType<XYVectorProperty>().Class("xyui-vector-property").Template().Name("PART_Axes").Descendant().Class("xyui-vector-axis-host")); Set(host, Border.BackgroundProperty, "XY.Brush.Surface.Input"); Set(host, Border.BorderBrushProperty, "XY.Brush.Border.Default"); host.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(1))); host.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(6))); styles.Add(host);
        var cell = new Style(x => x.OfType<XYVectorProperty>().Class("xyui-vector-property").Template().Name("PART_Axes").Descendant().Class("xyui-vector-axis-cell")); Set(cell, Border.BackgroundProperty, "XY.Brush.Surface.PanelAlt"); styles.Add(cell);
    }
    static void EnumProperty(Styles styles)
    {
        PropertyRoot<XYEnumProperty>(styles, "xyui-enum-property", XYEnumProperty.CreateTemplate(), double.NaN);
        TemplateText<XYEnumProperty>(styles, "xyui-enum-property", "PART_Label");
    }
    static void ReferenceProperty(Styles styles)
    {
        PropertyRoot<XYReferenceProperty>(styles, "xyui-reference-property", XYReferenceProperty.CreateTemplate(), double.NaN);
        TemplateText<XYReferenceProperty>(styles, "xyui-reference-property", "PART_Label");
        var identity = new Style(x => x.OfType<XYReferenceProperty>().Class("xyui-reference-property").Template().Name("PART_Identity")); Set(identity, TextBlock.ForegroundProperty, "XY.Brush.Text.Tertiary"); styles.Add(identity);
        var field = new Style(x => x.OfType<XYReferenceProperty>().Class("xyui-reference-property").Template().Name("PART_ReferenceField")); Set(field, Border.BackgroundProperty, "XY.Brush.Surface.Panel"); Set(field, Border.BorderBrushProperty, "XY.Brush.Border.Default"); field.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(1))); field.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(3))); styles.Add(field);
        State(styles, typeof(XYReferenceProperty), "xyui-reference-property", ":disabled", TemplatedControl.ForegroundProperty, "XY.Brush.State.Disabled.Text");
        var missing = new Style(x => x.OfType<XYReferenceProperty>().Class("xyui-reference-missing").Template().Name("PART_ReferenceField")); Set(missing, Border.BorderBrushProperty, "XY.Brush.Semantic.Error.Border"); styles.Add(missing);
        var mismatch = new Style(x => x.OfType<XYReferenceProperty>().Class("xyui-reference-mismatch").Template().Name("PART_ReferenceField")); Set(mismatch, Border.BorderBrushProperty, "XY.Brush.Semantic.Error.Border"); styles.Add(mismatch);
    }
    static Style PropertyRoot<T>(Styles styles, string cls, IControlTemplate template, double height) where T : TemplatedControl
    {
        var root = new Style(x => x.OfType<T>().Class(cls)); Set(root, TemplatedControl.ForegroundProperty, "XY.Brush.Text.Primary"); Set(root, TemplatedControl.FontFamilyProperty, "XY.Font.UI"); Set(root, TemplatedControl.FontSizeProperty, "XY.FontSize.Body"); root.Setters.Add(new Setter(Control.HeightProperty, height)); root.Setters.Add(new Setter(TemplatedControl.TemplateProperty, template)); styles.Add(root); return root;
    }
    static Style TemplateText<T>(Styles styles, string cls, string name) where T : TemplatedControl
    {
        var style = new Style(x => x.OfType<T>().Class(cls).Template().Name(name)); Set(style, TextBlock.ForegroundProperty, "XY.Brush.Text.Primary"); styles.Add(style); return style;
    }
}

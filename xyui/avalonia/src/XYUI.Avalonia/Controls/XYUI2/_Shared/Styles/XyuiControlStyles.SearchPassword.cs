using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Styling;

namespace XYUI.Avalonia.Controls;

public static partial class XyuiControlStyles
{
    static void SearchField(Styles styles)
    {
        var root = Input(styles, typeof(XYSearchField), "xyui-search-field", 32, false); root.Setters.Add(new Setter(TemplatedControl.TemplateProperty, XYSearchField.CreateTemplate())); Set(root, TemplatedControl.FontFamilyProperty, "XY.Font.UI"); Set(root, TemplatedControl.FontSizeProperty, "XY.FontSize.Body"); Set(root, TextBox.CaretBrushProperty, "XY.Brush.Accent.Strong"); Set(root, TextBox.SelectionBrushProperty, "XY.Brush.Editor.Selection"); Set(root, TextBox.SelectionForegroundBrushProperty, "XY.Brush.Surface.Raised");
        State(styles, typeof(XYSearchField), "xyui-search-field", ":focus", TemplatedControl.BackgroundProperty, "XY.Brush.Surface.Raised");
        IconPart(styles, typeof(XYSearchField), "PART_SearchGlyph", "XY.Brush.Text.Secondary"); IconPart(styles, typeof(XYSearchField), "PART_ClearGlyph", "XY.Brush.Text.Secondary"); IconPart(styles, typeof(XYSearchField), "PART_FilterGlyph", "XY.Brush.Text.Secondary"); SearchPart(styles, "PART_Placeholder", "XY.Brush.Text.Placeholder");
        var activeGlyph = new Style(x => x.OfType<XYSearchField>().Class("xyui-search-field").Class(":focus").Template().Name("PART_SearchGlyph")); Set(activeGlyph, XYIcon.StrokeProperty, "XY.Brush.Accent.Strong"); styles.Add(activeGlyph);
        var activeFilter = new Style(x => x.OfType<XYSearchField>().Class("xyui-search-filter-active").Template().Name("PART_FilterCell")); Set(activeFilter, Button.BackgroundProperty, "XY.Brush.State.Color.Active"); styles.Add(activeFilter);
        var hoverFilter = new Style(x => x.OfType<XYSearchField>().Template().Name("PART_FilterCell").Class(":pointerover")); Set(hoverFilter, Button.BackgroundProperty, "XY.Brush.State.Color.Hover"); styles.Add(hoverFilter);
        var filterCell = new Style(x => x.OfType<XYSearchField>().Template().Name("PART_FilterCell")); Set(filterCell, Button.BackgroundProperty, "XY.Brush.Surface.PanelAlt"); styles.Add(filterCell);
        SearchEdge(styles, typeof(XYSearchField), "xyui-search-field"); State(styles, typeof(XYSearchField), "xyui-search-field", ":disabled", TemplatedControl.BackgroundProperty, "XY.Brush.State.Disabled.Background"); State(styles, typeof(XYSearchField), "xyui-search-field", ":disabled", TemplatedControl.ForegroundProperty, "XY.Brush.State.Disabled.Text"); State(styles, typeof(XYSearchField), "xyui-search-field", ":disabled", TemplatedControl.BorderBrushProperty, "XY.Brush.State.Disabled.Border");
    }

    static void PasswordField(Styles styles)
    {
        var root = Input(styles, typeof(XYPasswordField), "xyui-password-field", 32, false); root.Setters.Add(new Setter(TemplatedControl.TemplateProperty, XYPasswordField.CreateTemplate())); Set(root, TemplatedControl.FontFamilyProperty, "XY.Font.UI"); Set(root, TemplatedControl.FontSizeProperty, "XY.FontSize.Body"); Set(root, TextBox.CaretBrushProperty, "XY.Brush.Accent.Strong"); Set(root, TextBox.SelectionBrushProperty, "XY.Brush.Editor.Selection"); Set(root, TextBox.SelectionForegroundBrushProperty, "XY.Brush.Surface.Raised"); State(styles, typeof(XYPasswordField), "xyui-password-field", ":focus", TemplatedControl.BackgroundProperty, "XY.Brush.Surface.Raised");
        IconPart(styles, typeof(XYPasswordField), "PART_EyeGlyph", "XY.Brush.Text.Secondary"); PasswordPart(styles, "PART_Placeholder", "XY.Brush.Text.Placeholder"); var cell = new Style(x => x.OfType<XYPasswordField>().Template().Name("PART_RevealCell")); Set(cell, Button.BackgroundProperty, "XY.Brush.Surface.PanelAlt"); styles.Add(cell);
        var holding = new Style(x => x.OfType<XYPasswordField>().Class("xyui-password-holding").Template().Name("PART_RevealCell")); Set(holding, Button.BackgroundProperty, "XY.Brush.State.Color.Pressed"); styles.Add(holding); var activeEye = new Style(x => x.OfType<XYPasswordField>().Class("xyui-password-holding").Template().Name("PART_EyeGlyph")); Set(activeEye, XYIcon.StrokeProperty, "XY.Brush.Accent.Strong"); styles.Add(activeEye);
        SearchEdge(styles, typeof(XYPasswordField), "xyui-password-field"); State(styles, typeof(XYPasswordField), "xyui-password-field", ":disabled", TemplatedControl.BackgroundProperty, "XY.Brush.State.Disabled.Background"); State(styles, typeof(XYPasswordField), "xyui-password-field", ":disabled", TemplatedControl.ForegroundProperty, "XY.Brush.State.Disabled.Text"); State(styles, typeof(XYPasswordField), "xyui-password-field", ":disabled", TemplatedControl.BorderBrushProperty, "XY.Brush.State.Disabled.Border");
    }

    static void IconPart(Styles styles, Type type, string name, string resource) { var style = new Style(x => x.OfType(type).Template().Name(name)); Set(style, XYIcon.StrokeProperty, resource); styles.Add(style); }
    static void SearchPart(Styles styles, string name, string resource) { var style = new Style(x => x.OfType<XYSearchField>().Template().Name(name)); Set(style, TextBlock.ForegroundProperty, resource); styles.Add(style); }
    static void PasswordPart(Styles styles, string name, string resource) { var style = new Style(x => x.OfType<XYPasswordField>().Template().Name(name)); if (name == "PART_Placeholder") Set(style, TextBlock.ForegroundProperty, resource); else Set(style, XYIcon.StrokeProperty, resource); styles.Add(style); }
    static void SearchEdge(Styles styles, Type type, string cls) { var edge = new Style(x => x.OfType(type).Class(cls).Template().Name("PART_FocusEdge")); edge.Setters.Add(new Setter(Control.OpacityProperty, 0d)); Set(edge, Border.BackgroundProperty, "XY.Brush.Accent.Strong"); styles.Add(edge); var focus = new Style(x => x.OfType(type).Class(cls).Class(":focus").Template().Name("PART_FocusEdge")); focus.Setters.Add(new Setter(Control.OpacityProperty, 1d)); styles.Add(focus); }
}

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;

namespace XYUI.Avalonia.Controls;

public static partial class XyuiControlStyles
{
    static void ComboBox(Styles styles)
    {
        var root = new Style(x => x.OfType<XYComboBox>().Class("xyui-combo-box"));
        root.Setters.Add(new Setter(TemplatedControl.TemplateProperty, XYComboBox.CreateTemplate())); Set(root, Control.HeightProperty, "XY.Input.Height"); Set(root, TemplatedControl.ForegroundProperty, "XY.Brush.Text.Primary"); Set(root, TemplatedControl.BackgroundProperty, "XY.Brush.Surface.Input"); Set(root, TemplatedControl.BorderBrushProperty, "XY.Brush.Border.Color.Default"); root.Setters.Add(new Setter(TemplatedControl.BorderThicknessProperty, new Thickness(1))); root.Setters.Add(new Setter(TemplatedControl.CornerRadiusProperty, new CornerRadius(3))); styles.Add(root);
        var embedded = new Style(x => x.OfType<XYTextField>().Class("xyui-combo-embedded")); embedded.Setters.Add(new Setter(TemplatedControl.BackgroundProperty, Brushes.Transparent)); embedded.Setters.Add(new Setter(TemplatedControl.BorderBrushProperty, Brushes.Transparent)); embedded.Setters.Add(new Setter(TemplatedControl.BorderThicknessProperty, new Thickness(0))); embedded.Setters.Add(new Setter(TemplatedControl.CornerRadiusProperty, new CornerRadius(0))); styles.Add(embedded);
        var embeddedEdge = new Style(x => x.OfType<XYTextField>().Class("xyui-combo-embedded").Template().Name("PART_FocusEdge")); embeddedEdge.Setters.Add(new Setter(Control.OpacityProperty, 0d)); styles.Add(embeddedEdge);
        var cell = new Style(x => x.OfType<XYComboBox>().Template().Name("PART_ChevronCell")); Set(cell, Border.BackgroundProperty, "XY.Brush.Surface.PanelAlt"); styles.Add(cell);
        var chevron = new Style(x => x.OfType<XYComboBox>().Template().Name("PART_Chevron")); chevron.Setters.Add(new Setter(TemplatedControl.BackgroundProperty, Brushes.Transparent)); Set(chevron, TemplatedControl.ForegroundProperty, "XY.Brush.Text.Secondary"); chevron.Setters.Add(new Setter(Control.WidthProperty, 32d)); styles.Add(chevron);
        var hover = new Style(x => x.OfType<XYComboBox>().Class("xyui-combo-box").Class(":pointerover").Template().Name("PART_ChevronCell")); Set(hover, Border.BackgroundProperty, "XY.Brush.State.Color.Hover"); styles.Add(hover);
        var open = new Style(x => x.OfType<XYComboBox>().Class("xyui-combo-open").Template().Name("PART_ChevronCell")); Set(open, Border.BackgroundProperty, "XY.Brush.State.Color.Hover"); styles.Add(open);
        var edge = new Style(x => x.OfType<XYComboBox>().Class("xyui-combo-box").Template().Name("PART_FocusEdge")); edge.Setters.Add(new Setter(Control.OpacityProperty, 0d)); edge.Setters.Add(new Setter(Border.BackgroundProperty, new DynamicResourceExtension("XY.Brush.Accent.Strong"))); styles.Add(edge);
        var focusEdge = new Style(x => x.OfType<XYComboBox>().Class("xyui-combo-box").Class(":focus").Template().Name("PART_FocusEdge")); focusEdge.Setters.Add(new Setter(Control.OpacityProperty, 1d)); styles.Add(focusEdge);
        State(styles, typeof(XYComboBox), "xyui-combo-box", ":focus", TemplatedControl.BorderBrushProperty, "XY.Brush.Border.Color.Focus"); State(styles, typeof(XYComboBox), "xyui-combo-box", ":error", TemplatedControl.BorderBrushProperty, "XY.Brush.Semantic.Error.Border");
        var list = new Style(x => x.OfType<XYComboBox>().Template().Name("PART_List")); list.Setters.Add(new Setter(Control.MinHeightProperty, 28d)); list.Setters.Add(new Setter(Control.MaxHeightProperty, 240d)); Set(list, TemplatedControl.BackgroundProperty, "XY.Brush.Surface.Raised"); styles.Add(list);
        var popup = new Style(x => x.OfType<XYComboBox>().Template().Name("PART_PopupSurface")); Set(popup, Border.BackgroundProperty, "XY.Brush.Surface.Raised"); Set(popup, Border.BorderBrushProperty, "XY.Brush.Border.Color.Default"); popup.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(1))); popup.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(3))); styles.Add(popup);
        var item = new Style(x => x.OfType<XYComboBox>().Template().Name("PART_List").Descendant().OfType<ListBoxItem>()); item.Setters.Add(new Setter(Control.MinHeightProperty, 28d)); item.Setters.Add(new Setter(TemplatedControl.PaddingProperty, new Thickness(8, 0))); Set(item, TemplatedControl.ForegroundProperty, "XY.Brush.Text.Primary"); styles.Add(item);
        var itemHover = new Style(x => x.OfType<XYComboBox>().Template().Name("PART_List").Descendant().OfType<ListBoxItem>().Class(":pointerover")); Set(itemHover, TemplatedControl.BackgroundProperty, "XY.Brush.State.Color.Hover"); styles.Add(itemHover);
    }
}

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Styling;

namespace XYUI.Avalonia.Controls;

public static partial class XyuiControlStyles
{
    static void Select(Styles styles)
    {
        var root = new Style(x => x.OfType<XYSelect>().Class("xyui-select")); root.Setters.Add(new Setter(TemplatedControl.TemplateProperty, XYSelect.CreateTemplate())); Set(root, Control.HeightProperty, "XY.Select.Height"); Set(root, TemplatedControl.ForegroundProperty, "XY.Brush.Text.Primary"); Set(root, TemplatedControl.BackgroundProperty, "XY.Brush.Surface.Input"); Set(root, TemplatedControl.BorderBrushProperty, "XY.Brush.Border.Color.Default"); root.Setters.Add(new Setter(TemplatedControl.BorderThicknessProperty, new Thickness(1))); Set(root, TemplatedControl.CornerRadiusProperty, "XY.Radius.Input"); styles.Add(root);
        var value = new Style(x => x.OfType<XYSelect>().Template().Name("PART_ValueSurface")); value.Setters.Add(new Setter(Border.BackgroundProperty, Brushes.Transparent)); styles.Add(value);
        var cell = new Style(x => x.OfType<XYSelect>().Template().Name("PART_ChevronCell")); cell.Setters.Add(new Setter(Border.WidthProperty, XyuiComponentTokens.SelectChevronSurfaceWidth)); Set(cell, Border.BackgroundProperty, "XY.Brush.Surface.PanelAlt"); styles.Add(cell);
        var hover = new Style(x => x.OfType<XYSelect>().Class("xyui-select").Class(":pointerover").Template().Name("PART_ChevronCell")); Set(hover, Border.BackgroundProperty, "XY.Brush.State.Color.Hover"); styles.Add(hover);
        var open = new Style(x => x.OfType<XYSelect>().Class("xyui-select-open").Template().Name("PART_ChevronCell")); Set(open, Border.BackgroundProperty, "XY.Brush.State.Color.Hover"); styles.Add(open);
        var pressed = new Style(x => x.OfType<XYSelect>().Class("xyui-select").Class(":pressed").Template().Name("PART_ChevronCell")); Set(pressed, Border.BackgroundProperty, "XY.Brush.State.Color.Pressed"); styles.Add(pressed);
        var icon = new Style(x => x.OfType<XYSelect>().Template().Name("PART_Chevron")); icon.Setters.Add(new Setter(XYIcon.StrokeWidthProperty, 1.25d)); styles.Add(icon);
        var edge = new Style(x => x.OfType<XYSelect>().Template().Name("PART_FocusEdge")); edge.Setters.Add(new Setter(Control.OpacityProperty, 0d)); edge.Setters.Add(new Setter(Border.BackgroundProperty, new DynamicResourceExtension("XY.Brush.Accent.Strong"))); styles.Add(edge);
        var focusEdge = new Style(x => x.OfType<XYSelect>().Class("xyui-select").Class(":focus").Template().Name("PART_FocusEdge")); focusEdge.Setters.Add(new Setter(Control.OpacityProperty, 1d)); styles.Add(focusEdge);
        var openEdge = new Style(x => x.OfType<XYSelect>().Class("xyui-select-open").Template().Name("PART_FocusEdge")); openEdge.Setters.Add(new Setter(Control.OpacityProperty, 1d)); styles.Add(openEdge);
        State(styles, typeof(XYSelect), "xyui-select", ":focus", TemplatedControl.BorderBrushProperty, "XY.Brush.Border.Color.Focus"); State(styles, typeof(XYSelect), "xyui-select", ":disabled", TemplatedControl.BackgroundProperty, "XY.Brush.State.Disabled.Background"); State(styles, typeof(XYSelect), "xyui-select", ":disabled", TemplatedControl.ForegroundProperty, "XY.Brush.State.Disabled.Text"); State(styles, typeof(XYSelect), "xyui-select", ":disabled", TemplatedControl.BorderBrushProperty, "XY.Brush.State.Disabled.Border");
        var placeholder = new Style(x => x.OfType<XYSelect>().Template().Name("PART_Value").Class("xyui-select-placeholder")); Set(placeholder, TextBlock.ForegroundProperty, "XY.Text.Placeholder"); styles.Add(placeholder);
        var list = new Style(x => x.OfType<XYSelect>().Template().Name("PART_List")); Set(list, TemplatedControl.BackgroundProperty, "XY.Brush.Surface.Raised"); list.Setters.Add(new Setter(Control.MinHeightProperty, 28d)); list.Setters.Add(new Setter(Control.MaxHeightProperty, 240d)); styles.Add(list);
        var popup = new Style(x => x.OfType<XYSelect>().Template().Name("PART_PopupSurface")); Set(popup, Border.BackgroundProperty, "XY.Brush.Surface.Raised"); Set(popup, Border.BorderBrushProperty, "XY.Brush.Border.Color.Default"); popup.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(1))); popup.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(3))); styles.Add(popup);
        var item = new Style(x => x.OfType<XYSelect>().Template().Name("PART_List").Descendant().OfType<ListBoxItem>()); item.Setters.Add(new Setter(Control.MinHeightProperty, 28d)); item.Setters.Add(new Setter(TemplatedControl.PaddingProperty, new Thickness(8, 0))); Set(item, TemplatedControl.ForegroundProperty, "XY.Brush.Text.Primary"); styles.Add(item);
        var itemHover = new Style(x => x.OfType<XYSelect>().Template().Name("PART_List").Descendant().OfType<ListBoxItem>().Class(":pointerover")); Set(itemHover, TemplatedControl.BackgroundProperty, "XY.Brush.State.Color.Hover"); styles.Add(itemHover);
        var itemSelected = new Style(x => x.OfType<XYSelect>().Template().Name("PART_List").Descendant().OfType<ListBoxItem>().Class(":selected")); Set(itemSelected, TemplatedControl.BackgroundProperty, "XY.Brush.Accent.Soft"); styles.Add(itemSelected);
    }
}

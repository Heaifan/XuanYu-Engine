using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Styling;
using VectorPath = Avalonia.Controls.Shapes.Path;

namespace XYUI.Avalonia.Controls;

public static partial class XyuiComponentStyles
{
    static void AddSelectionSemantics(Styles styles)
    {
        var text = new Style(x => x.OfType<XYSelectableText>().Class("xyui-selectable-text").Class(":disabled").Descendant().OfType<SelectableTextBlock>().Class("xyui-selectable-text-content"));
        Brush(text, TextBlock.ForegroundProperty, "XY.Brush.State.Disabled.Text"); styles.Add(text);
        var mark = new Style(x => x.OfType<XYSelectableText>().Class("xyui-selectable-text").Class(":disabled").Descendant().OfType<VectorPath>().Class("xyui-selectable-copy-mark"));
        Brush(mark, VectorPath.StrokeProperty, "XY.Brush.State.Disabled.Text"); styles.Add(mark);
        var empty = new Style(x => x.OfType<XYEmptyText>().Class("xyui-empty-text").Class(":disabled"));
        Brush(empty, TextBlock.ForegroundProperty, "XY.Brush.State.Disabled.Text"); styles.Add(empty);
    }
}

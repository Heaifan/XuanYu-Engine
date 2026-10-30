using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Styling;
using VectorPath = Avalonia.Controls.Shapes.Path;

namespace XYUI.Avalonia.Controls;

public static partial class XyuiComponentStyles
{
    static void AddResultTextSemantics(Styles styles)
    {
        var searchText = new Style(x => x.OfType<XYSearchHighlight>().Class("xyui-search-highlight").Class(":disabled").Descendant().OfType<TextBlock>().Class("xyui-search-highlight-text"));
        Brush(searchText, TextBlock.ForegroundProperty, "XY.Brush.State.Disabled.Text"); styles.Add(searchText);
        var searchMark = new Style(x => x.OfType<XYSearchHighlight>().Class("xyui-search-highlight").Class(":disabled").Descendant().OfType<VectorPath>().Class("xyui-search-highlight-mark"));
        Brush(searchMark, VectorPath.StrokeProperty, "XY.Brush.State.Disabled.Text"); styles.Add(searchMark);
        var truncated = new Style(x => x.OfType<XYTruncatedText>().Class("xyui-truncated-text").Class(":disabled"));
        Brush(truncated, TextBlock.ForegroundProperty, "XY.Brush.State.Disabled.Text"); styles.Add(truncated);
    }
}

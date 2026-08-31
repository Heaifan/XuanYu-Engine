using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;

namespace XYUI.Avalonia.Controls;

// Batch 01 Action Edge 样式辅助：变体/状态填色、显隐与 Hover 抬升（内部实现构件）。
internal static class XyuiEdgeStyles
{
    internal static void Fill(Styles styles, Func<Selector?, Selector> ancestor, string resource)
    {
        var style = new Style(x => ancestor(x).Descendant().OfType<XyuiActionEdge>());
        Key(style, Border.BackgroundProperty, resource);
        styles.Add(style);
    }

    internal static void FillState(Styles styles, Type type, string cls, string state, string resource) =>
        Fill(styles, x => x!.OfType(type).Class(cls).Class(state), resource);

    internal static void Show(Styles styles, Type type, string cls, string? state)
    {
        var style = new Style(x => Anchor(type, cls, state)(x).Descendant().OfType<XyuiActionEdge>());
        style.Setters.Add(new Setter(Visual.IsVisibleProperty, true));
        styles.Add(style);
    }

    internal static void Hide(Styles styles, Type type, string cls)
    {
        var style = new Style(x => x!.OfType(type).Class(cls).Descendant().OfType<XyuiActionEdge>());
        style.Setters.Add(new Setter(Visual.IsVisibleProperty, false));
        styles.Add(style);
    }

    internal static void HoverEdge(Styles styles, Type type, string cls)
    {
        var edge = new Style(x => x!.OfType(type).Class(cls).Class(":pointerover").Descendant().OfType<XyuiActionEdge>());
        edge.Setters.Add(new Setter(Border.HeightProperty, XyuiActionEdge.HoverHeight));
        styles.Add(edge);
    }

    internal static Func<Selector?, Selector> Anchor(Type type, string cls, string? state) => state is null
        ? (x => x!.OfType(type).Class(cls))
        : (x => x!.OfType(type).Class(cls).Class(state));

    static void Key(Style style, AvaloniaProperty property, string resource) =>
        style.Setters.Add(new Setter(property, new DynamicResourceExtension(resource)));
}

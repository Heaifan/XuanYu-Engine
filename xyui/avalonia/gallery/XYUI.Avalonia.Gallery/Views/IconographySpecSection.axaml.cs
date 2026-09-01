using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery.Views;

public partial class IconographySpecSection : UserControl
{
    public IconographySpecSection()
    {
        InitializeComponent();
        foreach (var item in Items()) Ladder.Children.Add(Card(item.Label, item.Role, item.Size));
    }

    static IEnumerable<(string Label, string Role, XyuiIconSize Size)> Items() =>
        [("14 DIP", "Compact", XyuiIconSize.Compact), ("16 DIP", "默认推荐", XyuiIconSize.Default),
         ("20 DIP", "Comfortable", XyuiIconSize.Comfortable), ("24 DIP", "Touch", XyuiIconSize.Touch)];

    static Control Card(string label, string role, XyuiIconSize size)
    {
        var icon = new XYIcon { Icon = XyuiVectorIcon.Search, Size = size, HorizontalAlignment = HorizontalAlignment.Center };
        var metrics = XyuiVectorIcons.GetMetrics(XyuiVectorIcon.Search);
        var text = new TextBlock { Text = $"{label} · {role}\n实际 Icon Size：{label}\nLogical Viewport：{metrics.LogicalViewport} DIP\nGeometry Bounds：{metrics.GeometryBounds.Width:0.#}×{metrics.GeometryBounds.Height:0.#}\nStroke：{icon.StrokeWidth:0.##} DIP", Classes = { "xyui-text-caption" }, TextAlignment = TextAlignment.Center };
        return new Border { Width = 150, Margin = new global::Avalonia.Thickness(0, 0, 8, 0), Padding = new global::Avalonia.Thickness(8), Classes = { "xyui-border-subtle" }, Child = new StackPanel { Spacing = 6, Children = { icon, text } } };
    }
}

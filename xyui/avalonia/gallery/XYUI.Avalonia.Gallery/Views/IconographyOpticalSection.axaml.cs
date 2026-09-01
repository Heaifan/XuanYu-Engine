using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery.Views;

public partial class IconographyOpticalSection : UserControl
{
    public IconographyOpticalSection()
    {
        InitializeComponent();
        foreach (var icon in Icons) Optical.Children.Add(Card(icon));
    }

    static readonly XyuiVectorIcon[] Icons = [XyuiVectorIcon.ChevronRight, XyuiVectorIcon.Eye,
        XyuiVectorIcon.Locate, XyuiVectorIcon.Code, XyuiVectorIcon.MoreHorizontal];

    static Control Card(XyuiVectorIcon icon)
    {
        var metrics = XyuiVectorIcons.GetMetrics(icon);
        var visual = new Grid { Width = 52, Height = 52, HorizontalAlignment = HorizontalAlignment.Center };
        visual.Children.Add(new Border { BorderBrush = global::Avalonia.Media.Brushes.LightGray, BorderThickness = new global::Avalonia.Thickness(1), Child = new XYIcon { Icon = icon, Size = XyuiIconSize.Default } });
        var text = new TextBlock { Text = $"{icon}\n数学中心：{metrics.GeometryBounds.Center.X:0.#},{metrics.GeometryBounds.Center.Y:0.#}\nOptical Offset：{metrics.OpticalOffset.X:0.#},{metrics.OpticalOffset.Y:0.#}\n{(metrics.HasOpticalCorrection ? "已修正" : "当前无需修正")}", Classes = { "xyui-text-caption" }, TextAlignment = TextAlignment.Center };
        return new Border { Width = 116, Margin = new global::Avalonia.Thickness(0, 0, 6, 0), Padding = new global::Avalonia.Thickness(6), Classes = { "xyui-border-subtle" }, Child = new StackPanel { Spacing = 5, Children = { visual, text } } };
    }
}

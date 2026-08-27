using Avalonia;
using Avalonia.Controls;
using XYUI.Avalonia.Spatial;

namespace XYUI.Avalonia.Controls;

public sealed class XYSectionTitle : XyuiTextSurface
{
    public XYSectionTitle() : base("xyui-section-title")
    {
        Child = null;
        var header = new Grid
        {
            Classes = { "xyui-section-title-header" },
            ColumnDefinitions = new ColumnDefinitions("3,*"),
            Height = 28,
            VerticalAlignment = global::Avalonia.Layout.VerticalAlignment.Center,
        };
        var mark = new Border
        {
            Classes = { "xyui-section-title-left-mark" },
            Width = 3,
            Height = 16,
            VerticalAlignment = global::Avalonia.Layout.VerticalAlignment.Center,
        };
        TextPresenter.VerticalAlignment = global::Avalonia.Layout.VerticalAlignment.Center;
        TextPresenter.Margin = new Thickness(XyuiSpatialTokens.Space2, 0, 0, 0);
        Grid.SetColumn(mark, 0);
        Grid.SetColumn(TextPresenter, 1);
        header.Children.Add(mark);
        header.Children.Add(TextPresenter);
        Child = header;
    }

    public override string CanonicalId => "XYUI-1-05";
}

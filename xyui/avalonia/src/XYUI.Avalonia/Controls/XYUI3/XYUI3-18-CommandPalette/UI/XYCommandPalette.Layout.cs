using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYCommandPalette
{
    Control BuildSurface()
    {
        var content = new Grid { RowDefinitions = new RowDefinitions("Auto,*"), RowSpacing = 8 };
        Grid.SetRow(SearchBox, 0); content.Children.Add(SearchBox);
        _results.HorizontalAlignment = HorizontalAlignment.Stretch; _results.Spacing = 0;
        _resultsViewport.Content = _results; _resultsViewport.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden; _resultsViewport.VerticalScrollBarVisibility = ScrollBarVisibility.Auto; _resultsViewport.MinHeight = 120;
        var body = new Grid { ColumnDefinitions = new ColumnDefinitions("*,Auto,*"), ColumnSpacing = 10 };
        Grid.SetColumn(_resultsViewport, 0); body.Children.Add(_resultsViewport);
        var divider = new Border { Width = 1, Classes = { "xyui-palette-divider" } }; Grid.SetColumn(divider, 1); body.Children.Add(divider);
        var details = new StackPanel { Spacing = 8, Classes = { "xyui-palette-details" } };
        _detailTitle.Classes.Add("xyui-palette-detail-title"); _detailCategory.Classes.Add("xyui-palette-detail-line"); _detailShortcut.Classes.Add("xyui-palette-detail-line");
        details.Children.Add(_detailTitle); details.Children.Add(_detailDescription); details.Children.Add(_detailCategory); details.Children.Add(_detailShortcut); details.Children.Add(new TextBlock { Text = "Enter 执行", Classes = { "xyui-palette-detail-line" } });
        Grid.SetColumn(details, 2); body.Children.Add(details); Grid.SetRow(body, 1); content.Children.Add(body);
        return new Border { Classes = { "xyui-palette-surface" }, Child = content };
    }

    void UpdateDetails(XYPaletteCommand? selected)
    {
        _detailTitle.Text = selected?.Label ?? "无匹配命令"; _detailDescription.Text = selected?.Description ?? "请输入命令或搜索词。";
        _detailCategory.Text = selected is null ? "分类　—" : $"分类　{selected.Category}"; _detailShortcut.Text = selected is null ? "快捷键　—" : $"快捷键　{(selected.Shortcut.Length == 0 ? "—" : selected.Shortcut)}";
    }
}

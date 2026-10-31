using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery.Views.Density;

public partial class DensityLabView
{
    static readonly (string Title, string Code, string Status, string Summary, string Desc)[] FullDataset =
    {
        ("华南主管网", "reg-001", "已保存", "18 个顶点 · 12,482 km²", "核心管辖范围 · 最近修改 18:07"),
        ("滨海快速道路", "rd-102", "同步中", "42 个沿线采集子站", "实时吞吐量 8,420 unit/s"),
        ("珠江水系", "river-018", "已加载", "河流 / 水文网络", "连接 16 个区域水文节点"),
        ("城市节点", "city-set", "正常", "126 个城市对象", "城市层级与区域索引已同步")
    };

    void RebuildItems(XYDensity density)
    {
        WorkbenchItemsPanel.Children.Clear();
        foreach (var item in FullDataset)
            WorkbenchItemsPanel.Children.Add(CreateRow(item.Title, item.Code, item.Status, item.Summary, item.Desc, density));
    }

    static Control CreateRow(string title, string code, string status, string summary, string desc, XYDensity density)
    {
        var border = new Border { Classes = { "xyui-surface-panel" }, CornerRadius = new(4), Padding = new(8, 6) };
        if (density == XYDensity.Compact)
        {
            var sp = new StackPanel { Spacing = 2 };
            var grid = new Grid { ColumnDefinitions = new("160,110,*,90") };
            var titleBox = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 6, VerticalAlignment = VerticalAlignment.Center };
            titleBox.Children.Add(new XYLabel { Text = title });
            titleBox.Children.Add(new XYCodeText { Text = code });
            grid.Children.Add(titleBox);
            var statusBox = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 4, VerticalAlignment = VerticalAlignment.Center };
            statusBox.Children.Add(new XYStatusDot { State = XyuiStatusState.Success });
            statusBox.Children.Add(new XYCaption { Text = status });
            Grid.SetColumn(statusBox, 1); grid.Children.Add(statusBox);
            var summaryTxt = new XYCaption { Text = summary, VerticalAlignment = VerticalAlignment.Center };
            Grid.SetColumn(summaryTxt, 2); grid.Children.Add(summaryTxt);
            var acts = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 4, HorizontalAlignment = HorizontalAlignment.Right };
            acts.Children.Add(new XYIconButton { Content = new XYIcon { Icon = XyuiVectorIcon.Locate, Size = XyuiIconSize.Small } });
            acts.Children.Add(new XYIconButton { Content = new XYIcon { Icon = XyuiVectorIcon.Code, Size = XyuiIconSize.Small } });
            Grid.SetColumn(acts, 3); grid.Children.Add(acts);
            sp.Children.Add(grid);
            sp.Children.Add(new XYCaption { Text = desc });
            border.Child = sp;
        }
        else if (density == XYDensity.Default)
        {
            var stack = new StackPanel { Spacing = 4 };
            var top = new Grid { ColumnDefinitions = new("*,Auto") };
            var titleBox = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 6 };
            titleBox.Children.Add(new XYLabel { Text = title });
            titleBox.Children.Add(new XYCodeText { Text = code });
            titleBox.Children.Add(new XYStatusBadge { Text = status, State = XyuiStatusState.Success });
            top.Children.Add(titleBox);
            var acts = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 4 };
            acts.Children.Add(new XYButton { Content = "定位", Variant = XyuiButtonVariant.Secondary });
            acts.Children.Add(new XYButton { Content = "编辑", Variant = XyuiButtonVariant.Secondary });
            Grid.SetColumn(acts, 1); top.Children.Add(acts); stack.Children.Add(top);
            stack.Children.Add(new XYCaption { Text = $"{summary} — {desc}" });
            border.Child = stack;
        }
        else
        {
            var stack = new StackPanel { Spacing = 6, Margin = new(4) };
            var top = new Grid { ColumnDefinitions = new("*,Auto") };
            var titleBox = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
            titleBox.Children.Add(new XYHeading { Text = title, Variant = XyuiHeadingVariant.PanelTitle });
            titleBox.Children.Add(new XYCodeText { Text = code });
            titleBox.Children.Add(new XYStatusBadge { Text = status, State = XyuiStatusState.Success });
            top.Children.Add(titleBox);
            var acts = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 6 };
            acts.Children.Add(new XYButton { Content = "定位视口", Variant = XyuiButtonVariant.Secondary });
            acts.Children.Add(new XYButton { Content = "属性检查", Variant = XyuiButtonVariant.Primary });
            Grid.SetColumn(acts, 1); top.Children.Add(acts); stack.Children.Add(top);
            stack.Children.Add(new XYText { Text = $"数据指标：{summary}" });
            stack.Children.Add(new XYCaption { Text = desc });
            border.Child = stack;
        }
        return border;
    }
}
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI1LiveExamplesFactory
{
    static Control CreateStatusBadgeExamples()
    {
        var panel = new StackPanel { Spacing = 12 };
        var s1 = new StackPanel { Spacing = 4 };
        s1.Children.Add(new XYCaption { Text = "场景 1 · 正常就绪状态 (Success / Neutral / Warning)" });
        var row1 = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 12 };
        row1.Children.Add(new XYStatusBadge { Text = "Compiled", State = XyuiStatusState.Success });
        row1.Children.Add(new XYStatusBadge { Text = "Pending", State = XyuiStatusState.Warning });
        row1.Children.Add(new XYStatusBadge { Text = "Neutral", State = XyuiStatusState.Neutral });
        s1.Children.Add(row1);
        panel.Children.Add(s1);

        var s2 = new StackPanel { Spacing = 4 };
        s2.Children.Add(new XYCaption { Text = "场景 2 · 异常与脱机状态 (Error / Disabled)" });
        var row2 = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 12 };
        row2.Children.Add(new XYStatusBadge { Text = "Failed", State = XyuiStatusState.Error });
        row2.Children.Add(new XYStatusBadge { Text = "Offline", State = XyuiStatusState.Neutral, IsEnabled = false });
        s2.Children.Add(row2);
        panel.Children.Add(s2);
        return panel;
    }

    static Control CreateStatusDotExamples()
    {
        var panel = new StackPanel { Spacing = 12 };
        var s1 = new StackPanel { Spacing = 4 };
        s1.Children.Add(new XYCaption { Text = "场景 1 · 核心服务健康表格 (宿主列布局分离文字与圆点)" });
        var t1 = new StackPanel { Spacing = 2 };
        t1.Children.Add(DotTableHeader("SERVICE", "HEALTH"));
        t1.Children.Add(DotTableRow("Renderer Pipeline", XyuiStatusState.Success));
        t1.Children.Add(DotTableRow("Worker Process", XyuiStatusState.Info));
        t1.Children.Add(DotTableRow("World Simulation", XyuiStatusState.Success));
        s1.Children.Add(t1);
        panel.Children.Add(s1);

        var s2 = new StackPanel { Spacing = 4 };
        s2.Children.Add(new XYCaption { Text = "场景 2 · 资产管线节点状态 (右侧独立指示信号)" });
        var t2 = new StackPanel { Spacing = 2 };
        t2.Children.Add(DotTableHeader("PIPELINE NODE", "STATUS"));
        t2.Children.Add(DotTableRow("Asset Importer", XyuiStatusState.Warning));
        t2.Children.Add(DotTableRow("Geometry Cache", XyuiStatusState.Neutral));
        t2.Children.Add(DotTableRow("Remote Sync Daemon", XyuiStatusState.Error));
        s2.Children.Add(t2);
        panel.Children.Add(s2);
        return panel;
    }

    static Grid DotTableHeader(string col1, string col2)
    {
        var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("200,60"), Margin = new Thickness(0, 0, 0, 4) };
        var h1 = new XYCaption { Text = col1 };
        var h2 = new XYCaption { Text = col2, HorizontalAlignment = HorizontalAlignment.Center };
        Grid.SetColumn(h1, 0); Grid.SetColumn(h2, 1);
        grid.Children.Add(h1); grid.Children.Add(h2);
        return grid;
    }

    static Grid DotTableRow(string name, XyuiStatusState state)
    {
        var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("200,60"), Margin = new Thickness(0, 2) };
        var text = new XYText { Text = name };
        var dot = new XYStatusDot { State = state, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        Grid.SetColumn(text, 0); Grid.SetColumn(dot, 1);
        grid.Children.Add(text); grid.Children.Add(dot);
        return grid;
    }
}

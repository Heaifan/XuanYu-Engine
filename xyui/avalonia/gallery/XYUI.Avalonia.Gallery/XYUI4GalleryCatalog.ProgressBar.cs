using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Threading;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI4GalleryCatalog
{
    static Control ProgressBarPreview() => new StackPanel
    {
        Spacing = 12,
        Children = { CleanLinear(), LabeledProgress(), SegmentedStages(), InlineCompact() }
    };

    static Control CleanLinear() => new StackPanel
    {
        Spacing = 4,
        Children = { Caption("Clean Linear · 25 / 50 / 75 / 100%"), Row(25, "25%"), Row(50, "50%"), Row(75, "75%"), Row(100, "100%") }
    };

    static Control LabeledProgress() => new StackPanel
    {
        Spacing = 4,
        Children = { Caption("Labeled Progress · 正在导入 DEM"),
            new Grid { ColumnDefinitions = new ColumnDefinitions("*,Auto"), Children =
            { new XYProgressBar { Value = 68, StatusText = "正在导入 DEM", Variant = XyuiProgressBarVariant.Labeled, Width = 260 },
              new TextBlock { Text = "68%", Classes = { "xyui-text-caption" }, [Grid.ColumnProperty] = 1 } } } }
    };

    static Control SegmentedStages() => new StackPanel
    {
        Spacing = 4,
        Children = { Caption("Segmented Stage Progress"),
            new TextBlock { Text = "准备   →   读取   →   解析   →   构建   →   完成", Classes = { "xyui-text-caption" } },
            new XYProgressBar { Value = 60, Variant = XyuiProgressBarVariant.SegmentedStage, Width = 260 } }
    };

    static Control InlineCompact() => new StackPanel
    {
        Orientation = Orientation.Horizontal, Spacing = 8, VerticalAlignment = VerticalAlignment.Center,
        Children = { Caption("Inline Compact"), new XYProgressBar { Value = 82, Size = XyuiProgressBarSize.Compact, Width = 110, Variant = XyuiProgressBarVariant.InlineCompact }, Caption("82%") }
    };

    static Control Row(double value, string text) => new Grid
    {
        ColumnDefinitions = new ColumnDefinitions("*,Auto"),
        Children = { new XYProgressBar { Value = value, Width = 260 }, new TextBlock { Text = text, Classes = { "xyui-text-caption" }, [Grid.ColumnProperty] = 1 } }
    };

    static TextBlock Caption(string text) => new() { Text = text, Classes = { "xyui-text-caption" } };

    static Control ProgressBarLiveExample()
    {
        var bar = new XYProgressBar { Value = 10, StatusText = "正在导入 DEM", Width = 300 };
        var status = new TextBlock { Classes = { "xyui-text-caption" } };
        var host = new StackPanel { Spacing = 8, Children = { status, bar } };
        var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(240) };
        timer.Tick += (_, _) => { bar.Value = bar.Value >= 100 ? 10 : bar.Value + 5; status.Text = $"{bar.StatusText} · {bar.Percentage}%"; };
        host.AttachedToVisualTree += (_, _) => { status.Text = $"{bar.StatusText} · {bar.Percentage}%"; timer.Start(); };
        host.DetachedFromVisualTree += (_, _) => timer.Stop();
        return host;
    }
}

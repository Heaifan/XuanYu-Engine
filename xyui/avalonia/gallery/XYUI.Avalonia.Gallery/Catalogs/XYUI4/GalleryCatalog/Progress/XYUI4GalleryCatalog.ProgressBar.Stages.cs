using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

enum GalleryStageState { Completed, Current, Pending }
readonly record struct GalleryStage(string Name, GalleryStageState State, double LocalProgress = 0);

public static partial class XYUI4GalleryCatalog
{
    static readonly GalleryStage[] DemoStages =
    [
        new("准备", GalleryStageState.Completed), new("读取", GalleryStageState.Completed),
        new("解析", GalleryStageState.Current, 0.4), new("构建", GalleryStageState.Pending),
        new("完成", GalleryStageState.Pending)
    ];

    static Control LabeledProgress() => new StackPanel
    {
        Spacing = 4,
        Children = { Caption("Labeled Progress · 正在导入 DEM"),
            new Grid { ColumnDefinitions = new ColumnDefinitions("*,Auto"), Children =
            { new TextBlock { Text = "正在导入 DEM", Classes = { "xyui-text-caption" } },
              new TextBlock { Text = "68%", Classes = { "xyui-text-caption" }, [Grid.ColumnProperty] = 1 } } },
            new XYProgressBar { Value = 68, StatusText = "正在导入 DEM", Variant = XyuiProgressBarVariant.Labeled, Width = 260 },
            new Grid { ColumnDefinitions = new ColumnDefinitions("*,Auto"), Children =
            { new TextBlock { Text = "17 / 25 个 Tile", Classes = { "xyui-text-caption" } },
              new TextBlock { Text = "剩余 8", Classes = { "xyui-text-caption" }, [Grid.ColumnProperty] = 1 } } },
            new TextBlock { Text = "当前：N24E121.hgt", Classes = { "xyui-text-caption" } } }
    };

    static Control SegmentedStages() => new StackPanel
    {
        Spacing = 4,
        Children = { Caption("Segmented Stage Progress"),
            StageStrip() }
    };

    static Control StageStrip()
    {
        var strip = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 4 };
        foreach (var stage in DemoStages) strip.Children.Add(Stage(stage));
        return strip;
    }

    static Control Stage(GalleryStage stage)
    {
        var value = stage.State switch { GalleryStageState.Completed => 100, GalleryStageState.Current => stage.LocalProgress * 100, _ => 0 };
        var status = stage.State switch { GalleryStageState.Completed => "完成", GalleryStageState.Current => $"当前 {stage.LocalProgress:P0}", _ => "待处理" };
        return new StackPanel { Width = 48, Spacing = 3, Children =
            { new TextBlock { Text = stage.Name, Classes = { "xyui-text-caption" }, HorizontalAlignment = HorizontalAlignment.Center },
              new XYProgressBar { Value = value, Size = XyuiProgressBarSize.Compact, Variant = XyuiProgressBarVariant.SegmentedStage, Width = 48 },
              new TextBlock { Text = status, Classes = { "xyui-text-caption" }, HorizontalAlignment = HorizontalAlignment.Center } } };
    }

    static Control FoundationBoundaries() => new StackPanel
    {
        Spacing = 4,
        Children = { Caption("Foundation · 0 / 50 / 100%"), Row(0, "0%"), Row(50, "50%"), Row(100, "100%") }
    };
}

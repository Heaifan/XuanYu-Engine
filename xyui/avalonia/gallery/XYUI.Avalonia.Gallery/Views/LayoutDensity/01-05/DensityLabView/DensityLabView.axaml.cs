using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using XYUI.Avalonia;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery.Views.Density;

public partial class DensityLabView : UserControl
{
    public DensityLabView()
    {
        InitializeComponent();
        BuildToolbar();
        ApplyDensity(XYDensity.Compact);
    }

    void BuildToolbar()
    {
        ToolbarHost.Children.Add(new XYToolbar(
            new XYToolGroup(
                new XYIconButton { Content = new XYIcon { Icon = XyuiVectorIcon.Search, Size = XyuiIconSize.Small } },
                new XYIconButton { Content = new XYIcon { Icon = XyuiVectorIcon.Locate, Size = XyuiIconSize.Small } },
                new XYIconButton { Content = new XYIcon { Icon = XyuiVectorIcon.MoreHorizontal, Size = XyuiIconSize.Small } }),
            new XYSeparator { Height = 18, Margin = new Thickness(4, 0) },
            new XYTextField { Width = 380, Placeholder = "在当前视口中过滤实体与属性…" },
            new XYButton { Content = "新建实体", Variant = XyuiButtonVariant.Primary }));
    }

    void OnCompactClick(object? sender, RoutedEventArgs e) => ApplyDensity(XYDensity.Compact);
    void OnDefaultClick(object? sender, RoutedEventArgs e) => ApplyDensity(XYDensity.Default);
    void OnComfortableClick(object? sender, RoutedEventArgs e) => ApplyDensity(XYDensity.Comfortable);

    void ApplyDensity(XYDensity density)
    {
        XY.SetDensity(this, density);
        BtnCompact.Variant = density == XYDensity.Compact ? XyuiButtonVariant.Primary : XyuiButtonVariant.Secondary;
        BtnDefault.Variant = density == XYDensity.Default ? XyuiButtonVariant.Primary : XyuiButtonVariant.Secondary;
        BtnComfortable.Variant = density == XYDensity.Comfortable ? XyuiButtonVariant.Primary : XyuiButtonVariant.Secondary;
        UpdateResultPanel(density);
        RebuildItems(density);
    }

    void UpdateResultPanel(XYDensity density)
    {
        ResultDensityText.Text = density switch { XYDensity.Compact => "紧凑", XYDensity.Default => "默认", _ => "舒适" };
        ResultItemsCountText.Text = density switch { XYDensity.Compact => "4 (全可见)", XYDensity.Default => "3 (可滚动)", _ => "2 (可滚动)" };
        ResultSecondaryText.Text = density switch { XYDensity.Compact => "同行收敛", XYDensity.Default => "次级独立行", _ => "完整多行展开" };
        ResultMetaText.Text = density switch { XYDensity.Compact => "极简", XYDensity.Default => "徽标 + 次级", _ => "完整键值对" };
        ResultActionsText.Text = density switch { XYDensity.Compact => "图标优先", XYDensity.Default => "图标 + 提示", _ => "完整文字" };
        ResultWrapText.Text = density switch { XYDensity.Compact => "0", XYDensity.Default => "1", _ => "多行" };
        ColHeaderBorder.IsVisible = density != XYDensity.Comfortable;
    }
}
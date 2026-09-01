using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Density;

namespace XYUI.Avalonia.Gallery.Views.Density;

public partial class DensityLabView : UserControl
{
    public DensityLabView()
    {
        InitializeComponent();
        ApplyDensity(XyuiDensity.Default);
    }

    void OnCompactClick(object? sender, RoutedEventArgs e) => ApplyDensity(XyuiDensity.Compact);
    void OnDefaultClick(object? sender, RoutedEventArgs e) => ApplyDensity(XyuiDensity.Default);
    void OnComfortableClick(object? sender, RoutedEventArgs e) => ApplyDensity(XyuiDensity.Comfortable);

    void ApplyDensity(XyuiDensity density)
    {
        XyuiDensityScope.SetDensity(WorkbenchContainer, density);
        BtnCompact.Variant = density == XyuiDensity.Compact ? XyuiButtonVariant.Primary : XyuiButtonVariant.Secondary;
        BtnDefault.Variant = density == XyuiDensity.Default ? XyuiButtonVariant.Primary : XyuiButtonVariant.Secondary;
        BtnComfortable.Variant = density == XyuiDensity.Comfortable ? XyuiButtonVariant.Primary : XyuiButtonVariant.Secondary;
        ScopeStatusText.Text = $"当前：XyuiDensity.{density} · 固定 SizeRole=Default（32 DIP 控件自身高度）";
        RebuildItems(density);
    }

    void RebuildItems(XyuiDensity density)
    {
        WorkbenchItemsPanel.Children.Clear();
        var metrics = XyuiDensityMetrics.For(density);
        WorkbenchItemsPanel.Spacing = metrics.RowGap;
        WorkbenchContainer.Padding = new Thickness(metrics.PanelPadding);

        var data = new (string Title, string Code, string Type, string Status, string Desc)[]
        {
            ("华南主干管网", "reg-001", "区域", "已保存", "涵盖 18 个顶点与 12,482 km² 核心管辖范围，最近修改于 18:07。"),
            ("滨海快速道路", "rd-102", "道路", "同步中", "连接主干线与 42 个沿线采集子站，实时吞吐量 8,420 unit/s。"),
            ("管制枢纽节点", "sec-801", "管制", "已锁定", "安全等级 Alpha-1，包含 6 处防空识别区与独立备用电源。"),
            ("太阳能采集阵", "res-305", "采集", "运行中", "12 组高倍率光伏矩阵，当前输出效率 98.4%，状态稳定。")
        };

        foreach (var item in data)
            WorkbenchItemsPanel.Children.Add(CreateRow(item.Title, item.Code, item.Type, item.Status, item.Desc, density));
    }
}

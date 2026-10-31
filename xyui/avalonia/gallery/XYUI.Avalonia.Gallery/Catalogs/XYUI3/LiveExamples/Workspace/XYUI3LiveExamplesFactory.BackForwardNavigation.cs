using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3LiveExamplesFactory
{
    static Control CreateBackForwardNavigationLiveExamples()
    {
        var nav = new XYBackForwardNavigation();
        nav.Navigate("世界地图 / 概览");
        nav.Navigate("数据集 / 广东省道路");
        nav.Navigate("道路编辑 / 核心要塞段");

        var status = new TextBlock { Classes = { "xyui-text-caption" } };
        void SyncStatus() => status.Text = $"当前位置: [{nav.CurrentLocation}] · 索引: {nav.CurrentIndex + 1}/{nav.History.Count} · 后退: {(nav.CanGoBack ? "可用" : "已达起点禁用")} · 前进: {(nav.CanGoForward ? "可用" : "已达终点禁用")}";
        nav.LocationChanged += (_, _) => SyncStatus();
        SyncStatus();

        var btnSim = new XYButton { Content = "跳转: 兵棋推演 (截断前进历史)", Variant = XyuiButtonVariant.Secondary };
        btnSim.Click += (_, _) => nav.Navigate("实验 / 兵棋推演");
        var btnConfig = new XYButton { Content = "跳转: 系统配置", Variant = XyuiButtonVariant.Secondary };
        btnConfig.Click += (_, _) => nav.Navigate("设置 / 全局配置");

        var tip = new TextBlock { Text = "提示: 点击前进/后退箭头体验单步导航；到达起点/终点时按钮自动禁用；支持右键箭头弹出历史记录或 Alt+Left / Alt+Right 快捷键。", Classes = { "xyui-text-caption" } };
        var col = new StackPanel
        {
            Spacing = 12,
            Children =
            {
                new TextBlock { Text = "交互式导航历史 (点击按钮验证连续跳转、边界禁用与分支截断):", Classes = { "xyui-text-label" } },
                nav,
                status,
                new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, Children = { btnSim, btnConfig } },
                tip
            }
        };
        return WrapCard(col, "线性历史前进后退导航 · 34 DIP 高度、边界禁用与历史分支截断联动");
    }

    static Control CreateBackForwardNavigationComposition()
    {
        var nav = new XYBackForwardNavigation();
        nav.Navigate("地图 / 核心要塞");
        var title = new TextBlock { Text = "当前视口: 主战役地图 · 缩放 100%", Classes = { "xyui-text-label" }, VerticalAlignment = VerticalAlignment.Center };
        var tool1 = new XYButton { Content = "重置视角", Variant = XyuiButtonVariant.Secondary };
        var tool2 = new XYButton { Content = "全屏查看", Variant = XyuiButtonVariant.Secondary };

        var titleBorder = new Border { Child = title, Margin = new(16, 0) };
        var topBar = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto"),
            Height = 38,
            Children =
            {
                nav,
                titleBorder,
                new StackPanel { Orientation = Orientation.Horizontal, Spacing = 6, Children = { tool1, tool2 } }
            }
        };
        Grid.SetColumn(titleBorder, 1);
        Grid.SetColumn((StackPanel)topBar.Children[2], 2);

        var viewportMock = new Border
        {
            Height = 120,
            Classes = { "xyui-surface-panel-alt" },
            CornerRadius = new(4),
            Child = new TextBlock { Text = "【主工作区视口画布】\n导航历史与主视口坐标联动，后退时视口自动回退至上一编辑位置。", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, TextAlignment = TextAlignment.Center, Classes = { "xyui-text-caption" } }
        };

        var panel = new StackPanel { Spacing = 8, Children = { topBar, viewportMock } };
        return WrapCard(panel, "主编辑器顶栏协同组合 · 视口位置历史与工作区工具同轴布局");
    }
}

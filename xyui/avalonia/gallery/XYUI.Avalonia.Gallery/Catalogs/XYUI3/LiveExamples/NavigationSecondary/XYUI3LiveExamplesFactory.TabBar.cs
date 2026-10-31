using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3LiveExamplesFactory
{
    static Control CreateTabBarLiveExamples()
    {
        var tab1 = new XYTab { Label = "Scene_01.map", IsSelected = true };
        var tab2 = new XYTab { Label = "Terrain_02.data", IsModified = true };
        var tab3 = new XYTab { Label = "Lighting.env" };
        var tab4 = new XYTab { Label = "AI_Behavior.lua" };
        var tab5 = new XYTab { Label = "Physics_Config.json" };
        var tab6 = new XYTab { Label = "Audio_BGM.bank" };

        var tabBar = new XYTabBar(tab1, tab2, tab3, tab4, tab5, tab6) { Width = 520, SizingMode = XyuiTabSizingMode.Content };

        var statusText = new TextBlock { Text = "当前活动文档：Scene_01.map", Classes = { "xyui-text-caption" } };
        var eventLog = new TextBlock { Text = "交互指南：点击 ◀ / ▶ 翻页，点击 ⋯ 弹出完整溢出清单，点击 ✚ 动态创建新文档。", Classes = { "xyui-text-caption" } };

        tabBar.Tabs.SelectionChanged += (_, tab) => statusText.Text = $"当前活动文档：{tab.Label} (IsModified={tab.IsModified})";
        var docIndex = 6;
        tabBar.NewRequested += (_, _) =>
        {
            var newTab = new XYTab { Label = $"NewScene_{++docIndex}.map" };
            tabBar.Tabs.Add(newTab, select: true);
            eventLog.Text = $"事件：已动态创建新文档 [{newTab.Label}] 并自动选中。";
        };

        var container = new StackPanel { Spacing = 14, Children = { tabBar, statusText, eventLog } };
        return WrapCard(container, "多页签管理器 · 真实横向滚动、溢出菜单选页与动态增删");
    }

    static Control CreateTabBarComposition()
    {
        var panel = new StackPanel { Spacing = 10 };
        var mockTabBar = new XYTabBar(
            new XYTab { Label = "MainScene.map", IsSelected = true },
            new XYTab { Label = "Hierarchy.view" },
            new XYTab { Label = "Inspector.view" }) { Width = 480, SizingMode = XyuiTabSizingMode.Content };

        var workspace = new Border
        {
            Classes = { "xyui-surface-panel" },
            Height = 120,
            Padding = new(16),
            Child = new TextBlock
            {
                Text = "中央编辑器工作区（TabBar 底部厚度为 0，仅靠 Tab 自身 3 DIP Accent 构成单底线，杜绝平行双底线）",
                Classes = { "xyui-text-body" },
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            }
        };

        panel.Children.Add(mockTabBar);
        panel.Children.Add(workspace);
        panel.Children.Add(new TextBlock
        {
            Text = "PRES-3.09 红线验证：底部仅单条 Accent 线 (无两条平行底边)；文字与关闭按钮同轴垂直居中；Hover/Selected 尺寸零跳动。",
            Classes = { "xyui-text-caption" },
            TextWrapping = global::Avalonia.Media.TextWrapping.Wrap
        });
        return WrapCard(panel, "红线验收布局 · TabBar 与中央画布无缝衔接 (无双底边)");
    }
}

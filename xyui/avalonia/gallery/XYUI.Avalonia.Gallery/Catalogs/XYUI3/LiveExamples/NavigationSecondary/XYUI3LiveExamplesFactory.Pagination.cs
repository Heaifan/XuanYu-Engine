using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3LiveExamplesFactory
{
    static Control CreatePaginationLiveExamples()
    {
        var pageFirst = new XYPagination { CurrentPage = 1, TotalPages = 24, TotalItems = 480, ShowTotalItems = true };
        var pageMid = new XYPagination { CurrentPage = 12, TotalPages = 24, TotalItems = 480, ShowTotalItems = true };
        var pageLast = new XYPagination { CurrentPage = 24, TotalPages = 24, TotalItems = 480, ShowTotalItems = true };

        var interactive = new XYPagination { CurrentPage = 12, TotalPages = 24, TotalItems = 480, ShowTotalItems = true };
        var statusText = new TextBlock { Text = "当前活动页: 第 12 页 · 槽位固定 · 翻页时整体尺寸与操作键位绝对稳定", Classes = { "xyui-text-caption" } };
        interactive.PageChanged += (_, p) => statusText.Text = $"当前活动页: 第 {p} 页 / 24 · 首/上一页: {(p > 1 ? "可用" : "禁用")}, 下/末页: {(p < 24 ? "可用" : "禁用")}";

        var btn1 = new XYButton { Content = "切至第 1 页 (首页边界)", Variant = XyuiButtonVariant.Secondary };
        btn1.Click += (_, _) => interactive.GoTo(1);
        var btn12 = new XYButton { Content = "切至第 12 页 (居中展开)", Variant = XyuiButtonVariant.Secondary };
        btn12.Click += (_, _) => interactive.GoTo(12);
        var btn24 = new XYButton { Content = "切至第 24 页 (末页边界)", Variant = XyuiButtonVariant.Secondary };
        btn24.Click += (_, _) => interactive.GoTo(24);

        var footer = new XYPaginationFooter(totalItems: 640, totalPages: 26);

        var col = new StackPanel
        {
            Spacing = 12,
            Children =
            {
                new TextBlock { Text = "槽位稳定性真实对比 (首页 1 / 中页 12 / 末页 24 宽度统一，Next/Jump 无抖动):", Classes = { "xyui-text-label" } },
                pageFirst, pageMid, pageLast,
                new TextBlock { Text = "交互式动态切页 (点击按钮验证连续切页时操作按钮不位移):", Classes = { "xyui-text-label" } },
                interactive, statusText,
                new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, Children = { btn1, btn12, btn24 } },
                new TextBlock { Text = "标准数据页脚 XYPaginationFooter (含每页条数 25/50/100 联动):", Classes = { "xyui-text-label" } },
                footer
            }
        };
        return WrapCard(col, "高密度分页与数据页脚 · 槽位固定、零抖动与 34 DIP 等高对齐");
    }

    static Control CreatePaginationComposition()
    {
        var tableHeader = new Border
        {
            Classes = { "xyui-surface-panel-alt" },
            Padding = new(10, 6),
            Child = new TextBlock { Text = "名称　　　　　　　类型　　　　大小　　　　修改时间", Classes = { "xyui-text-code" } }
        };
        var row1 = new Border { Padding = new(10, 6), Child = new TextBlock { Text = "Terrain_Highland.raw　HeightMap　32.4 MB　2026-09-01 10:20", Classes = { "xyui-text-caption" } } };
        var row2 = new Border { Padding = new(10, 6), Child = new TextBlock { Text = "Road_Network_01.bin　Topology　 4.8 MB　 2026-09-02 14:15", Classes = { "xyui-text-caption" } } };
        var row3 = new Border { Padding = new(10, 6), Child = new TextBlock { Text = "Foliage_Cluster.json　Config　　512 KB　 2026-09-03 09:30", Classes = { "xyui-text-caption" } } };

        var footer = new XYPaginationFooter(totalItems: 84, totalPages: 4);

        var panel = new StackPanel
        {
            Spacing = 4, Width = 680,
            Children =
            {
                tableHeader, row1, row2, row3,
                new Border { Height = 1, Classes = { "xyui-border-subtle" }, Margin = new(0, 4) },
                footer
            }
        };
        return WrapCard(panel, "资源视口底栏组合 · 聚合条数统计、分页容量与快速跳转");
    }
}

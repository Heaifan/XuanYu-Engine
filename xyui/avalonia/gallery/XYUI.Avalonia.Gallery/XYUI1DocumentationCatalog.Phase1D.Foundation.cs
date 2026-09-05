namespace XYUI.Avalonia.Gallery;

public static partial class XYUI1DocumentationCatalog
{
    static IReadOnlyList<XYUIDocFoundationItem> Phase1DFoundationMappings(string id) => id switch
    {
        "XYUI-1-19" => [
            new("浮层底色", "XY.Brush.Surface.Overlay", "独立悬浮层高反差底色"),
            new("外框边框", "XY.Brush.Border.Color.Subtle", "微弱次级边框与 2 DIP 左侧重点强调"),
            new("最大宽度", "280 DIP", "Canonical MaxWidth 几何约束"),
            new("已知限制 (GAP)", "XYUI1-GAP-005", "Avalonia 原生浮层行为接管未完全落地")
        ],
        "XYUI-1-20" => [
            new("Normal 前景", "XY.Brush.Text.Primary", "常规正文字阶排版"),
            new("Strong 前景", "XY.Brush.Text.Primary", "FontWeight.SemiBold 强调加粗"),
            new("Mono 前景", "xy:XY.Font.Mono", "等宽代码字阶 (FontSizeMono)")
        ],
        "XYUI-1-21" => [
            new("只读容器", "SelectableTextBlock", "Avalonia 核心文本选区机制"),
            new("拷贝角标", "8 DIP Copy Mark", "基线浅灰 (#A8B2B8) 矢量拷贝图标"),
            new("Technical", "xy:XY.Font.Mono", "Technical 变体切换为等宽技术字体")
        ],
        "XYUI-1-22" => [
            new("排版字阶", "xy:XY.Typography", "Caption (11 DIP / 400 Normal)"),
            new("次级前景色", "XY.Brush.Text.Tertiary", "弱对比度占位灰阶"),
            new("纯文本反馈", "Zero Decoration", "无默认矢量图标与附加按钮")
        ],
        "XYUI-1-23" => [
            new("高亮底色", "XY.Brush.Surface.PanelAlt", "浅底色高亮容器"),
            new("矢量角标", "8 DIP Search Mark", "右上角浅灰 (#A8B2B8) 放大镜角标"),
            new("间距规范", "XyuiSpatialTokens.Space2", "8 DIP 角标水平间距")
        ],
        "XYUI-1-24" => [
            new("截断规则", "CharacterEllipsis", "TextTrimming 自动追加省略号"),
            new("换行规则", "TextWrapping.NoWrap", "强制单行，由父容器宽度决定截断点"),
            new("已知限制 (GAP)", "XYUI1-GAP-002", "Middle 模式降级为 EndEllipsis")
        ],
        _ => []
    };

    static IReadOnlyList<XYUIDocState> Phase1DStates(string id) => id switch
    {
        "XYUI-1-19" => [
            new("Open", "显示：Hover 目标或触发后浮出呈现"),
            new("Close", "关闭：指针移出或失去焦点后自动隐藏")
        ],
        "XYUI-1-20" => [
            new("Normal", "常态排版：Normal / Strong / Mono 同段流式呈现")
        ],
        "XYUI-1-21" => [
            new("Normal", "常态：文本可选中，拷贝按钮浅灰待命"),
            new("Selected", "选区激活：Avalonia 原生系统高亮选区色"),
            new("Disabled", "禁用：前景色自动切换 DisabledText")
        ],
        "XYUI-1-22" => [
            new("Normal", "常态：Caption 字阶弱化占位反馈")
        ],
        "XYUI-1-23" => [
            new("Normal", "常态：高亮底色 + 右上角浅灰放大镜角标")
        ],
        "XYUI-1-24" => [
            new("Normal", "常态：空间充裕时完整显示"),
            new("Truncated", "截断态：宽度受限时自动追加省略号")
        ],
        _ => []
    };
}

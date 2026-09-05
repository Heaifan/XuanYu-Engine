namespace XYUI.Avalonia.Gallery;

public static partial class XYUI1DocumentationCatalog
{
    static IReadOnlyList<XYUIDocGuideItem> Phase1DHowToUse(string id) => id switch
    {
        "XYUI-1-19" => [
            new("Recommended", "依托 ToolTip.Tip 附加短提示，遵循 280 DIP 宽度限制。"),
            new("Advanced", "浮层由 Overlay 底色与 Subtle 外框驱动，保证与背景形成层级反差。"),
            new("Don't", "禁止放入可点击的交互控件（InteractiveContent=false）。")
        ],
        "XYUI-1-20" => [
            new("Recommended", "按 Text / StrongText / MonoText 传递三段式排版内容。"),
            new("Advanced", "用于呈现“耗时 / 统计 / 摘要”等内联包含 ID 的信息。"),
            new("Don't", "不要在此处假冒支持 Markdown 语法或 Link 链接。")
        ],
        "XYUI-1-21" => [
            new("Recommended", "直接绑定 Text 属性，用户可鼠标拖拽选区或点击右侧复制角标。"),
            new("Advanced", "针对 ID 或哈希串使用 Variant=\"Technical\" 获得等宽对齐。"),
            new("Don't", "禁止作为 TextBox 接收输入，不可当作编辑控件。")
        ],
        "XYUI-1-22" => [
            new("Recommended", "在空列表或无选中对象时放置一条纯文本占位说明。"),
            new("Advanced", "保持界面整洁，避免未产生数据时产生过多视觉干扰。"),
            new("Don't", "不要在此处添加复杂的行动号召（CTA）按钮或插画。")
        ],
        "XYUI-1-23" => [
            new("Recommended", "将搜索算法已经计算好的匹配项直接赋给 Text 属性进行展示。"),
            new("Advanced", "右上角 8 DIP 浅灰放大镜角标提供纯净的搜索语义识别。"),
            new("Don't", "禁止在此处假造动态 Query 属性或尝试绑定搜索输入框。")
        ],
        "XYUI-1-24" => [
            new("Recommended", "放入受限宽度的父容器（如 120 DIP 侧边栏列表项），自动产生省略号。"),
            new("Advanced", "超长文本可配合 ToolTip.Tip 提供完整未截断内容。"),
            new("Don't", "禁止控件内部写死 Width 强制截断，应由外部宿主布局驱动。")
        ],
        _ => []
    };
}

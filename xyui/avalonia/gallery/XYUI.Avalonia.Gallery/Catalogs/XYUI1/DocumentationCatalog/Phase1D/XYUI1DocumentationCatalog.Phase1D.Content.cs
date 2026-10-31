namespace XYUI.Avalonia.Gallery;

public static partial class XYUI1DocumentationCatalog
{
    static string Phase1DQuickStart(string id, string type) => id switch
    {
        "XYUI-1-19" => "<Button Content=\"动作\">\n  <ToolTip.Tip>\n    <xy:XYTooltip Content=\"在当前工程中检索 (Ctrl+F)\" />\n  </ToolTip.Tip>\n</Button>",
        "XYUI-1-20" => "<xy:XYRichText Text=\"着色器编译完成：\" StrongText=\"18 个着色器\" MonoText=\"pipeline_04 · 2.4s\" />",
        "XYUI-1-21" => "<xy:XYSelectableText Text=\"7f12a8d4c92b8e4f1a603c9d\" Variant=\"Technical\" />",
        "XYUI-1-22" => "<xy:XYEmptyText Text=\"未找到符合条件的着色器资源\" />",
        "XYUI-1-23" => "<xy:XYSearchHighlight Text=\"World_terrain_chunk_loader\" />",
        "XYUI-1-24" => "<xy:XYTruncatedText Text=\"Textures/Environment/Atmosphere/skybox_hdr_v3.dds\" Mode=\"End\" />",
        _ => ""
    };

    static IReadOnlyList<XYUIDocRule> Phase1DCoreRules(string id) => id switch
    {
        "XYUI-1-19" => [
            new("组件定义", "非交互、短时、上下文相关的悬浮辅助提示组件，仅承载说明性内容。"),
            new("适用场景", "工具栏图标按钮说明、截断文本查看完整值、紧凑表单项参数引导。"),
            new("禁用场景", "禁止放入按钮、链接等可交互内容（InteractiveContent=false）；不可替代气泡弹窗。"),
            new("相邻区别", "vs HelpText：Tooltip 为悬浮就地浮层，不占常驻界面面积；HelpText 为常驻内联说明。"),
            new("已知限制 (GAP)", "XYUI1-GAP-004 (AutomationPeer) / XYUI1-GAP-005 (Avalonia 原生浮层行为接管未完全落地)。")
        ],
        "XYUI-1-20" => [
            new("组件定义", "支持 Normal、Strong、Mono 三段式受控内联排版的轻量富文本组件。"),
            new("适用场景", "编译耗时摘要、包含 ID 标识的紧凑状态文本、少量强调字段的只读段落。"),
            new("禁用场景", "禁止作为 Markdown/HTML 渲染器使用；目前不支持 Link Run（登记 GAP-003）。"),
            new("相邻区别", "vs XYText：RichText 允许在单一文本流内组合粗体强调与等宽代码字阶。"),
            new("已知限制 (GAP)", "XYUI1-GAP-003：Canonical Link semantic run 尚未落地，当前仅支持 Normal/Strong/Mono。")
        ],
        "XYUI-1-21" => [
            new("组件定义", "只读可选择文本组件，支持指针划选、快捷键复制与一键拷贝按钮。"),
            new("适用场景", "实体 GUID、资源哈希值、日志追踪码、异常堆栈与诊断信息。"),
            new("禁用场景", "禁止作为可编辑输入框（TextBox）使用；不可用于接收用户键盘输入。"),
            new("相邻区别", "vs XYText：支持鼠标选区与文本复制；vs TextBox：严格 ReadOnly 且无输入光标干扰。"),
            new("已知限制 (GAP)", "XYUI1-GAP-004：Avalonia AutomationPeer 辅助功能映射尚未落地，诚实登记保留。")
        ],
        "XYUI-1-22" => [
            new("组件定义", "轻量空状态纯文本提示组件，以弱化字阶与次级灰色呈现占位反馈。"),
            new("适用场景", "空列表容器、未选中任何对象、搜索无匹配结果的纯文本占位说明。"),
            new("禁用场景", "禁止在此添加大型插图、图标或操作按钮（复杂空态应使用专用 EmptyState 复合组件）。"),
            new("相邻区别", "vs Caption：EmptyText 具备明确的内容缺失语义；vs 复合 EmptyState：本组件为纯文本。")
        ],
        "XYUI-1-23" => [
            new("组件定义", "搜索命中结果的视觉呈现组件，右上角带有规范 8 DIP 浅灰矢量放大镜角标。"),
            new("适用场景", "搜索结果列表项、过滤命中节点、属性面板匹配项的高亮结果视觉表达。"),
            new("禁用场景", "不要当作搜索算法/查询引擎（非 Query Filter 控件）；禁止假造动态 Query 计算 API。"),
            new("相邻区别", "vs SelectableText：SearchHighlight 专用于搜索结果的静态高亮呈现，右上角带 Search 矢量角标。")
        ],
        "XYUI-1-24" => [
            new("组件定义", "在受限布局宽度内自动触发文本截断并追加省略号的单行文本组件。"),
            new("适用场景", "资源路径、超长对象名、紧凑检查器列表项，由父级布局控制可用宽度。"),
            new("禁用场景", "禁止控件内部硬编码固定宽度；Middle 模式在当前 Avalonia 运行时仍降级为 End。"),
            new("相邻区别", "vs XYText：强制 NoWrap 并配置 TextTrimming；超长时由宿主宽度驱动省略截断。"),
            new("已知限制 (GAP)", "XYUI1-GAP-002：Avalonia TextBlock 缺少 MiddleEllipsis，当前 Middle 变体降级为 End。")
        ],
        _ => []
    };
}

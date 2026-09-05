using XYUI.Avalonia.Catalog;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI1DocumentationCatalog
{
    public static IReadOnlyList<XYUI1ComponentDocument> Build()
    {
        return XyuiCatalogSource.Load().Where(x => x.Module == "XYUI-1")
            .Select(Create).ToArray();
    }

    static XYUI1ComponentDocument Create(XyuiCatalogEntry entry)
    {
        var id = entry.CanonicalId;
        var type = entry.AvaloniaType.Split('.').Last();
        var chineseName = entry.Title.Split('｜').Last();
        var details = Details(id, chineseName);
        return new(id, chineseName, entry.Name, details.Overview, details.WhenToUse,
            () => XYUI1GalleryCatalog.CreatePreview(id), Usages(id, type), Variants(id),
            Phase1AStates(id), Properties(id), Tokens(id), type)
        {
            CanonicalIdentity = entry.CanonicalIdentity,
            KnownGap = entry.KnownGap,
            Category = CategoryOf(id),
            QuickStartXaml = QuickStart(id, type),
            CoreRules = CoreRules(id),
            FoundationMappings = FoundationMappings(id),
            HowToUse = HowToUse(id),
            LiveExamplesFactory = HasLiveExamples(id) ? () => XYUI1LiveExamplesFactory.Create(id)! : null
        };
    }

    static bool HasLiveExamples(string id) => id switch
    {
        "XYUI-1-01" or "XYUI-1-02" or "XYUI-1-03" or "XYUI-1-04" or "XYUI-1-05" or "XYUI-1-06" => true,
        "XYUI-1-07" or "XYUI-1-08" or "XYUI-1-09" or "XYUI-1-10" or "XYUI-1-11" or "XYUI-1-12" or "XYUI-1-13" => true,
        "XYUI-1-14" or "XYUI-1-15" or "XYUI-1-16" or "XYUI-1-17" or "XYUI-1-18" => true,
        "XYUI-1-19" or "XYUI-1-20" or "XYUI-1-21" or "XYUI-1-22" or "XYUI-1-23" or "XYUI-1-24" => true,
        _ => false
    };

    static string CategoryOf(string id) => id switch
    {
        "XYUI-1-01" => "Canonical Stable · Typography / Text",
        "XYUI-1-02" => "Canonical Stable · Form / Property Key",
        "XYUI-1-03" => "Canonical Stable · Auxiliary / Secondary",
        "XYUI-1-04" => "Canonical Stable · Structure / Heading",
        "XYUI-1-05" => "Canonical Stable · Inspector / Group Header",
        "XYUI-1-06" => "Canonical Stable · Navigation / Interactive Link",
        "XYUI-1-07" => "Canonical Stable · Typography / Code",
        "XYUI-1-08" => "Canonical Stable · Data / Monospace",
        "XYUI-1-09" => "Canonical Stable · Indicator / Badge",
        "XYUI-1-10" => "Canonical Stable · Status / Badge",
        "XYUI-1-11" => "Canonical Stable · Status / Dot",
        "XYUI-1-12" => "Canonical Stable · Graphic / Icon",
        "XYUI-1-13" => "Canonical Stable · Composite / IconLabel",
        "XYUI-1-14" => "Canonical Stable · Structure / Separator",
        "XYUI-1-15" => "Canonical Stable · Feedback / HelpText",
        "XYUI-1-16" => "Canonical Stable · Feedback / ErrorText",
        "XYUI-1-17" => "Canonical Stable · Feedback / WarningText",
        "XYUI-1-18" => "Canonical Stable · Keyboard / ShortcutHint",
        "XYUI-1-19" => "Canonical Stable · Feedback / Tooltip",
        "XYUI-1-20" => "Canonical Stable · Typography / RichText",
        "XYUI-1-21" => "Canonical Stable · Typography / SelectableText",
        "XYUI-1-22" => "Canonical Stable · Feedback / EmptyText",
        "XYUI-1-23" => "Canonical Stable · Indicator / SearchHighlight",
        "XYUI-1-24" => "Canonical Stable · Typography / TruncatedText",
        _ => "Canonical Stable · Typography / Text"
    };

    static (string Overview, string WhenToUse) Details(string id, string title) => id switch
    {
        "XYUI-1-01" => ("普通文本是编辑器中承载主要阅读内容的基础文字组件。", "用于正文、说明、表单值和面板内的常规阅读内容。"),
        "XYUI-1-02" => ("字段名称用于建立标签与值之间的清晰关系。", "用于属性面板、筛选器和设置表单中的字段名。"),
        "XYUI-1-03" => ("辅助信息以更低的视觉权重补充上下文，不抢占主要内容注意力。", "用于时间、来源、单位和次级说明。"),
        "XYUI-1-04" => ("标题组件提供页面标题和面板标题两种明确层级。", "用于页面、面板和内容区域的标题表达。"),
        "XYUI-1-05" => ("S-05 Soft Header + Left Mark：用于 Inspector 内部区块标题，不承担页面主标题或 Divider 职责。", "用于属性面板、Inspector 和设置页中的字段分组。"),
        "XYUI-1-06" => ("超链接表达可被激活的文档、对象或外部资源入口。", "用于可导航文本；不要把普通说明文字做成链接。"),
        "XYUI-1-07" => ("代码 / ID 使用等宽字体，右下角包含 8 DIP 矢量角标（VERIFY-BASELINE）。", "用于实体 ID、资源路径、属性键名与诊断参数。"),
        "XYUI-1-08" => ("等宽数据容器（Aligned Monospace Data）基于共享 Grid 列布局提供稳定的数字对齐。", "用于三维空间坐标、帧耗时、帧率、内存与工程统计数据。"),
        "XYUI-1-09" => ("紧凑型标签具有 22 DIP 规范高度与 11 DIP 左指针（VERIFY-CANONICAL）。", "用于本地资产、只读状态、实验性功能与未保存标记。"),
        "XYUI-1-10" => ("状态标签将 5 项语义状态与短文本集成在单一可读单元中，复用 Badge 骨架。", "用于编译、同步、校验等明确操作状态；禁用通过 IsEnabled 控制。"),
        "XYUI-1-11" => ("状态圆点以最小面积呈现纯净状态信号，不包含文本内容，与 StatusBadge 共享色源。", "用于服务监控、树节点紧凑标记与管线健康指示。"),
        "XYUI-1-12" => ("标准矢量图标控件支持 24×24 视口均匀居中缩放与尺寸阶梯联动，显式 Size 优先。", "用于工具栏动作、状态辅助符号与紧凑面板图标。"),
        "XYUI-1-13" => ("图标标签由 XYIcon 与 Text 复合而成，间距固定为 4 DIP 并保持严格垂直居中。", "用于场景树节点、资产类别项与属性说明。"),
        "XYUI-1-14" => ("分割线为内容建立连续、分组或方向关系。", "用于区隔列表行、面板、标题区和垂直分栏。"),
        "XYUI-1-15" => ("帮助说明为当前操作提供低干扰的上下文提示。", "用于输入项下方或空闲区域的操作指导。"),
        "XYUI-1-16" => ("错误说明直接指出需要修复的问题，并保持与语义色一致。", "用于校验失败、无效输入和不可完成的操作结果。"),
        "XYUI-1-17" => ("警告说明提示风险或未完成条件，但不等同于操作失败。", "用于未保存、潜在风险和需要确认的状态。"),
        "XYUI-1-18" => ("快捷键提示把可操作动作与其键盘组合紧凑地呈现。", "用于按钮旁、工具栏或命令说明中。"),
        "XYUI-1-19" => ("悬浮提示承载短时、就地的补充说明；行为参数已形成 API，但 Avalonia 浮层接管仍登记 GAP-005。", "用于图标按钮、截断文本和不便直接展开的辅助信息。"),
        "XYUI-1-20" => ("富文本提供受控的强调与等宽语义；Link run API 尚未形成，登记 GAP-003。", "用于需要少量内联层次但不需要自由排版的说明。"),
        "XYUI-1-21" => ("可选择文本保留文本选择能力，适合复制对象值和诊断内容。", "用于用户需要复制或比对的文本；Technical 变体沿用等宽风格。"),
        "XYUI-1-22" => ("空状态文本是纯净的 Caption 层级反馈，不包含默认 Vector Decoration。", "用于空列表、无结果和首次使用的内容区域。"),
        "XYUI-1-23" => ("命中文本本体提供高亮；右上角 8 DIP Vector Search Mark 仅识别搜索语义，不参与正文排版。", "用于搜索结果、树节点和属性列表中的命中内容。"),
        "XYUI-1-24" => ("截断文本在有限宽度内保留可读摘要；End 为真实末尾省略，Middle 保留 API 与语义标记，但当前 Avalonia 运行时仍降级为 EndEllipsis（GAP-002）。", "普通名称使用 End；路径、文件名、ID 和 Hash 使用 Middle，并通过 Tooltip 或 Inspector 提供完整值。"),
        _ => ($"{title}是 XYUI-1 文本与信息模块中的组件。", "用于需要该语义的编辑器界面内容。")
    };
}

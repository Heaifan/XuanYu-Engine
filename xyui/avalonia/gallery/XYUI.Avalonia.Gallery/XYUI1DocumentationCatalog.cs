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
            States(id), Properties(id), Tokens(id), type)
        { CanonicalIdentity = entry.CanonicalIdentity, KnownGap = entry.KnownGap };
    }

    static (string Overview, string WhenToUse) Details(string id, string title) => id switch
    {
        "XYUI-1-01" => ("普通文本是编辑器中承载主要阅读内容的基础文字组件。", "用于正文、说明、表单值和面板内的常规阅读内容。"),
        "XYUI-1-02" => ("字段名称用于建立标签与值之间的清晰关系。", "用于属性面板、筛选器和设置表单中的字段名。"),
        "XYUI-1-03" => ("辅助信息以更低的视觉权重补充上下文，不抢占主要内容注意力。", "用于时间、来源、单位和次级说明。"),
        "XYUI-1-04" => ("标题组件提供页面标题和面板标题两种明确层级。", "用于页面、面板和内容区域的标题表达。"),
        "XYUI-1-05" => ("区块标题用于在同一面板内划分相关属性或内容组。", "用于 Inspector、设置页和列表中的内容分组。"),
        "XYUI-1-06" => ("超链接表达可被激活的文档、对象或外部资源入口。", "用于可导航文本；不要把普通说明文字做成链接。"),
        "XYUI-1-07" => ("代码 / ID 使用等宽字体，右下角独立显示 8 DIP 的 Vector Code Mark，不进入正文排版。", "用于实体 ID、路径、短代码和诊断信息。"),
        "XYUI-1-08" => ("等宽数据使用 M-05A 三列 Grid 锁定 Label 与 Value 的共同起点，保持坐标和帧时间的列节奏。", "用于坐标、尺寸、哈希和调试数据。"),
        "XYUI-1-09" => ("标签是 22 DIP 的矢量牌形，左侧 11 DIP 指针与右侧主体由同一 Background Geometry 表达。", "用于类型、筛选条件和轻量分类信息。"),
        "XYUI-1-10" => ("状态标签把状态语义与短文本放在同一个可读单元中。", "用于保存、连接、校验和同步状态。"),
        "XYUI-1-11" => ("状态圆点用最小面积传达状态，不承担完整状态文字。", "用于列表、树节点或状态标签旁的快速指示。"),
        "XYUI-1-12" => ("图标由 XYUI Vector Icon Registry 提供真实 StreamGeometry，尺寸与 StrokeWidth 按 Tiny / Small / Default / Large 联动。", "用于动作提示、状态补充和紧凑工具栏。"),
        "XYUI-1-13" => ("图标加文字把符号和可读名称组合成一个紧凑标签。", "用于导航项、对象类别和带图标的字段说明。"),
        "XYUI-1-14" => ("分割线为内容建立连续、分组或方向关系。", "用于区隔列表行、面板、标题区和垂直分栏。"),
        "XYUI-1-15" => ("帮助说明为当前操作提供低干扰的上下文提示。", "用于输入项下方或空闲区域的操作指导。"),
        "XYUI-1-16" => ("错误说明直接指出需要修复的问题，并保持与语义色一致。", "用于校验失败、无效输入和不可完成的操作结果。"),
        "XYUI-1-17" => ("警告说明提示风险或未完成条件，但不等同于操作失败。", "用于未保存、潜在风险和需要确认的状态。"),
        "XYUI-1-18" => ("快捷键提示把可操作动作与其键盘组合紧凑地呈现。", "用于按钮旁、工具栏或命令说明中。"),
        "XYUI-1-19" => ("悬浮提示承载短时、就地的补充说明；行为参数已形成 API，但 Avalonia 浮层接管仍登记 GAP-005。", "用于图标按钮、截断文本和不便直接展开的辅助信息。"),
        "XYUI-1-20" => ("富文本提供受控的强调与等宽语义；Link run API 尚未形成，登记 GAP-003。", "用于需要少量内联层次但不需要自由排版的说明。"),
        "XYUI-1-21" => ("可选择文本保留文本选择能力，适合复制对象值和诊断内容。", "用于用户需要复制或比对的文本；Technical 变体沿用等宽风格。"),
        "XYUI-1-22" => ("空状态文本在没有数据时给出明确且克制的反馈。", "用于空列表、无结果和首次使用的内容区域。"),
        "XYUI-1-23" => ("搜索高亮在右上角显示独立的 8 DIP Vector Search Mark，不参与正文排版。", "用于搜索结果、树节点和属性列表中的命中内容。"),
        "XYUI-1-24" => ("截断文本在有限宽度内保留可读摘要；End 为真实末尾省略，Middle 保留 API 与语义标记，但当前 Avalonia 运行时仍降级为 EndEllipsis（GAP-002）。", "普通名称使用 End；路径、文件名、ID 和 Hash 使用 Middle，并通过 Tooltip 或 Inspector 提供完整值。"),
        _ => ($"{title}是 XYUI-1 文本与信息模块中的组件。", "用于需要该语义的编辑器界面内容。")
    };
}

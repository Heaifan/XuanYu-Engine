namespace XYUI.Avalonia.Gallery;

public static partial class XYUI1DocumentationCatalog
{
    static string Phase1BQuickStart(string id, string type) => id switch
    {
        "XYUI-1-07" => "<xy:XYCodeText Text=\"terrain/main-heightfield\" />",
        "XYUI-1-08" => "var data = new XYMonoText();\ndata.Rows.Add(new(\"X 坐标\", \"142.583\", \"m\"));\ndata.Rows.Add(new(\"帧耗时\", \"16.67\", \"ms\"));",
        "XYUI-1-09" => "<xy:XYBadge Text=\"Local\" Variant=\"Default\" />",
        "XYUI-1-10" => "<xy:XYStatusBadge Text=\"Compiled\" State=\"Success\" />",
        "XYUI-1-11" => "<xy:XYStatusDot State=\"Info\" />",
        "XYUI-1-12" => "<xy:XYIcon Icon=\"Search\" xy:XY.Size=\"Comfortable\" />",
        "XYUI-1-13" => "<xy:XYIconLabel Icon=\"Info\" Label=\"区域\" />",
        _ => Phase1CQuickStart(id, type)
    };

    static IReadOnlyList<XYUIDocRule> Phase1BCoreRules(string id) => id switch
    {
        "XYUI-1-07" => [
            new("组件定义", "表达代码、ID、键名、路径、技术标识等短技术文本。"),
            new("适用场景", "实体 ID、属性键名、资源路径、诊断参数与技术标识符。"),
            new("禁用场景", "不要用于普通正文、多行代码块或数值对齐表（请使用 XYMonoText）。"),
            new("相邻区别", "vs XYText：带 PanelAlt 底色与等宽字体；vs XYMonoText：CodeText 为独立标识，MonoText 为数值对齐。")
        ],
        "XYUI-1-08" => [
            new("组件定义", "Aligned Monospace Data 对齐等宽数据容器，基于共享 Grid 表达 Label/Value/Unit。"),
            new("适用场景", "空间三维坐标、物理参数、帧率耗时、内存监控与工程统计量。"),
            new("禁用场景", "不要作为单段普通文本使用（禁止伪造单行 Text 属性）；不用于常规阅读流。"),
            new("相邻区别", "vs XYCodeText：CodeText 为单短语标识；MonoText 为多行结构化数值列对齐容器。")
        ],
        "XYUI-1-09" => [
            new("组件定义", "22 DIP 高度的紧凑型 LeftPointer 标签，用于类型标记与工作区状态。"),
            new("适用场景", "本地资产标记、只读状态、实验性功能与未保存标记。"),
            new("禁用场景", "禁止发明 Success/Warning/Error 等变体（请使用 XYStatusBadge）；不要横向拉伸。"),
            new("相邻区别", "vs XYStatusBadge：Badge 仅表达分类与标记（Default/Accent），不承担状态语义。")
        ],
        "XYUI-1-10" => [
            new("组件定义", "复用 Badge 骨架并注入语义状态指示，将状态与短文本集成在单一单元。"),
            new("适用场景", "着色器编译状态、网络同步连接、校验告警与任务完成度。"),
            new("禁用场景", "禁止使用颜色词定义状态；没有单独 Disabled State enum（通过 IsEnabled=False 表达）。"),
            new("相邻区别", "vs XYStatusDot：StatusBadge 包含完整文本描述；StatusDot 为紧凑无字信号。"),
            new("已知限制 (GAP)", "XYUI1-GAP-004：Avalonia AutomationPeer 辅助功能映射尚未落地，诚实登记保留。")
        ],
        "XYUI-1-11" => [
            new("组件定义", "最小面积的状态视觉指示信号，不包含文本内容。"),
            new("适用场景", "服务监控指示灯、树节点紧凑标记、渲染器与资产管线健康指示。"),
            new("禁用场景", "不要用于需要文字解释的关键业务结果（请使用 XYStatusBadge）；严禁定义私有状态色。"),
            new("相邻区别", "vs XYStatusBadge：与 StatusBadge 100% 共享 XyuiStatusStateTokens 状态色源。"),
            new("已知限制 (GAP)", "XYUI1-GAP-004：Avalonia AutomationPeer 辅助功能映射尚未落地，诚实登记保留。")
        ],
        "XYUI-1-12" => [
            new("组件定义", "消费 XYUI Vector Icon Registry 注册表的标准矢量图标控件，支持 24×24 视口均匀缩放。"),
            new("适用场景", "工具栏动作图标、状态辅助符号、紧凑面板提示图标。"),
            new("禁用场景", "禁止建立独立图标资产展示墙；不要手写独立宽高的矢量画布。"),
            new("相邻区别", "统一由 Foundation xy:XY.Size 控制尺寸阶梯，显式 Size 优先级高于继承尺寸。"),
            new("已知限制 (GAP)", "XYUI1-GAP-004：Avalonia AutomationPeer 辅助功能映射尚未落地，诚实登记保留。")
        ],
        "XYUI-1-13" => [
            new("组件定义", "严格由 XYIcon 与 Text 复合而成的标签控件，间距固定为 Space1 (4 DIP)。"),
            new("适用场景", "场景树层级节点、资产类别项、带图标的状态与属性说明。"),
            new("禁用场景", "严禁复制独立图标绘制实现；不要忽略文本 Primary 与图标 Secondary 的层级差异。"),
            new("相邻区别", "vs XYIcon：IconLabel 为复合文本组件，内置垂直居中与规范间距，支持全状态联动。")
        ],
        _ => Phase1CCoreRules(id)
    };
}

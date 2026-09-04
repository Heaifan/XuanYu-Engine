namespace XYUI.Avalonia.Gallery;

public static partial class XYUI1DocumentationCatalog
{
    static string QuickStart(string id, string type) => id switch
    {
        "XYUI-1-01" => "<xy:XYText Text=\"当前工程已就绪，所有着色器已成功编译。\" />",
        "XYUI-1-02" => "<xy:XYLabel Text=\"采样精度\" />",
        "XYUI-1-03" => "<xy:XYCaption Text=\"自动保存于 2 分钟前 · 无未提交改动\" />",
        "XYUI-1-04" => "<xy:XYHeading Text=\"世界地图设置\" Variant=\"PanelTitle\" />",
        "XYUI-1-05" => "<xy:XYSectionTitle Text=\"变换属性 (Transform)\" />",
        "XYUI-1-06" => "<xy:XYLink Content=\"打开渲染管线配置手册\" />",
        _ => Usages(id, type).FirstOrDefault() ?? $"<xy:{type} Text=\"示例内容\" />"
    };

    static IReadOnlyList<XYUIDocRule> CoreRules(string id) => id switch
    {
        "XYUI-1-01" => [
            new("组件定义", "编辑器主要阅读流的基础正文组件，由 Foundation Typography 与 Foreground 驱动。"),
            new("适用场景", "正文段落、属性详细描述、对话框说明信息、常规只读内容。"),
            new("禁用场景", "不要用于属性列表前的字段名称（请使用 XYLabel）；不要用于次级时间戳或单位（请使用 XYCaption）。"),
            new("相邻区别", "vs XYLabel：Text 承载主要内容阅读，默认 Primary 前景；Label 承担 Key-Value 键名语义与紧凑对齐。")
        ],
        "XYUI-1-02" => [
            new("组件定义", "表单字段与属性面板中的键名（Key）组件，具有稳定的行高与垂直对齐规格。"),
            new("适用场景", "输入框、下拉框、滑块等控件左侧或上方的参数名称、设置项标题。"),
            new("禁用场景", "不要用于大段描述性多行正文（请使用 XYText）；不要用于无关联控件的孤立说明。"),
            new("相邻区别", "vs XYText：Label 专用于表单排版，字阶和基线与输入控件协同对齐，强调 Key 属性。")
        ],
        "XYUI-1-03" => [
            new("组件定义", "低视觉权重的次要辅助文本，以 Secondary/Tertiary 前景呈现，降低视觉干扰。"),
            new("适用场景", "保存时间戳、度量单位（如 DIP、ms）、文件路径补充、只读状态弱提示。"),
            new("禁用场景", "不要用于核心操作指引或长正文（避免对比度不足影响可读性）；不要用于错误提示（请使用 XYErrorText）。"),
            new("相邻区别", "vs XYText：Caption 默认使用更小字号（11~12 DIP）与次级灰度，形成明确的层级弱化。")
        ],
        "XYUI-1-04" => [
            new("组件定义", "页面与独立面板顶部的内容组织标题，提供 PageTitle 与 PanelTitle 两级明确语义。"),
            new("适用场景", "窗口/工作区页首大标题、Dock 面板标题栏、重要模态框大标题。"),
            new("禁用场景", "不要用于 Inspector 内部细分属性组（请使用带竖标的 XYSectionTitle）；不要滥用大号 PageTitle。"),
            new("相邻区别", "vs XYSectionTitle：Heading 是无左标的独立容器大标题；SectionTitle 专为属性检查器分组打造。")
        ],
        "XYUI-1-05" => [
            new("组件定义", "Inspector 专用的区块分组标题，遵循 S-05 Soft Header + 3×16 DIP Left Mark 规范。"),
            new("适用场景", "属性面板内部对参数进行归类分组（如“基础信息”、“光照”、“碰撞体”）。"),
            new("禁用场景", "不要用作窗口顶级标题；不要作为纯水平分割线（Divider）使用。"),
            new("相邻区别", "vs XYHeading：SectionTitle 具有紧凑高度（28 DIP）与垂直指示标，形成软性区块区隔。")
        ],
        "XYUI-1-06" => [
            new("组件定义", "内联或独立的可交互导航文本，具备透明背景与标准 Focus/Hover/Pressed 状态合同。"),
            new("适用场景", "文档跳转入口、关联实体引用链接、外部 URL 导航。"),
            new("禁用场景", "不要用于触发执行命令或破坏性操作（如保存、删除请使用 XYButton）；不要用于非交互文字。"),
            new("相邻区别", "vs XYButton：Link 呈现无边框下划线文本风格，表达“资源导航”，而非“触发命令动作”。")
        ],
        _ => []
    };

    static IReadOnlyList<XYUIDocGuideItem> HowToUse(string id) => id switch
    {
        "XYUI-1-01" => [
            new("Recommended", "依托父容器的 xy:XY.Size 与 xy:XY.Typography 自动继承，保持 XAML 简洁。"),
            new("Advanced", "结合 xy:XY.Foreground=\"XY.Text.Link\" 或语义色实现局部的强调与对比。"),
            new("Don't", "禁止在控件上手写硬编码 FontSize 或 HEX 前景色（如 Foreground=\"#243744\"）。")
        ],
        "XYUI-1-02" => [
            new("Recommended", "在 Grid 中将 Label 列宽固定（如 100~120 DIP），保持属性面板整齐统一。"),
            new("Advanced", "配合 xy:XY.Density=\"Compact\" 实现超高信息密度的游戏引擎属性面板。"),
            new("Don't", "禁止使用普通 TextBlock + FontWeight=\"Bold\" 拼装标签。")
        ],
        "XYUI-1-03" => [
            new("Recommended", "置于输入框底部提供格式说明，或置于卡片角落显示更新时间。"),
            new("Advanced", "结合 xy:XY.Font=\"XY.Font.Mono\" 用于代码行号、内存与帧率单位标注。"),
            new("Don't", "禁止使用 Caption 承载操作报错或警告信息。")
        ],
        "XYUI-1-04" => [
            new("Recommended", "单页面保持唯一 PageTitle，Dock 子面板使用 PanelTitle。"),
            new("Advanced", "可在 Heading 下方配以 XYCaption 呈现轻量副标题说明。"),
            new("Don't", "禁止为标题手写 FontSize=\"20\" 等字阶魔法数字。")
        ],
        "XYUI-1-05" => [
            new("Recommended", "作为属性面板每个 PropertyGroup 的头部起始控件。"),
            new("Advanced", "可在其右侧添加折叠指示或微型操作按钮，扩展分组能力。"),
            new("Don't", "禁止修改 3×16 DIP Left Mark 的标准几何规范。")
        ],
        "XYUI-1-06" => [
            new("Recommended", "用于跳转到外部参考或打开关联资产。"),
            new("Advanced", "结合 ToolTip.Tip 提示完整的链接目标地址。"),
            new("Don't", "禁止将无点击动作的普通说明文字套用链接样式。")
        ],
        _ => []
    };
}

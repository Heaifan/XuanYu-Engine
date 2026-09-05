namespace XYUI.Avalonia.Gallery;

public static partial class XYUI1DocumentationCatalog
{
    static IReadOnlyList<XYUIDocGuideItem> Phase1BHowToUse(string id) => id switch
    {
        "XYUI-1-07" => [
            new("Recommended", "实体与路径引用直接绑定 Text 属性，自动呈现等宽与浅色底色。"),
            new("Advanced", "右下角 8 DIP Vector Code Mark 为真实实现（标记 VERIFY-BASELINE）。"),
            new("Don't", "不要用于大段多行代码排版。")
        ],
        "XYUI-1-08" => [
            new("Recommended", "通过 Rows.Add 提供 Label/Value/Unit 结构化数据行，获得稳定列宽对齐。"),
            new("Advanced", "运行时高频刷新 Value 时，共享列宽保证界面绝不抖动。"),
            new("Don't", "不要伪造单段普通文本使用。")
        ],
        "XYUI-1-09" => [
            new("Recommended", "按 Default / Accent 变体表达轻量分类或强调标记。"),
            new("Advanced", "22 DIP 高度为 Canonical Geometry，11 DIP 左指针标注 VERIFY-CANONICAL。"),
            new("Don't", "不要拉伸宽度铺满整行或发明状态变体。")
        ],
        "XYUI-1-10" => [
            new("Recommended", "按 Success / Warning / Error / Info / Neutral 传递标准状态语义。"),
            new("Advanced", "脱机或不可用通过 IsEnabled=\"False\" 表达，前景色自动降级。"),
            new("Don't", "不要硬编码私有颜色或使用红/黄/绿等视觉词汇。")
        ],
        "XYUI-1-11" => [
            new("Recommended", "嵌入列表项或服务名左侧，提供高密度紧凑状态反馈。"),
            new("Advanced", "与 XYStatusBadge 共享同一套语义色彩 Token。"),
            new("Don't", "不要孤立放置无业务上下文的圆点。")
        ],
        "XYUI-1-12" => [
            new("Recommended", "优先依托父级 xy:XY.Size 统一缩放，保持界面节奏协调。"),
            new("Advanced", "特殊场景通过显式 Size 属性覆盖，优先级高于继承尺寸。"),
            new("Don't", "不要在此建立图标 Catalog 资产墙。")
        ],
        "XYUI-1-13" => [
            new("Recommended", "绑定 Icon 与 Label 属性即可自动获得 4 DIP 间距与垂直对齐。"),
            new("Advanced", "文本默认 Primary 色，图标默认 Secondary 色，层级清晰。"),
            new("Don't", "不要在外部重复套 StackPanel 或手动加 Margin。")
        ],
        _ => []
    };
}

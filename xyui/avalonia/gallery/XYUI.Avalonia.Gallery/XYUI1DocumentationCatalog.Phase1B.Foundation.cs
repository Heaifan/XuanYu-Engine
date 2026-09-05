namespace XYUI.Avalonia.Gallery;

public static partial class XYUI1DocumentationCatalog
{
    static IReadOnlyList<XYUIDocFoundationItem> Phase1BFoundationMappings(string id) => id switch
    {
        "XYUI-1-07" => [
            new("排版规格", "xy:XY.Font.Mono", "等宽字体族群与紧凑行高"),
            new("前景色阶", "XY.Brush.Text.Tertiary", "弱化对比度降低视觉噪音"),
            new("容器底色", "XY.Brush.Surface.PanelAlt", "技术标识浅底色容器"),
            new("矢量角标", "8 DIP Code Mark", "VERIFY-BASELINE · 标识语义")
        ],
        "XYUI-1-08" => [
            new("等宽排版", "xy:XY.Font.Mono", "数值列严格等宽对齐"),
            new("列体系", "Grid 共享列宽", "Label(Auto) | Value(Auto) | Unit(Auto)"),
            new("间距系统", "XyuiSpatialTokens", "Label-Value: Space4+1(20) / Value-Unit: Space2(8)"),
            new("对齐规则", "数值右对齐", "Value 右对齐保证小数点与位宽稳定")
        ],
        "XYUI-1-09" => [
            new("规范高度", "22 DIP", "Canonical Component Geometry"),
            new("左指针宽", "11 DIP", "VERIFY-CANONICAL · 骨架几何"),
            new("Default", "XY.Brush.Surface.PanelAlt", "常规只读与分类底色"),
            new("Accent", "XY.Brush.Accent.Strong", "强调与未保存状态")
        ],
        "XYUI-1-10" => [
            new("状态色源", "XyuiStatusStateTokens", "共享状态语义色彩源"),
            new("状态矩阵", "Success/Warning/Error/Info/Neutral", "5 项标准语义状态"),
            new("禁用合同", "IsEnabled=\"False\"", "前景色自动降级 DisabledText"),
            new("结构复用", "Badge 骨架", "复用 Badge 文本与圆角布局")
        ],
        "XYUI-1-11" => [
            new("状态色源", "XyuiStatusStateTokens", "与 StatusBadge 100% 共享色源"),
            new("几何尺寸", "8 DIP 圆点", "极紧凑无文本状态信号"),
            new("状态矩阵", "Success/Warning/Error/Info/Neutral", "标准 5 项业务语义")
        ],
        "XYUI-1-12" => [
            new("视口规范", "24 × 24 Viewport", "逻辑视口与均匀缩放"),
            new("尺寸映射", "xy:XY.Size", "Compact(14) / Default(16) / Comfortable(20) / Touch(24)"),
            new("覆盖规则", "显式 Size 优先", "显式 Size 属性高于继承尺寸"),
            new("线宽联动", "StrokeThickness", "随尺寸变体联动 (1.25~2.00 DIP)")
        ],
        "XYUI-1-13" => [
            new("复合复用", "真实复用 XYIcon", "XYIcon + TextPresenter"),
            new("前景层级", "XY.Brush.Text.*", "Text 默认 Primary，Icon 默认 Secondary"),
            new("间距规范", "Space1 (4 DIP)", "固定微水平间距"),
            new("禁用联动", "IsEnabled=\"False\"", "Text 与 Icon 统一切换 DisabledText")
        ],
        _ => []
    };

    static IReadOnlyList<XYUIDocState> Phase1BStates(string id) => id switch
    {
        "XYUI-1-07" => [
            new("Normal", "常态：PanelAlt 底色 + Tertiary 前景"),
            new("Disabled", "禁用：前景色自动降级 DisabledText")
        ],
        "XYUI-1-08" => [
            new("Normal", "常态：Label UI / Value Mono / Unit UI 稳定对齐"),
            new("Disabled", "禁用：整体降级禁用灰色")
        ],
        "XYUI-1-09" => [
            new("Default", "默认变体：次级面板底色"),
            new("Accent", "强调变体：品牌强调色"),
            new("Disabled", "禁用态弱化呈现")
        ],
        "XYUI-1-10" => [
            new("Success", "成功 / 已完成 / 正常"),
            new("Warning", "警告 / 待处理 / 注意"),
            new("Error", "错误 / 阻断 / 失败"),
            new("Info", "信息 / 提示 / 进行中信息"),
            new("Neutral", "中性 / 未分类 / 默认状态"),
            new("Disabled", "控件禁用：IsEnabled=\"False\"")
        ],
        "XYUI-1-11" => [
            new("Success", "成功 / 已完成 / 正常"), new("Warning", "警告 / 待处理 / 注意"),
            new("Error", "错误 / 阻断 / 失败"), new("Info", "信息 / 提示 / 进行中信息"),
            new("Neutral", "中性 / 未分类 / 默认状态")
        ],
        "XYUI-1-12" => [
            new("Normal", "常态：继承父级前景色"),
            new("Disabled", "禁用：自动切换 DisabledText")
        ],
        "XYUI-1-13" => [
            new("Normal", "常态：Text Primary / Icon Secondary"),
            new("Disabled", "禁用：Text 与 Icon 统一致灰")
        ],
        _ => []
    };
}

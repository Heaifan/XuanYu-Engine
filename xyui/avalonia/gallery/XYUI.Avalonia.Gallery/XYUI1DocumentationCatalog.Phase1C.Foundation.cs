namespace XYUI.Avalonia.Gallery;

public static partial class XYUI1DocumentationCatalog
{
    static IReadOnlyList<XYUIDocFoundationItem> Phase1CFoundationMappings(string id) => id switch
    {
        "XYUI-1-14" => [
            new("基准线宽", "XY.Border.Width.Default", "1.0 DIP Canonical Divider 几何厚度"),
            new("分割色阶", "XY.Brush.Divider.Default", "规范内容分割浅灰色线"),
            new("变体内嵌", "XyuiSpatialTokens", "Default(0) / Header(0) / Panel(0) / Section(8) / ListRow(16) / VerticalSplit(0)"),
            new("方向规则", "Variant-Driven", "VerticalSplit 为垂直方向，其余变体均为水平方向")
        ],
        "XYUI-1-15" => [
            new("排版字阶", "xy:XY.Typography", "Caption (11 DIP / 400 Normal)"),
            new("文本前景色", "XY.Brush.Text.Secondary", "次级辅助提示对比度"),
            new("Mark 语义色", "XY.Brush.Semantic.Info.Text", "装饰性信息标记矢量色"),
            new("禁用联动", "XY.Brush.State.Disabled.Text", "IsEnabled=\"False\" 时文字与标记统一步骤降级")
        ],
        "XYUI-1-16" => [
            new("排版字阶", "xy:XY.Typography", "Caption (11 DIP / 500 Medium)"),
            new("错误语义色", "XY.Brush.Semantic.Error.Text", "文本与错误 Mark 100% 共享阻断色源"),
            new("禁用联动", "XY.Brush.State.Disabled.Text", "IsEnabled=\"False\" 时文字与标记统一步骤降级"),
            new("已知限制 (GAP)", "XYUI1-GAP-004", "Avalonia AutomationPeer 映射待未来补齐")
        ],
        "XYUI-1-17" => [
            new("排版字阶", "xy:XY.Typography", "Caption (11 DIP / 500 Medium)"),
            new("警告语义色", "XY.Brush.Semantic.Warning.Text", "文本与警告 Mark 100% 共享注意色源"),
            new("禁用联动", "XY.Brush.State.Disabled.Text", "IsEnabled=\"False\" 时文字与标记统一步骤降级"),
            new("已知限制 (GAP)", "XYUI1-GAP-004", "Avalonia AutomationPeer 映射待未来补齐")
        ],
        "XYUI-1-18" => [
            new("键帽高度", "XY.Size.Control.XS", "22 DIP (ControlExtraSmallHeight) 规范高度"),
            new("几何边框", "XY.Border.Width.Default", "1.0 DIP 微弱边框 (Subtle Border)"),
            new("键帽圆角", "XY.Radius.Control", "4.0 DIP 规范圆角 (RadiusControl)"),
            new("键帽底色", "XY.Brush.Surface.PanelAlt", "次级面板背景，轻微浮出"),
            new("排版字阶", "xy:XY.Font.Mono", "Caption 等宽键帽字符族群"),
            new("禁用联动", "XY.Brush.State.Disabled.*", "Disabled 背景、边框与文字统一灰阶降级")
        ],
        _ => Phase1DFoundationMappings(id)
    };

    static IReadOnlyList<XYUIDocState> Phase1CStates(string id) => id switch
    {
        "XYUI-1-14" => [
            new("Light", "浅色主题：Divider.Default 柔和浅灰"),
            new("Dark", "深色主题：Divider.Default 低反差可见中灰")
        ],
        "XYUI-1-15" => [
            new("Normal", "常态：Secondary 文本 + Info Mark"),
            new("Disabled", "禁用：IsEnabled=False，文本与 Mark 同步降级 DisabledText")
        ],
        "XYUI-1-16" => [
            new("Normal", "常态：Semantic.Error 文本与 Mark 阻断警示"),
            new("Disabled", "禁用：IsEnabled=False，文本与 Mark 同步降级 DisabledText")
        ],
        "XYUI-1-17" => [
            new("Normal", "常态：Semantic.Warning 文本与 Mark 注意警示"),
            new("Disabled", "禁用：IsEnabled=False，文本与 Mark 同步降级 DisabledText")
        ],
        "XYUI-1-18" => [
            new("Normal", "常态：PanelAlt 底色 + Subtle 边框 + Mono 文本"),
            new("Disabled", "禁用：IsEnabled=False，底色/边框/文字统一为 Disabled 灰阶")
        ],
        _ => Phase1DStates(id)
    };
}

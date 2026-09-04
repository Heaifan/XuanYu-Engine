namespace XYUI.Avalonia.Gallery;

public static partial class XYUI1DocumentationCatalog
{
    static IReadOnlyList<XYUIDocFoundationItem> FoundationMappings(string id) => id switch
    {
        "XYUI-1-01" => [
            new("排版规格", "xy:XY.Typography", "默认映射 XY.Type.Body (13 DIP / 18 DIP 行高)"),
            new("前景颜色", "xy:XY.Foreground", "默认继承 XY.Brush.Text.Primary"),
            new("尺寸联动", "xy:XY.Size", "支持 Compact / Default / Comfortable / Touch 尺寸阶梯"),
            new("密度响应", "xy:XY.Density", "跟随容器 Compact / Normal / Loose 自动调整")
        ],
        "XYUI-1-02" => [
            new("排版规格", "xy:XY.Typography", "默认映射 XY.Type.Label，与表单输入框基准对齐"),
            new("前景颜色", "xy:XY.Foreground", "默认使用 XY.Brush.Text.Primary"),
            new("尺寸联动", "xy:XY.Size", "跟随表单尺寸同步缩放"),
            new("密度响应", "xy:XY.Density", "在 Compact 密度下提供紧凑垂直间距")
        ],
        "XYUI-1-03" => [
            new("排版规格", "xy:XY.Typography", "默认映射 XY.Type.Caption (11~12 DIP / 弱视觉权重)"),
            new("前景颜色", "xy:XY.Foreground", "默认使用 XY.Brush.Text.Secondary 或 Tertiary"),
            new("尺寸联动", "xy:XY.Size", "比例阶梯随父级联动"),
            new("弱化对比", "XY.Theme", "深浅色主题下保持稳定的次级对比度")
        ],
        "XYUI-1-04" => [
            new("页面标题", "Variant=\"PageTitle\"", "映射 XY.FontSize.PageTitle (20 DIP / SemiBold)"),
            new("面板标题", "Variant=\"PanelTitle\"", "映射 XY.FontSize.PanelTitle (16 DIP / SemiBold)"),
            new("前景颜色", "xy:XY.Foreground", "使用 XY.Brush.Text.Primary 确保标题辨识度"),
            new("字体族群", "xy:XY.Font", "默认 XY.Font.UI，支持切换 XY.Font.Mono")
        ],
        "XYUI-1-05" => [
            new("结构规范", "S-05 Soft Header", "固定高度 28 DIP，背景使用 XY.Brush.Surface.Panel"),
            new("左指示标", "Left Mark", "规格 3 × 16 DIP，使用 XY.Brush.Accent.Strong 品牌标色"),
            new("文字排版", "XY.Type.Label", "14 DIP / SemiBold / 18 DIP 行高，左间距 XY.Space.2 (8 DIP)"),
            new("边框系统", "xy:XY.Border", "边缘微弱边框绑定 XY.Brush.Border.Color.Subtle")
        ],
        "XYUI-1-06" => [
            new("排版规格", "xy:XY.Typography", "继承 XY.Type.Body (13 DIP / 500 Medium)"),
            new("状态前景色", "XY.Brush.Text.Link", "默认状态下使用链接主蓝色"),
            new("悬浮/按下", "XY.Brush.Accent.Strong", "Hover 与 Pressed 状态使用强化强调色"),
            new("焦点外框", "xyui-focusable", "支持键盘 Tab 导航并呈现统一无障碍焦点框"),
            new("禁用前景色", "XY.Brush.State.Disabled.Text", "禁用状态下自动降级为 Disabled 灰色")
        ],
        _ => []
    };

    static IReadOnlyList<XYUIDocState> Phase1AStates(string id) => id switch
    {
        "XYUI-1-06" => [
            new("Normal", "常态：Foreground 绑定 XY.Brush.Text.Link，无边框透明背景"),
            new("Hover", "指针悬停：Foreground 强化为 XY.Brush.Accent.Strong，显式下划线反馈"),
            new("Pressed", "激活按下：保持强调色，触感下压反馈"),
            new("Focus", "键盘焦点：xyui-focusable 全局焦点环接管，清晰指引"),
            new("Disabled", "禁用：IsEnabled=False，Foreground 切换至 XY.Brush.State.Disabled.Text")
        ],
        "XYUI-1-01" or "XYUI-1-02" or "XYUI-1-03" or "XYUI-1-04" or "XYUI-1-05" => [
            new("Normal", "常态可读，继承父级上下文排版与主题颜色"),
            new("Disabled", "控件或祖先处于禁用态时，前景色自动降级为禁用灰")
        ],
        _ => States(id)
    };
}

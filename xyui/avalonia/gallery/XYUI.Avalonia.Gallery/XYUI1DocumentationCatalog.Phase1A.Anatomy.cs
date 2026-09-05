namespace XYUI.Avalonia.Gallery;

public static partial class XYUI1DocumentationCatalog
{
    static IReadOnlyList<XYUIDocVariant> Phase1AAnatomy(string id) => id switch
    {
        "XYUI-1-01" => [
            new("Content", "普通正文内容", "Inline / text content role"),
            new("Typography", "继承自 Foundation 排版系统", "xy:XY.Typography"),
            new("Foreground", "默认继承全局主文本色", "Inherited from theme"),
            new("Layout", "正文流内联布局", "Auto wrap / inline flow"),
            new("Variants", "无额外变体，专注纯净正文", "None")
        ],
        "XYUI-1-02" => [
            new("Role", "表单属性与检查器键名", "Field / property key"),
            new("Alignment", "与右侧值列保持基准对齐", "Works with value column"),
            new("Layout", "单行固定或自适应宽度列", "Key column (100~120 DIP)"),
            new("Variants", "无额外变体，专用于键值布局", "None")
        ],
        "XYUI-1-03" => [
            new("Role", "次级辅助与上下文说明", "Secondary supporting info"),
            new("Emphasis", "次级灰度降低视觉干扰", "Secondary / Tertiary"),
            new("Typography", "弱化小字阶 (11~12 DIP)", "XY.Type.Caption"),
            new("Variants", "无额外变体，保持低视觉权重", "None")
        ],
        "XYUI-1-04" => [
            new("PageTitle", "页面/工作区顶级标题 (20 DIP / SemiBold)", "地图工程配置"),
            new("PanelTitle", "Dock 子面板标准标题 (16 DIP / SemiBold)", "光照贴图参数")
        ],
        "XYUI-1-05" => [
            new("Geometry", "固定高度 28 DIP 规范骨架", "Canonical component geometry"),
            new("Left Mark", "3 × 16 DIP 品牌强调竖标", "XY.Brush.Accent.Strong"),
            new("Surface", "S-05 Soft Header 浅底色容器", "XY.Brush.Surface.PanelAlt"),
            new("Variants", "无额外变体，专用于属性分组", "None")
        ],
        "XYUI-1-06" => [
            new("Content", "可交互的内联文本导航内容", "Text navigation element"),
            new("Hit Area", "贴合文字的紧凑点击区域", "Zero button padding bounds"),
            new("Focus Visual", "高辨识度无障碍焦点环", "Non-button focus outline"),
            new("Variants", "无额外变体，纯净文本交互", "None")
        ],
        _ => []
    };
}

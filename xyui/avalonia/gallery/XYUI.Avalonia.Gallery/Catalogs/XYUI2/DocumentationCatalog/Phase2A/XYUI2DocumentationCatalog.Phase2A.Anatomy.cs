namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2DocumentationCatalog
{
    static IReadOnlyList<XYUIDocVariant> Phase2AVariants(string id) => id switch
    {
        "XYUI-2-01" => [
            new("Primary", "主要动作变体，使用 3 DIP 高亮 Action Edge", "容器核心确定/提交按钮"),
            new("Secondary", "次要动作变体，使用弱化 Action Edge", "取消、返回等辅助操作"),
            new("Danger", "危险破坏性动作变体，使用红白高对比度警示色", "删除、重置、丢弃修改")
        ],
        "XYUI-2-02" => [
            new("Ghost", "默认完全透明背景，悬停显露容器底色", "视口工具栏、检视面板高频操作"),
            new("Selected", "外部状态驱动激活态，显露底色与 Action Edge", "当前激活的编辑工具/视图模式")
        ],
        "XYUI-2-03" => [
            new("Unchecked", "默认未激活态，Raised 背景无高亮 Edge", "功能未开启"),
            new("Checked", "激活态，常驻高亮 Persistent Edge 与 Surface.Selected", "功能已开启保持")
        ],
        "XYUI-2-04" => [
            new("Default", "左右双命中区共享统一 Chrome，中置 18 DIP 分割线", "默认主命令与次级菜单"),
            new("Disabled", "禁用态，两区同步衰减，阻断任何命令触发", "命令当前不可用")
        ],
        "XYUI-2-05" => [
            new("Default", "整钮单一命中区，右侧 Chevron 槽作为纯装饰", "触发格式/选项下拉菜单"),
            new("Disabled", "禁用态，文字与 Chevron 同步衰减", "下拉选项不可用")
        ],
        "XYUI-2-06" => [
            new("Unchecked", "空选框，内部无勾选标记", "未选中"),
            new("Checked", "选框内部展示 14 DIP 矢量 Check 勾选标记", "已选中"),
            new("Indeterminate", "选框内部展示水平短横条（Mixed 态）", "部分选中 / 混合状态")
        ],
        _ => []
    };

    static IReadOnlyList<XYUIDocProperty> Phase2AProperties(string id) => id switch
    {
        "XYUI-2-01" => [
            new("Variant", "XyuiButtonVariant", "Primary", "Primary / Secondary / Danger"),
            new("Icon", "XyuiVectorIcon?", "null", "可选矢量图标，与 Content 水平混排"),
            new("Command", "ICommand?", "null", "点击时执行的命令契约"),
            new("IsEnabled", "bool", "true", "控制交互启用与衰减视觉")
        ],
        "XYUI-2-02" => [
            new("Icon", "XyuiVectorIcon?", "null", "按钮展示的居中矢量图标"),
            new("IsSelected", "bool", "false", "外部驱动选中态（Selected ≠ Checked）"),
            new("AutomationProperties.Name", "string", "—", "必填：屏幕阅读器与无障碍语义名称"),
            new("Command", "ICommand?", "null", "点击时执行的命令契约")
        ],
        "XYUI-2-03" => [
            new("IsChecked", "bool?", "false", "当前选中状态，true 激活 Persistent Edge"),
            new("IsThreeState", "bool", "false", "是否启用 null 混合态"),
            new("Command", "ICommand?", "null", "状态翻转时可选通知命令")
        ],
        "XYUI-2-04" => [
            new("MainCommand", "ICommand?", "null", "主命中区（PART_MainZone）触发命令"),
            new("MainCommandParameter", "object?", "null", "主命令传递参数"),
            new("MenuCommand", "ICommand?", "null", "右侧菜单槽（PART_MenuZone）触发命令"),
            new("MenuCommandParameter", "object?", "null", "菜单命令传递参数"),
            new("MenuZoneWidth", "double", "34", "常量：右侧图标槽固定宽度 34 DIP"),
            new("DividerHeight", "double", "18", "常量：中间分割线固定高度 18 DIP")
        ],
        "XYUI-2-05" => [
            new("OpenCommand", "ICommand?", "null", "点击整钮唯一命中区触发的外部菜单命令"),
            new("OpenCommandParameter", "object?", "null", "菜单命令参数"),
            new("ChevronBrush", "IBrush?", "null", "Chevron 颜色（由样式层按状态供给）"),
            new("ChevronTrackWidth", "double", "28", "常量：Chevron 装饰槽固定宽度 28 DIP")
        ],
        "XYUI-2-06" => [
            new("IsChecked", "bool?", "false", "支持 Unchecked (false) / Checked (true) / Mixed (null)"),
            new("IsThreeState", "bool", "false", "是否启用三态循环流转"),
            new("Content", "object?", "null", "右侧说明文本或自定义控件")
        ],
        _ => []
    };
}

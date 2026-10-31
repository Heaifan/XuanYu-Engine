namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2DocumentationCatalog
{
    static string Phase2AQuickStart(string id) => id switch
    {
        "XYUI-2-01" => "<xy:XYButton Content=\"新建项目\" Variant=\"Primary\" />\n<xy:XYButton Content=\"删除\" Variant=\"Danger\" />",
        "XYUI-2-02" => "<xy:XYIconButton Icon=\"Search\" AutomationProperties.Name=\"在工程中检索\"\n  ToolTip.Tip=\"在工程中检索资源 (Ctrl+F)\" />",
        "XYUI-2-03" => "<xy:XYToggleButton Content=\"网格吸附\" IsChecked=\"{Binding Snapping}\" />",
        "XYUI-2-04" => "<xy:XYSplitButton Content=\"构建工程\"\n  MainCommand=\"{Binding BuildCmd}\" MenuCommand=\"{Binding OpenBuildMenuCmd}\" />",
        "XYUI-2-05" => "<xy:XYDropDownButton Content=\"导出资产\" OpenCommand=\"{Binding OpenExportMenuCmd}\" />",
        "XYUI-2-06" => "<xy:XYCheckbox Content=\"显示参考辅助线\" IsChecked=\"{Binding ShowGuides}\" />",
        _ => ""
    };

    static IReadOnlyList<XYUIDocRule> Phase2ACoreRules(string id) => id switch
    {
        "XYUI-2-01" => [
            new("组件定义", "标准命令触发按钮，支持 Primary、Secondary、Danger 三种变体与 Action Edge。"),
            new("适用场景", "对话框确定/取消、工具栏主操作、危险操作（删除/重置）等明确动作入口。"),
            new("禁用场景", "禁止在紧凑图标列表内使用文本按钮；不要用普通 Button 模拟开关状态。"),
            new("设计原则", "同一容器内 Primary 按钮原则上不超过一个；Secondary 使用弱化 Action Edge。")
        ],
        "XYUI-2-02" => [
            new("组件定义", "紧凑 Ghost 纯图标按钮，基于 Command 语义（Selected ≠ Checked）。"),
        new("可访问性", "硬规则：纯图标按钮必须通过 AutomationProperties.Name 提供语义名称；ToolTip.Tip 只能作为补充提示，Icon 不能替代名称。"),
            new("状态驱动", "IsSelected 纯由外部状态（如活动工具）驱动，点击自身不自动反转状态。"),
            new("适用场景", "视口工具栏、检视面板标题栏操作、代码/拷贝/搜索等高频紧凑操作点。")
        ],
        "XYUI-2-03" => [
            new("组件定义", "持久激活状态的双态/三态切换按钮，激活时展示高亮 Persistent Edge 与选中容器色。"),
            new("适用场景", "正交视图、网格吸附、光影开关等需要视觉保持按压/选中状态的场景。"),
            new("相邻区别", "vs XYIconButton：ToggleButton 每次点击自身翻转 IsChecked；IconButton 仅发命令。"),
            new("禁用场景", "不要在需要展示开关滑块的偏好设置中使用（偏好设置应使用 XYSwitch）。")
        ],
        "XYUI-2-04" => [
            new("组件定义", "双命中区拆分按钮，共享一层 Chrome，分隔线高度 18 DIP，无永久 Action Edge。"),
            new("边界契约", "核心契约：MainCommand 与 MenuCommand 互不串发；本组件不拥有 Menu/Flyout/Popup 本体。"),
            new("交互语义", "主区直接执行默认命令；右侧紧凑图标槽仅触发菜单请求，由外部宿主弹出菜单。"),
            new("键盘行为", "键盘获得焦点时，Enter/Space 默认触发 MainCommand，符合通用操作习惯。")
        ],
        "XYUI-2-05" => [
            new("组件定义", "带右侧 Chevron 装饰槽的下拉触发按钮，整钮横跨单一命中区（PART_OpenZone）。"),
            new("边界契约", "硬契约：Trigger ≠ Popup owner。整钮仅提供 OpenCommand 触发器，不内置任何浮层。"),
            new("相邻区别", "vs SplitButton：DropDownButton 无分隔线，右槽 IsHitTestVisible=false，点击任意处均开菜单。"),
            new("适用场景", "格式导出、排序规则切换、构建配置切换等需要弹出选择项的操作入口。")
        ],
        "XYUI-2-06" => [
            new("组件定义", "标准复选框，支持 Unchecked、Checked、Indeterminate (Mixed) 三种状态。"),
            new("适用场景", "图层可见性开关、包含子树的批量属性勾选、多选项并存设置。"),
            new("状态契约", "IsThreeState=true 时，点击流转 Checked → Indeterminate → Unchecked。"),
            new("相邻区别", "vs XYSwitch：复选框适用于紧凑数据表单与多选；Switch 适用于立即生效的系统设置。")
        ],
        _ => []
    };
}

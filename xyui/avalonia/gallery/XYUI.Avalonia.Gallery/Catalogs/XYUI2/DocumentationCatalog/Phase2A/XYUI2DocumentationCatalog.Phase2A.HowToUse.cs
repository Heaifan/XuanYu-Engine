namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2DocumentationCatalog
{
    static IReadOnlyList<XYUIDocGuideItem> Phase2AHowToUse(string id) => id switch
    {
        "XYUI-2-01" => [
            new("适用场景 (Use when)", "需要用户进行明确动作抉择时，如保存工程、执行编译、取消操作或删除资源。"),
            new("禁用场景 (Avoid when)", "不要在工具栏紧凑网格中使用（应使用 XYIconButton）；不要作为状态切换控件使用。"),
            new("排版推荐 (Layout)", "表单或弹窗底部排版：主要操作居右（Primary），次要操作居左或靠前（Secondary）。"),
            new("语义约束 (Semantics)", "危险删除操作必须显式标记 Variant=\"Danger\"，避免误触。")
        ],
        "XYUI-2-02" => [
            new("无障碍要求 (Accessibility)", "硬规则：纯图标必须设置 AutomationProperties.Name；ToolTip.Tip 只能补充说明，Icon 本身不能替代语义名称。"),
            new("状态设计 (Interaction)", "IsSelected 纯由业务层 ViewModel 驱动，点击按钮自身应发出 Command，由外部决定是否切换。"),
            new("排版推荐 (Layout)", "常用于 34×34 DIP 紧凑网格或横向/纵向工具栏，配合 XyuiVectorIcon 矢量库使用。"),
            new("禁用场景 (Avoid when)", "需要包含可变文字标签时，应使用带 Icon 的 XYButton，而非硬拼 TextBlock。")
        ],
        "XYUI-2-03" => [
            new("适用场景 (Use when)", "用于控制某种长期生效的工作模式，例如「对齐网格」、「深度测试」、「线框渲染」。"),
            new("禁用场景 (Avoid when)", "不要用于瞬时动作执行（瞬时动作应使用 XYButton 或 XYIconButton）。"),
            new("视觉特征 (Visual)", "激活后具有常驻的 Action Edge 强调线与选中底色，用户可一眼识别当前启用的模式。"),
            new("键盘交互 (Keyboard)", "支持 Tab 导航获得焦点，按 Space 键直接切换 IsChecked 状态。")
        ],
        "XYUI-2-04" => [
            new("命令区隔离 (Command Zones)", "主区（PART_MainZone）触发 MainCommand；右侧图标槽（PART_MenuZone）触发 MenuCommand。"),
            new("宿主契约 (No Popup Owner)", "重要：XYSplitButton 本身不包含或拥有 Popup/Menu 控件。MenuCommand 仅为动作触发器。"),
            new("交互场景 (Scenarios)", "适用于「具有常用默认动作，同时支持高级配置」的高频操作，如「运行工程」与「调试配置」。"),
            new("键盘行为 (Keyboard)", "焦点落在按钮整体时，按 Enter 或 Space 键默认触发主命令（MainCommand）。")
        ],
        "XYUI-2-05" => [
            new("触发器定位 (Trigger Only)", "硬规则：Trigger ≠ Popup owner。整钮仅承载 OpenCommand，浮层菜单由外部响应弹出。"),
            new("单一命中区 (Single Hit Zone)", "整钮横跨一个点击区，右侧 Chevron 仅是视觉装饰槽（IsHitTestVisible=false），无独立次级点击。"),
            new("与 SplitButton 区别", "DropDownButton 无纵向分割线，点击任何位置效果一致；SplitButton 有分割线且双命令隔离。"),
            new("适用场景 (Use when)", "适用于必须展开下拉列表才能做出选择的操作，例如「选择导出格式」、「筛选分类」。")
        ],
        "XYUI-2-06" => [
            new("适用场景 (Use when)", "用于独立选项的开启/关闭，或者层级树形结构中的多选与半选汇总。"),
            new("三态流转 (ThreeState)", "当含有子项时开启 IsThreeState=\"True\"，使用 null 表达部分勾选，true 表达全选。"),
            new("禁用场景 (Avoid when)", "如果开启后会立即触发异步网络同步或重型任务，建议使用带明确提示的 XYSwitch。"),
            new("键盘操作 (Keyboard)", "通过 Tab 键聚焦到复选框后，按 Space 键在各种状态间循环切换。")
        ],
        _ => []
    };
}

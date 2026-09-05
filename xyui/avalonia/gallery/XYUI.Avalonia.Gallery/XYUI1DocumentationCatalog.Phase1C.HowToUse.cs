namespace XYUI.Avalonia.Gallery;

public static partial class XYUI1DocumentationCatalog
{
    static IReadOnlyList<XYUIDocGuideItem> Phase1CHowToUse(string id) => id switch
    {
        "XYUI-1-14" => [
            new("Recommended", "按 Section / ListRow / VerticalSplit 指定变体，方向与边距由变体自动驱动。"),
            new("Advanced", "VerticalSplit 自动采用 1 DIP 宽度并填充垂直高度。"),
            new("Don't", "禁止在 XAML 中手动指定 Orientation 属性或将其作为外层 Border。")
        ],
        "XYUI-1-15" => [
            new("Recommended", "置于输入框、开关或设置项下方，提供参数格式或使用说明。"),
            new("Advanced", "禁用由 IsEnabled=\"False\" 驱动，文本与信息 Mark 同步降级。"),
            new("Don't", "不要用于强调风险警告或阻断失败。")
        ],
        "XYUI-1-16" => [
            new("Recommended", "置于非法输入项或失败操作下方，明确指出阻断原因。"),
            new("Advanced", "文本与错误 Mark 均绑定 Semantic.Error 语义色族，在 Dark 下保持纯正低刺激度。"),
            new("Don't", "不要用于普通提示，避免在整个界面滥用红色错误干扰视线。")
        ],
        "XYUI-1-17" => [
            new("Recommended", "置于可能引发性能开销、未保存或需注意的设置项下方。"),
            new("Advanced", "提示存在风险但并不阻断操作，允许用户继续下一步。"),
            new("Don't", "不要与 ErrorText 混淆，阻断性错误必须使用 ErrorText。")
        ],
        "XYUI-1-18" => [
            new("Recommended", "通过 Shortcut=\"Ctrl+Shift+S\" 传递加号连接的快捷键字符串，自动拆解独立键帽。"),
            new("Advanced", "单键 (F2)、双键 (Ctrl+S)、三键 (Ctrl+Shift+P) 均有良好视觉呈现。"),
            new("Don't", "不要当作交互按钮使用；不要手动拼接方括号或手写样式。")
        ],
        _ => []
    };
}

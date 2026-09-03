using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;

namespace XYUI.Avalonia.Interaction;

// 交互状态 → Canonical 资源键唯一真值（第二真值红线）。
// F4 不新增任何 raw 色值 / 边框宽度 / 圆角：全部消费 R3-F1/F3 已登记令牌。
// Checked 只提供 Avalonia 原生状态选择器；具体 On/Checked 视觉由组件 Canonical 决定。
public static class XyuiInteractionState
{
    public const string HoverToken = "XY.State.Color.Hover";
    public const string PressedToken = "XY.State.Color.Pressed";
    public const string SelectedToken = "XY.State.Color.Selected";
    public const string ActiveToken = "XY.State.Color.Active";
    public const string DraggingToken = "XY.State.Color.Dragging";
    public const string DropTargetBackgroundToken = "XY.State.Color.DropTarget.Background";
    public const string DropTargetBorderToken = "XY.State.Color.DropTarget.Border";
    public const string DisabledBackgroundToken = "XY.State.Disabled.Background";
    public const string DisabledTextToken = "XY.State.Disabled.Text";
    public const string DisabledBorderToken = "XY.State.Disabled.Border";
    public const string ReadOnlyBackgroundToken = "XY.State.ReadOnly.Background";
    public const string ReadOnlyTextToken = "XY.State.ReadOnly.Text";
    public const string ReadOnlyBorderToken = "XY.State.ReadOnly.Border";
    public const string LockedBackgroundToken = "XY.State.Locked.Background";
    public const string LockedTextToken = "XY.State.Locked.Text";
    public const string LockedBorderToken = "XY.State.Locked.Border";

    public const string HoverBrush = "XY.Brush.State.Color.Hover";
    public const string PressedBrush = "XY.Brush.State.Color.Pressed";
    public const string SelectedBrush = "XY.Brush.State.Color.Selected";
    public const string FocusBorderBrush = "XY.Brush.Border.Color.Focus";
    public const string SelectedBorderBrush = "XY.Brush.Border.Color.Selected";

    public const string DisabledBackground = "XY.Brush.State.Disabled.Background";
    public const string DisabledText = "XY.Brush.State.Disabled.Text";
    public const string DisabledBorder = "XY.Brush.State.Disabled.Border";

    public const string ActiveBrush = "XY.Brush.State.Color.Active";
    public const string DraggingBrush = "XY.Brush.State.Color.Dragging";
    public const string DropTargetBackground = "XY.Brush.State.Color.DropTarget.Background";
    public const string DropTargetBorder = "XY.Brush.State.Color.DropTarget.Border";
    public const string ReadOnlyBackground = "XY.Brush.State.ReadOnly.Background";
    public const string ReadOnlyText = "XY.Brush.State.ReadOnly.Text";
    public const string ReadOnlyBorder = "XY.Brush.State.ReadOnly.Border";
    public const string LockedBackground = "XY.Brush.State.Locked.Background";
    public const string LockedText = "XY.Brush.State.Locked.Text";
    public const string LockedBorder = "XY.Brush.State.Locked.Border";

    public const string FocusWidth = "XY.Border.Width.Focus";
    public const string SelectedWidth = "XY.Border.Width.Selected";

    // 依赖属性句柄：目标控件均为 TemplatedControl 派生（Button / ListBoxItem / ToggleButton / TextBox），
    // 其 Background / Foreground / BorderBrush / BorderThickness / CornerRadius 由 TemplatedControl 声明。
    public static readonly AvaloniaProperty BackgroundProperty = TemplatedControl.BackgroundProperty;
    public static readonly AvaloniaProperty ForegroundProperty = TemplatedControl.ForegroundProperty;
    public static readonly AvaloniaProperty BorderBrushProperty = TemplatedControl.BorderBrushProperty;
    public static readonly AvaloniaProperty BorderThicknessProperty = TemplatedControl.BorderThicknessProperty;
    public static readonly AvaloniaProperty CornerRadiusProperty = TemplatedControl.CornerRadiusProperty;

    // 原生状态机选择器：消费 Avalonia 原生伪类（:pointerover / :pressed / :disabled / :selected / :checked / :focus）。
    // 伪类与本类同存于 control.Classes（已反射确认 ReferenceEquals 相同集合），
    // 故用 x.Class(":pressed") 即可精确匹配原生状态，且不会在 Window 等无关控件上调用 GetValue 崩溃。
    public static readonly Func<Selector?, Selector> None = x => x!;
    public static readonly Func<Selector?, Selector> Hover =
        x => x!.Class(":pointerover");
    public static readonly Func<Selector?, Selector> Pressed =
        x => x!.Class(":pressed");
    public static readonly Func<Selector?, Selector> Disabled =
        x => x!.Class(":disabled");
    public static readonly Func<Selector?, Selector> Selected =
        x => x!.Class(":selected");
    public static readonly Func<Selector?, Selector> Checked =
        x => x!.Class(":checked");
    public static readonly Func<Selector?, Selector> Focused =
        x => x!.Class(":focus");

    // 构建单一“类 + 状态”样式：消费 Avalonia 原生状态机，不手写 IsHovered / IsPressed 等状态字段。
    // 仅按 Class 匹配（不限控件类型），使 Button / TextBox / ListBoxItem / ToggleButton 等皆命中。
    public static Style Build(string cls, Func<Selector?, Selector> state, AvaloniaProperty property, string resourceKey)
    {
        Func<Selector?, Selector> selector = x => state(x.Class(cls));
        var style = new Style(selector);
        style.Setters.Add(new Setter(property, new DynamicResourceExtension(resourceKey)));
        return style;
    }
}

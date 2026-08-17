using Avalonia.Styling;
using XYUI.Avalonia.Interaction;

namespace XYUI.Avalonia.Interaction;

// 交互状态样式集（通过 Class 契约消费）：
//   xyui-interactive  基础表面 + 悬停 / 按下 / 禁用
//   xyui-selectable   选中（PersistentBase：底色 + 选中边框环）
//   xyui-checkable    勾选（复用 Accent，GAP-R3F4-001）
//   xyui-focusable    焦点边框环（见 XyuiFocusStyles，与 Hover / Selected 视觉分离）
// 经 XYUI.Avalonia 初始化链（Gallery App）加载，Consumer 无需逐页手工注册。
//
// 优先级顺序（与 canonical 行为令牌一致）：Default < Selected < Hover < Pressed < Checked/Focus < Disabled
// 后者覆盖前者，故 Hover 底色压过 Selected，Disabled 最高压过全部。
public static class XyuiInteractionStyles
{
    public static Styles Create()
    {
        var styles = new Styles();
        AddBase(styles);
        AddSelectable(styles);
        AddHover(styles);
        AddPressed(styles);
        AddCheckable(styles);
        foreach (var s in XyuiFocusStyles.Create()) styles.Add(s);
        AddDisabled(styles);
        return styles;
    }

    private static void AddBase(Styles styles)
    {
        styles.Add(XyuiInteractionState.Build("xyui-interactive", XyuiInteractionState.None,
            XyuiInteractionState.BackgroundProperty, XyuiInteractionState.DefaultBackground));
        styles.Add(XyuiInteractionState.Build("xyui-interactive", XyuiInteractionState.None,
            XyuiInteractionState.ForegroundProperty, XyuiInteractionState.DefaultForeground));
        styles.Add(XyuiInteractionState.Build("xyui-interactive", XyuiInteractionState.None,
            XyuiInteractionState.CornerRadiusProperty, XyuiInteractionState.ControlRadius));
    }

    private static void AddSelectable(Styles styles)
    {
        styles.Add(XyuiInteractionState.Build("xyui-selectable", XyuiInteractionState.Selected,
            XyuiInteractionState.BackgroundProperty, XyuiInteractionState.SelectedBrush));
        styles.Add(XyuiInteractionState.Build("xyui-selectable", XyuiInteractionState.Selected,
            XyuiInteractionState.BorderBrushProperty, XyuiInteractionState.SelectedBorderBrush));
        styles.Add(XyuiInteractionState.Build("xyui-selectable", XyuiInteractionState.Selected,
            XyuiInteractionState.BorderThicknessProperty, XyuiInteractionState.SelectedWidth));
    }

    private static void AddHover(Styles styles) =>
        styles.Add(XyuiInteractionState.Build("xyui-interactive", XyuiInteractionState.Hover,
            XyuiInteractionState.BackgroundProperty, XyuiInteractionState.HoverBrush));

    private static void AddPressed(Styles styles) =>
        styles.Add(XyuiInteractionState.Build("xyui-interactive", XyuiInteractionState.Pressed,
            XyuiInteractionState.BackgroundProperty, XyuiInteractionState.PressedBrush));

    private static void AddCheckable(Styles styles) =>
        styles.Add(XyuiInteractionState.Build("xyui-checkable", XyuiInteractionState.Checked,
            XyuiInteractionState.BackgroundProperty, XyuiInteractionState.CheckedBrush));

    private static void AddDisabled(Styles styles)
    {
        styles.Add(XyuiInteractionState.Build("xyui-interactive", XyuiInteractionState.Disabled,
            XyuiInteractionState.BackgroundProperty, XyuiInteractionState.DisabledBackground));
        styles.Add(XyuiInteractionState.Build("xyui-interactive", XyuiInteractionState.Disabled,
            XyuiInteractionState.ForegroundProperty, XyuiInteractionState.DisabledText));
        styles.Add(XyuiInteractionState.Build("xyui-interactive", XyuiInteractionState.Disabled,
            XyuiInteractionState.BorderBrushProperty, XyuiInteractionState.DisabledBorder));
    }
}

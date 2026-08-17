using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using XYUI.Avalonia.Foundation;
using XYUI.Avalonia.Interaction;
using XYUI.Avalonia.Theme;

namespace XYUI.Avalonia.Tests;

// A 类：Canonical 映射（第二真值红线）+ B 类：运行时样式（真实控件验证七态实际生效）
[Collection("XyuiHeadless")]
public class InteractionStateTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public InteractionStateTests(XyuiHeadlessFixture fx) => _fx = fx;

    // 模拟原生状态：控件自身写入伪类（Avalonia 原生 :pointerover / :pressed 由控件自身设置，测试子类等价模拟）
    class SimButton : Button { public void Sim(string p, bool v) => PseudoClasses.Set(p, v); }

    static void Load()
    {
        var app = Application.Current!;
        if (app.Resources.ContainsKey("XYUI-F4-LOADED")) return;
        app.Resources["XYUI-F4-LOADED"] = true;
        app.Resources.MergedDictionaries.Add(XyuiTheme.CreateLight());
        app.Styles.Add(XyuiInteractionStyles.Create());
    }

    static T Show<T>(T c) where T : Control { var w = new Window { Content = c }; w.Show(); c.ApplyStyling(); return c; }
    static Color C(string id) { XyuiColorTokens.TryFind(id, out var t); return t.ToColor(false); }
    static Color Bg(TemplatedControl c) => ((SolidColorBrush)c.Background!).Color;
    static Color Bd(TemplatedControl c) => ((SolidColorBrush)c.BorderBrush!).Color;

    [Fact] public void All_State_Keys_Resolve_In_Theme() => _fx.Run(() =>
    {
        Load();
        var theme = XyuiTheme.CreateLight();
        var keys = new[] { XyuiInteractionState.DefaultBackground, XyuiInteractionState.DefaultForeground, XyuiInteractionState.ControlRadius,
            XyuiInteractionState.HoverBrush, XyuiInteractionState.PressedBrush, XyuiInteractionState.SelectedBrush, XyuiInteractionState.FocusBorderBrush,
            XyuiInteractionState.SelectedBorderBrush, XyuiInteractionState.DisabledBackground, XyuiInteractionState.DisabledText,
            XyuiInteractionState.DisabledBorder, XyuiInteractionState.CheckedBrush, XyuiInteractionState.FocusWidth, XyuiInteractionState.SelectedWidth };
        foreach (var k in keys) Assert.True(theme.ContainsKey(k), $"交互状态键 {k} 未登记（第二真值风险）");
    });
    [Fact] public void Checked_Reuses_Canonical_Accent() => _fx.Run(() =>
    {
        Assert.Equal("XY.Brush.Accent.Default", XyuiInteractionState.CheckedBrush);
        Assert.True(XyuiColorTokens.All.Any(t => XyuiColorTokens.BrushKey(t.TokenId) == XyuiInteractionState.CheckedBrush));
    });
    [Fact] public void Style_Collection_Has_Fourteen_Rules() => _fx.Run(() =>
        Assert.Equal(14, XyuiInteractionStyles.Create().Count));
    [Fact] public void Button_Default_And_Hover() => _fx.Run(() =>
    {
        Load();
        var b = Show(new SimButton { Classes = { "xyui-interactive", "xyui-focusable" }, Content = "x" });
        Assert.Equal(C("XY.Surface.Panel"), Bg(b));
        b.Sim(":pointerover", true); b.ApplyStyling();
        Assert.Equal(C("XY.State.Color.Hover"), Bg(b));
    });
    [Fact] public void Button_Pressed() => _fx.Run(() =>
    {
        Load();
        var b = Show(new SimButton { Classes = { "xyui-interactive", "xyui-focusable" }, Content = "x" });
        b.Sim(":pressed", true); b.ApplyStyling();
        Assert.Equal(C("XY.State.Color.Pressed"), Bg(b));
    });
    [Fact] public void Button_Focus_Ring() => _fx.Run(() =>
    {
        Load();
        var b = Show(new SimButton { Classes = { "xyui-interactive", "xyui-focusable" }, Content = "x" });
        b.Focus(); b.ApplyStyling();
        Assert.True(b.IsFocused);
        Assert.Equal(C("XY.Border.Color.Focus"), Bd(b));
    });
    [Fact] public void Button_Disabled_Degrades() => _fx.Run(() =>
    {
        Load();
        var b = Show(new SimButton { Classes = { "xyui-interactive", "xyui-focusable" }, Content = "x" });
        b.IsEnabled = false; b.ApplyStyling();
        Assert.Equal(C("XY.State.Disabled.Background"), Bg(b));
        Assert.Equal(C("XY.State.Disabled.Text"), ((SolidColorBrush)b.Foreground!).Color);
    });
    [Fact] public void ToggleButton_Checked_Uses_Accent() => _fx.Run(() =>
    {
        Load();
        var t = Show(new ToggleButton { Classes = { "xyui-interactive", "xyui-focusable", "xyui-checkable" }, Content = "x" });
        t.IsChecked = true; t.ApplyStyling();
        Assert.Equal(C("XY.Accent.Default"), Bg(t));
    });
    [Fact] public void ListBoxItem_Selected_Ring_And_Background() => _fx.Run(() =>
    {
        Load();
        var i = Show(new ListBoxItem { Classes = { "xyui-interactive", "xyui-focusable", "xyui-selectable" }, Content = "x" });
        i.IsSelected = true; i.ApplyStyling();
        Assert.Equal(C("XY.State.Color.Selected"), Bg(i));
        Assert.Equal(C("XY.Border.Color.Selected"), Bd(i));
    });
}

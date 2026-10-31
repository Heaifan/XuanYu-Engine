using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using XYUI.Avalonia.Foundation;
using XYUI.Avalonia.Interaction;
using XYUI.Avalonia.Theme;

namespace XYUI.Avalonia.Tests;

// C 类：状态组合 —— 优先级稳定，Hover / Selected / Focus 互不覆盖
[Collection("XyuiHeadless")]
public class InteractionCombinationTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public InteractionCombinationTests(XyuiHeadlessFixture fx) => _fx = fx;

    class SimListBoxItem : ListBoxItem { public void Sim(string p, bool v) => PseudoClasses.Set(p, v); }

    static void Load()
    {
        var app = Application.Current!;
        if (app.Resources.ContainsKey("XYUI-F4-LOADED")) return;
        app.Resources["XYUI-F4-LOADED"] = true;
        app.Resources.MergedDictionaries.Add(XyuiTheme.CreateLight());
        app.Styles.Add(XyuiInteractionStyles.Create());
    }

    static Color C(string id) { XyuiColorTokens.TryFind(id, out var t); return t.ToColor(false); }
    static Color Bg(TemplatedControl c) => ((SolidColorBrush)c.Background!).Color;
    static Color Bd(TemplatedControl c) => ((SolidColorBrush)c.BorderBrush!).Color;

    [Fact] public void Selected_Plus_Hover_Keeps_Ring_Shows_Hover() => _fx.Run(() =>
    {
        Load();
        var i = new SimListBoxItem { Classes = { "xyui-interactive", "xyui-focusable", "xyui-selectable" }, Content = "x" };
        var w = new Window { Content = i }; w.Show(); i.ApplyStyling();
        i.IsSelected = true; i.ApplyStyling();
        i.Sim(":pointerover", true); i.ApplyStyling();
        Assert.Equal(C("XY.State.Color.Hover"), Bg(i));        // Hover 底色优先于 Selected
        Assert.Equal(C("XY.Border.Color.Selected"), Bd(i));   // 选中环保留
    });

    [Fact] public void Selected_Plus_Focus_Keeps_Background_Shows_Focus_Ring() => _fx.Run(() =>
    {
        Load();
        var i = new SimListBoxItem { Classes = { "xyui-interactive", "xyui-focusable", "xyui-selectable" }, Content = "x" };
        var w = new Window { Content = i }; w.Show(); i.ApplyStyling();
        i.IsSelected = true; i.ApplyStyling();
        i.Focus(); i.ApplyStyling();
        Assert.True(i.IsFocused);
        Assert.Equal(C("XY.State.Color.Selected"), Bg(i));     // 选中底色保留
        Assert.Equal(C("XY.Border.Color.Focus"), Bd(i));       // 焦点环独立可见
    });

    [Fact] public void Checked_Plus_Focus_Keeps_Focus_Ring() => _fx.Run(() =>
    {
        Load();
        var t = new ToggleButton { Classes = { "xyui-interactive", "xyui-focusable", "xyui-checkable" }, Content = "x" };
        var w = new Window { Content = t }; w.Show(); t.ApplyStyling();
        t.IsChecked = true; t.ApplyStyling();
        t.Focus(); t.ApplyStyling();
        Assert.Equal(C("XY.Border.Color.Focus"), Bd(t));       // 焦点环独立
    });

    [Fact] public void Disabled_Overrides_Selected() => _fx.Run(() =>
    {
        Load();
        var i = new SimListBoxItem { Classes = { "xyui-interactive", "xyui-focusable", "xyui-selectable" }, Content = "x" };
        var w = new Window { Content = i }; w.Show(); i.ApplyStyling();
        i.IsSelected = true; i.IsEnabled = false; i.ApplyStyling();
        Assert.Equal(C("XY.State.Disabled.Background"), Bg(i)); // Disabled 最高优先级降级
    });
}

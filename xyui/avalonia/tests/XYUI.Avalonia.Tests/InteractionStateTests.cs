using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;
using XYUI.Avalonia.Foundation;
using XYUI.Avalonia.Interaction;
using XYUI.Avalonia.Theme;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public class InteractionStateTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public InteractionStateTests(XyuiHeadlessFixture fx) => _fx = fx;

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
    static IEnumerable<Setter> Setters() => XyuiInteractionStyles.Create().OfType<Style>()
        .SelectMany(style => style.Setters.OfType<Setter>());
    static bool HasResource(string key) => Setters().Any(setter =>
        Equals((setter.Value as DynamicResourceExtension)?.ResourceKey, key));

    [Fact] public void All_State_Keys_Resolve_In_Theme() => _fx.Run(() =>
    {
        Load();
        var theme = XyuiTheme.CreateLight();
        var keys = new[] { XyuiInteractionState.HoverBrush, XyuiInteractionState.PressedBrush, XyuiInteractionState.SelectedBrush, XyuiInteractionState.FocusBorderBrush,
            XyuiInteractionState.SelectedBorderBrush, XyuiInteractionState.DisabledBackground, XyuiInteractionState.DisabledText,
            XyuiInteractionState.DisabledBorder, XyuiInteractionState.FocusWidth, XyuiInteractionState.SelectedWidth };
        foreach (var k in keys) Assert.True(theme.ContainsKey(k), $"交互状态键 {k} 未登记（第二真值风险）");
    });
    [Fact] public void Foundation_Leaves_Default_And_Checked_Appearance_To_Components() => _fx.Run(() =>
    {
        Assert.False(HasResource("XY.Brush.Surface.Panel"));
        Assert.False(HasResource("XY.Brush.Text.Primary"));
        Assert.False(HasResource("XY.Radius.Control"));
        Assert.False(HasResource("XY.Brush.Accent.Default"));
        Assert.NotNull(XyuiInteractionState.Checked);
    });
    [Fact] public void Button_Default_And_Hover() => _fx.Run(() =>
    {
        Load();
        var b = Show(new SimButton { Classes = { "xyui-interactive", "xyui-focusable" }, Content = "x" });
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
    [Fact] public void Checked_Selector_Is_Available_For_Component_Test_Mapping() => _fx.Run(() =>
    {
        var style = XyuiInteractionState.Build("test-checkable", XyuiInteractionState.Checked,
            XyuiInteractionState.BackgroundProperty, XyuiInteractionState.HoverBrush);
        Assert.NotNull(style);
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

using Avalonia;
using Avalonia.Controls;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI4HoverStateTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI4HoverStateTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Surface_hover_uses_hover_token_without_layout_shift() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var state = new XYHoverState { Child = new TextBlock { Text = "对象" }, Width = 120, Height = 32 };
        var window = XyuiBatchTestHost.Show(state);
        var before = state.Bounds;
        state.IsHovered = true;
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Color.Hover"), XyuiBatchTestHost.ColorOf(state.Background));
        Assert.Equal(0, state.BorderThickness.Left);
        Assert.Equal(before.Size, state.Bounds.Size);
        window.Close();
    });

    [Fact]
    public void Border_and_outline_variants_keep_hover_channels_distinct() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var (border, borderWindow) = Show(XyuiHoverStateVariant.Border);
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Color.Hover"), XyuiBatchTestHost.ColorOf(border.BorderBrush));
        Assert.Equal(1, border.BorderThickness.Left);
        borderWindow.Close();
        var (outline, outlineWindow) = Show(XyuiHoverStateVariant.Outline);
        Assert.Equal(XyuiBatchTestHost.Token("XY.Accent.Default"), XyuiBatchTestHost.ColorOf(outline.BorderBrush));
        Assert.Equal(1.5, outline.BorderThickness.Left);
        outlineWindow.Close();
    });

    [Fact]
    public void Disabled_hover_does_not_apply_feedback() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var state = new XYHoverState { IsEnabled = false, IsHovered = true, Child = new TextBlock { Text = "禁用" } };
        var window = XyuiBatchTestHost.Show(state);
        Assert.Equal(0, state.BorderThickness.Left);
        Assert.NotEqual(XyuiBatchTestHost.Token("XY.State.Color.Hover"), XyuiBatchTestHost.ColorOf(state.Background));
        window.Close();
    });

    static (XYHoverState State, Window Window) Show(XyuiHoverStateVariant variant)
    {
        var state = new XYHoverState { Variant = variant, IsHovered = true, Child = new TextBlock { Text = "对象" } };
        var window = XyuiBatchTestHost.Show(state);
        return (state, window);
    }
}

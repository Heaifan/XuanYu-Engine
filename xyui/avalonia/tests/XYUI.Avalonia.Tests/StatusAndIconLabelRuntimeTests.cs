using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;
using VectorPath = Avalonia.Controls.Shapes.Path;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class StatusAndIconLabelRuntimeTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public StatusAndIconLabelRuntimeTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void StatusBadge_and_dot_share_semantic_state_source_and_disabled_tone() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var badge = new XYStatusBadge { Text = "已编译", State = XyuiStatusState.Success };
        var dot = new XYStatusDot { State = XyuiStatusState.Success };
        var window = XyuiBatchTestHost.Show(new StackPanel { Children = { badge, dot } });
        var text = badge.GetVisualDescendants().OfType<TextBlock>().Single();
        var mark = badge.GetVisualDescendants().OfType<VectorPath>().Single();
        var success = XyuiBatchTestHost.Token("XY.Semantic.Success.Text");
        Assert.Equal(success, XyuiBatchTestHost.ColorOf(text.Foreground)); Assert.Equal(success, XyuiBatchTestHost.ColorOf(dot.Background));
        Assert.Equal(success, XyuiBatchTestHost.ColorOf(mark.Fill));
        badge.IsEnabled = false; dot.IsEnabled = false;
        var disabled = XyuiBatchTestHost.Token("XY.State.Disabled.Text");
        Assert.Equal(disabled, XyuiBatchTestHost.ColorOf(text.Foreground)); Assert.Equal(disabled, XyuiBatchTestHost.ColorOf(dot.Background)); window.Close();
    });

    [Theory]
    [InlineData(XyuiStatusState.Success, "XY.Semantic.Success.Text", "XY.Semantic.Success.Text")]
    [InlineData(XyuiStatusState.Warning, "XY.Semantic.Warning.Text", "XY.Semantic.Warning.Text")]
    [InlineData(XyuiStatusState.Error, "XY.Semantic.Error.Text", "XY.Semantic.Error.Text")]
    [InlineData(XyuiStatusState.Info, "XY.Semantic.Info.Text", "XY.Semantic.Info.Text")]
    [InlineData(XyuiStatusState.Neutral, "XY.Text.Secondary", "XY.Text.Tertiary")]
    public void Every_status_state_uses_canonical_badge_and_dot_tokens(XyuiStatusState state, string badgeToken, string dotToken) => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var badge = new XYStatusBadge { Text = "状态", State = state }; var dot = new XYStatusDot { State = state };
        var window = XyuiBatchTestHost.Show(new StackPanel { Children = { badge, dot } });
        var text = badge.GetVisualDescendants().OfType<TextBlock>().Single();
        Assert.Equal(XyuiBatchTestHost.Token(badgeToken), XyuiBatchTestHost.ColorOf(text.Foreground));
        Assert.Equal(XyuiBatchTestHost.Token(dotToken), XyuiBatchTestHost.ColorOf(dot.Background)); window.Close();
    });

    [Fact]
    public void IconLabel_reuses_xyicon_and_text_with_shared_alignment_and_disabled_tone() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var label = new XYIconLabel { Icon = XyuiVectorIcon.Code, Label = "Scene" };
        var window = XyuiBatchTestHost.Show(label);
        var icon = label.GetVisualDescendants().OfType<XYIcon>().Single(); Assert.Same(label.IconPart, icon);
        var text = label.GetVisualDescendants().OfType<TextBlock>().Single(x => x.Classes.Contains("xyui-icon-label-text"));
        Assert.Equal(XyuiVectorIcon.Code, icon.Icon); Assert.Equal(XyuiIconSize.Small, icon.Size); Assert.Equal("Scene", text.Text);
        Assert.Equal(XyuiBatchTestHost.Token("XY.Text.Primary"), XyuiBatchTestHost.ColorOf(text.Foreground));
        Assert.Equal(XyuiBatchTestHost.Token("XY.Text.Secondary"), XyuiBatchTestHost.ColorOf(icon.Stroke));
        Assert.InRange(Math.Abs(icon.Bounds.Center.Y - text.Bounds.Center.Y), 0, 0.5);
        label.IsEnabled = false;
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Disabled.Text"), XyuiBatchTestHost.ColorOf(text.Foreground));
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Disabled.Text"), XyuiBatchTestHost.ColorOf(icon.Stroke)); window.Close();
    });
}

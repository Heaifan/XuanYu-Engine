using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Foundation;
using VectorPath = Avalonia.Controls.Shapes.Path;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class Phase1CFeedbackRuntimeTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public Phase1CFeedbackRuntimeTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Feedback_controls_share_vector_surface_and_semantic_sources() => _fx.Run(() =>
    {
        var app = XyuiBatchTestHost.Prepare(); app.RequestedThemeVariant = ThemeVariant.Light;
        var cases = new (XyuiVectorTextSurface Control, string TextClass, string MarkClass, string TextToken, string MarkToken)[]
        {
            (new XYHelpText { Text = "说明" }, "xyui-help-text-text", "xyui-help-text-mark", "XY.Text.Secondary", "XY.Semantic.Info.Text"),
            (new XYErrorText { Text = "错误" }, "xyui-error-text-text", "xyui-error-text-mark", "XY.Semantic.Error.Text", "XY.Semantic.Error.Text"),
            (new XYWarningText { Text = "警告" }, "xyui-warning-text-text", "xyui-warning-text-mark", "XY.Semantic.Warning.Text", "XY.Semantic.Warning.Text"),
        };
        var windows = cases.Select(x => XyuiBatchTestHost.Show(x.Control)).ToArray();
        foreach (var item in cases)
        {
            Assert.IsAssignableFrom<XyuiVectorTextSurface>(item.Control);
            Assert.Empty(item.Control.GetVisualDescendants().OfType<XYIcon>());
            var text = item.Control.GetVisualDescendants().OfType<TextBlock>().Single(x => x.Classes.Contains(item.TextClass));
            var mark = item.Control.GetVisualDescendants().OfType<VectorPath>().Single(x => x.Classes.Contains(item.MarkClass));
            Assert.Equal(XyuiBatchTestHost.Token(item.TextToken), XyuiBatchTestHost.ColorOf(text.Foreground));
            Assert.Equal(XyuiBatchTestHost.Token(item.MarkToken), XyuiBatchTestHost.ColorOf(mark.Stroke));
        }
        app.RequestedThemeVariant = ThemeVariant.Dark;
        Assert.Equal(XyuiBatchTestHost.Token("XY.Semantic.Error.Text", true), TextColor(cases[1].Control, "xyui-error-text-text"));
        Assert.Equal(XyuiBatchTestHost.Token("XY.Semantic.Warning.Text", true), MarkColor(cases[2].Control, "xyui-warning-text-mark"));
        foreach (var window in windows) window.Close(); app.RequestedThemeVariant = ThemeVariant.Light;
    });

    [Fact]
    public void Feedback_text_and_mark_share_disabled_token() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var controls = new XyuiVectorTextSurface[] { new XYHelpText(), new XYErrorText(), new XYWarningText() };
        var windows = controls.Select(x => { x.IsEnabled = false; return XyuiBatchTestHost.Show(x); }).ToArray();
        var expected = XyuiBatchTestHost.Token("XY.State.Disabled.Text");
        foreach (var control in controls)
        {
            Assert.Equal(expected, TextColor(control, control.Classes.Single(x => x.StartsWith("xyui-") && x.EndsWith("text")) + "-text"));
            Assert.Equal(expected, XyuiBatchTestHost.ColorOf(control.GetVisualDescendants().OfType<VectorPath>().Single().Stroke));
        }
        foreach (var window in windows) window.Close();
    });

    static Color TextColor(XyuiVectorTextSurface control, string className) => XyuiBatchTestHost.ColorOf(control.GetVisualDescendants().OfType<TextBlock>().Single(x => x.Classes.Contains(className)).Foreground);
    static Color MarkColor(XyuiVectorTextSurface control, string className) => XyuiBatchTestHost.ColorOf(control.GetVisualDescendants().OfType<VectorPath>().Single(x => x.Classes.Contains(className)).Stroke);
}

using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Threading;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticViewportInputPassthroughTests
{
    readonly UiHeadlessFixture _fixture;
    public DiagnosticViewportInputPassthroughTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Viewport_probe_uses_card_popup_without_native_highlight_popup()
    {
        _fixture.Run(() =>
        {
            var target = new Button { Width = 120, Height = 30 }; var host = new DiagnosticOverlayHost();
            var window = new Window { Width = 320, Height = 200,
                Content = new Grid { Children = { target, host } } };
            window.Show(); window.UpdateLayout(); Dispatcher.UIThread.RunJobs();
            host.SetProbeResult(new DiagnosticProbeResult(target, target, "XYE.VIEWPORT",
                "N/A", "VulkanViewport", "N/A", "N/A", "XYE.VIEWPORT", true, true,
                target.Bounds, DiagnosticProbeMode.Semantic));
            Dispatcher.UIThread.RunJobs();
            var card = (Popup)Field(host, "_nativeCardPopup")!.GetValue(host)!;
            var highlight = (Popup?)Field(host, "_nativeHighlightPopup")!.GetValue(host);
            Assert.True(card.IsOpen); Assert.True(highlight is null || !highlight.IsOpen);
            window.Close();
        });
    }

    [Fact]
    public void Floating_layer_is_not_a_window_input_target()
    {
        _fixture.Run(() =>
        {
            var host = new DiagnosticOverlayHost();
            var window = new Window { Width = 320, Height = 200,
                Content = new Grid { Children = { host } } };
            window.Show(); window.UpdateLayout(); Dispatcher.UIThread.RunJobs();
            var layer = (Control)Field(host, "_floatingLayer")!.GetValue(host)!;
            Assert.False(layer.IsHitTestVisible); window.Close();
        });
    }

    static FieldInfo? Field(object target, string name) => target.GetType().GetField(name,
        BindingFlags.Instance | BindingFlags.NonPublic);
}

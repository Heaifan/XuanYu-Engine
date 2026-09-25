using System.Reflection;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticViewportInputPassthroughTests
{
    readonly UiHeadlessFixture _fixture;
    public DiagnosticViewportInputPassthroughTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Viewport_probe_uses_tool_window_without_native_popups()
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
            var tool = (DiagnosticFloatingToolWindow)Field(host, "_toolWindow")!.GetValue(host)!;
            Assert.True(tool.IsVisible); Assert.Empty(host.GetVisualDescendants().OfType<Avalonia.Controls.Primitives.Popup>());
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

using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Threading;
using Avalonia.VisualTree;
using System.Reflection;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticNativeOverlayRuntimeTests
{
    readonly UiHeadlessFixture _fixture;
    public DiagnosticNativeOverlayRuntimeTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Probe_card_is_hosted_by_one_owned_tool_window()
    {
        _fixture.Run(() =>
        {
            var target = new Button { Width = 120, Height = 30 };
            var host = new DiagnosticOverlayHost();
            var window = new Window { Width = 320, Height = 200, Content = new Grid { Children = { target, host } } };
            window.Show(); window.UpdateLayout(); Dispatcher.UIThread.RunJobs();
            host.SetProbeResult(new DiagnosticProbeResult(target, target, "N/A", "N/A",
                "Button", "N/A", "N/A", "N/A", true, true, target.Bounds, DiagnosticProbeMode.Semantic));
            var tool = (DiagnosticFloatingToolWindow)Field(host, "_toolWindow")!;
            Assert.True(tool.IsVisible);
            Assert.False(tool.CanResize);
            Assert.False(tool.ShowInTaskbar);
            Assert.Equal(WindowDecorations.None, tool.WindowDecorations);
            Assert.IsType<DiagnosticFloatingCard>(tool.Content);
            Assert.Empty(host.GetVisualDescendants().OfType<Avalonia.Controls.Primitives.Popup>());
            window.Close();
        });
    }

    [Fact]
    public void Target_changes_reuse_the_same_tool_window_and_card()
    {
        _fixture.Run(() =>
        {
            var target = new Button { Width = 120, Height = 30 };
            var host = new DiagnosticOverlayHost();
            var window = new Window { Width = 320, Height = 200,
                Content = new Grid { Children = { target, host } } };
            window.Show(); window.UpdateLayout(); Dispatcher.UIThread.RunJobs();
            host.SetProbeResult(DiagnosticProbeResolver.Resolve(target)); Dispatcher.UIThread.RunJobs();
            var first = (DiagnosticFloatingToolWindow)Field(host, "_toolWindow")!;
            var card = first.Content;
            var second = new TextBlock { Text = "Second" };
            ((Grid)window.Content!).Children.Add(second);
            host.SetProbeResult(DiagnosticProbeResolver.Resolve(second));
            Dispatcher.UIThread.RunJobs();
            Assert.Same(first, Field(host, "_toolWindow"));
            Assert.Same(card, first.Content);
            window.Close();
        });
    }

    static object? Field(object target, string name) => typeof(DiagnosticOverlayHost)
        .GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(target);
}

using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticProbeOverlayRuntimeTests
{
    readonly UiHeadlessFixture _fixture;

    public DiagnosticProbeOverlayRuntimeTests(UiHeadlessFixture fixture) => _fixture = fixture;
    [Fact]
    public void Probe_keeps_at_most_one_highlight_when_target_changes()
    {
        _fixture.Run(() =>
        {
            var first = Target(100, 30); var second = Target(140, 32);
            var host = new DiagnosticOverlayHost(); var window = Show(host, first, second);
            host.SetProbeResult(Result(first)); host.SetProbeResult(Result(second));
            Assert.Equal(1, host.ActiveProbeHighlightCount);
            Assert.Equal(1, host.ActiveProbeCardCount); window.Close();
        });
    }
    [Fact]
    public void Probe_does_not_change_target_bounds_or_scroll_extent()
    {
        _fixture.Run(() =>
        {
            var target = Target(120, 30); var stack = new StackPanel { Height = 600, Children = { target } };
            var scroll = new ScrollViewer { Content = stack }; var host = new DiagnosticOverlayHost();
            var window = new Window { Width = 320, Height = 200, Content = new Grid { Children = { scroll, host } } };
            window.Show(); window.UpdateLayout(); var bounds = target.Bounds; var extent = scroll.Extent;
            host.SetProbeResult(Result(target)); Dispatcher.UIThread.RunJobs(); window.UpdateLayout();
            Assert.Equal(bounds, target.Bounds); Assert.Equal(extent, scroll.Extent);
            Assert.DoesNotContain(host.GetVisualDescendants(), x => ReferenceEquals(x, target)); window.Close();
        });
    }
    [Fact]
    public void Probe_clear_removes_highlight_and_creates_no_popup_or_window()
    {
        _fixture.Run(() =>
        {
            var target = Target(120, 30); var host = new DiagnosticOverlayHost(); var window = Show(host, target);
            var topLevels = TopLevel.GetTopLevel(host) is null ? 0 : 1;
            host.SetProbeResult(Result(target)); host.SetProbeResult(null);
            Assert.Equal((0, 0), (host.ActiveProbeHighlightCount, host.ActiveProbeCardCount));
            Assert.Empty(host.GetVisualDescendants().OfType<Avalonia.Controls.Primitives.Popup>());
            Assert.Equal(topLevels, TopLevel.GetTopLevel(host) is null ? 0 : 1); window.Close();
        });
    }
    [Fact]
    public void Probe_hides_when_window_is_minimized_and_recovers_on_activation()
    {
        _fixture.Run(() =>
        {
            var target = Target(120, 30); var host = new DiagnosticOverlayHost(); var window = Show(host, target);
            host.SetProbeResult(Result(target)); Assert.Equal(1, host.ActiveProbeHighlightCount);
            window.WindowState = WindowState.Minimized; Dispatcher.UIThread.RunJobs();
            Assert.Equal(0, host.ActiveProbeHighlightCount); window.WindowState = WindowState.Normal;
            window.Activate(); Dispatcher.UIThread.RunJobs(); Assert.Equal(1, host.ActiveProbeHighlightCount); window.Close();
        });
    }
    [Fact]
    public void Probe_clears_when_diagnostic_mode_turns_off_and_stays_cleared_on_activation()
    {
        _fixture.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            var target = Target(120, 30); var host = new DiagnosticOverlayHost();
            var window = new Window { Width = 320, Height = 200, DataContext = vm,
                Content = new Grid { Children = { target, host } } };
            host.DataContext = vm;
            window.Show(); window.UpdateLayout(); vm.RunCommand.Execute("诊断模式");
            Dispatcher.UIThread.RunJobs(); window.UpdateLayout();
            host.SetProbeResult(Result(target));
            Assert.Equal(1, host.ActiveProbeHighlightCount);
            vm.RunCommand.Execute("诊断模式"); Dispatcher.UIThread.RunJobs();
            Assert.Equal(0, host.ActiveProbeHighlightCount);
            window.Activate(); Dispatcher.UIThread.RunJobs();
            Assert.Equal(0, host.ActiveProbeHighlightCount); window.Close();
        });
    }

    static Window Show(DiagnosticOverlayHost host, params Control[] controls)
    {
        var content = new Grid(); foreach (var control in controls) content.Children.Add(control); content.Children.Add(host);
        var window = new Window { Width = 320, Height = 200, Content = content }; window.Show(); window.UpdateLayout(); Dispatcher.UIThread.RunJobs(); return window;
    }

    static Border Target(double width, double height) => new() { Width = width, Height = height };

    static DiagnosticProbeResult Result(Control target) => new(target, target, "N/A", "N/A", target.GetType().Name,
        "N/A", "N/A", "N/A", true, true, target.Bounds, DiagnosticProbeMode.Semantic);

}

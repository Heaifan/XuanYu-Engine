using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XuanYu.Editor.UI;
namespace XuanYu.World.Tests.UiRuntime;
[Collection("UiRuntime")]
public sealed class DiagnosticProbeInteractionTests
{
    readonly UiHeadlessFixture _fixture;
    public DiagnosticProbeInteractionTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Diagnostic_mode_enables_probe_without_second_mode()
    {
        var state = _fixture.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            vm.RunCommand.Execute("诊断模式"); return vm.IsDiagnosticMode;
        });
        Assert.True(state);
    }

    [Fact]
    public void Turning_diagnostic_mode_off_disables_probe()
    {
        var state = _fixture.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            vm.RunCommand.Execute("诊断模式"); vm.RunCommand.Execute("诊断模式"); return vm.IsDiagnosticMode;
        });
        Assert.False(state);
    }

    [Fact]
    public void Hover_uses_semantic_and_alt_uses_deep_visual()
    {
        var result = _fixture.Run(() =>
        {
            var button = new Button { Content = new TextBlock { Text = "保存" } };
            var host = new DiagnosticOverlayHost(); var vm = new UiVm(null, seedInitialScene: false);
            vm.RunCommand.Execute("诊断模式");
            host.DataContext = vm; var window = Show(button, host);
            var text = button.GetVisualDescendants().OfType<TextBlock>().Single();
            host.ProbeHover(text, false); var semantic = host.CurrentProbeResult;
            host.ProbeHover(text, true); var deep = host.CurrentProbeResult;
            return (semantic?.ProbeMode, semantic?.SemanticTarget, deep?.ProbeMode, deep?.DeepVisual);
        });
        Assert.Equal(DiagnosticProbeMode.Semantic, result.Item1);
        Assert.NotNull(result.Item2); Assert.Equal(DiagnosticProbeMode.DeepVisual, result.Item3);
        Assert.NotNull(result.Item4);
    }

    [Fact]
    public void Click_copies_element_diagnostic_and_escape_restores_input()
    {
        var clipboard = new FakeClipboard();
        var state = _fixture.Run(() =>
        {
            var button = new Button { Content = "保存" }; var host = new DiagnosticOverlayHost(clipboard);
            var vm = new UiVm(null, seedInitialScene: false); vm.RunCommand.Execute("诊断模式");
            host.DataContext = vm; var window = Show(button, host);
            host.ProbeHover(button, false); host.ProbeClick().GetAwaiter().GetResult(); host.ExitProbe();
            return (clipboard.Text, host.ActiveProbeHighlightCount);
        });
        Assert.Contains("[XYEngine UI 诊断报告]", state.Text);
        Assert.Contains("XYUI索引：", state.Text);
        Assert.Equal(0, state.ActiveProbeHighlightCount);
    }

    [Fact]
    public void Probe_does_not_change_layout_bounds()
    {
        var bounds = _fixture.Run(() =>
        {
            var button = new Button { Width = 120, Height = 32 }; var host = new DiagnosticOverlayHost();
            var vm = new UiVm(null, seedInitialScene: false); vm.RunCommand.Execute("诊断模式");
            host.DataContext = vm; var window = Show(button, host);
            var before = button.Bounds; host.ProbeHover(button, false); return (before, button.Bounds);
        });
        Assert.Equal(bounds.before, bounds.Item2);
    }

    static Window Show(Control target, DiagnosticOverlayHost host)
    {
        var window = new Window { Width = 320, Height = 180, Content = new Grid { Children = { target, host } } };
        window.Show(); window.UpdateLayout(); Dispatcher.UIThread.RunJobs(); return window;
    }

    sealed class FakeClipboard : IDiagnosticClipboard
    {
        public string Text { get; private set; } = string.Empty;
        public Task SetTextAsync(Control target, string text) { Text = text; return Task.CompletedTask; }
    }
}

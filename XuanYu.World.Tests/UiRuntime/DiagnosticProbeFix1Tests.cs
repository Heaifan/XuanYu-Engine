using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticProbeFix1Tests
{
    readonly UiHeadlessFixture _fixture;
    public DiagnosticProbeFix1Tests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Diagnostic_mode_alone_observes_hover_without_probe_toggle()
    {
        var result = _fixture.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); vm.RunCommand.Execute("诊断模式");
            var host = new DiagnosticOverlayHost(); var button = new Button { Content = "观察" };
            host.DataContext = vm; var window = Show(button, host); host.ProbeHover(button, false);
            var result = host.CurrentProbeResult; window.Close(); return result;
        });
        Assert.NotNull(result);
    }

    [Fact]
    public void Diagnostic_probe_does_not_consume_normal_button_click()
    {
        var count = _fixture.Run(() =>
        {
            var command = new ProbeCommand(); var button = new Button { Content = "保存", Command = command };
            var host = new DiagnosticOverlayHost(); var vm = new UiVm(null, seedInitialScene: false);
            vm.RunCommand.Execute("诊断模式"); vm.RunCommand.Execute("元素拾取"); host.DataContext = vm;
            var window = Show(button, host); var point = button.TranslatePoint(new Point(4, 4), window)!.Value;
            window.MouseDown(point, MouseButton.Left); window.MouseUp(point, MouseButton.Left);
            Dispatcher.UIThread.RunJobs(); window.Close(); return command.Count;
        });
        Assert.Equal(1, count);
    }

    [Fact]
    public void Locked_card_is_attached_to_editor_overlay_layer()
    {
        _fixture.Run(() =>
        {
            var target = new Border { Width = 120, Height = 30 }; var host = new DiagnosticOverlayHost();
            var window = new Window { Width = 320, Height = 200,
                Content = new Grid { Children = { target, host } } };
            window.Show(); window.UpdateLayout(); Dispatcher.UIThread.RunJobs();
            host.SetProbeResult(DiagnosticProbeResolver.Resolve(target));
            host.LockProbe();
            var field = typeof(DiagnosticOverlayHost).GetField("_probeCard",
                BindingFlags.Instance | BindingFlags.NonPublic);
            var card = field?.GetValue(host) as Control;
            Assert.NotNull(card); Assert.Equal("Canvas", card!.Parent?.GetType().Name); window.Close();
        });
    }

    static Window Show(Control target, DiagnosticOverlayHost host)
    {
        var window = new Window { Width = 320, Height = 180,
            Content = new Grid { Children = { target, host } } };
        window.Show(); window.UpdateLayout(); Dispatcher.UIThread.RunJobs(); return window;
    }

    sealed class ProbeCommand : System.Windows.Input.ICommand
    {
        public int Count { get; private set; }
        public event EventHandler? CanExecuteChanged { add { } remove { } }
        public bool CanExecute(object? _) => true;
        public void Execute(object? _) => Count++;
    }
}

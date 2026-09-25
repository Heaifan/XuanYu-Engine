using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticFix13ClickTrackTests
{
    readonly UiHeadlessFixture _fixture;
    public DiagnosticFix13ClickTrackTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Click_locks_and_second_click_switches_without_consuming_button_input()
    {
        var result = _fixture.Run(() =>
        {
            var command = new ProbeCommand();
            var first = new Button { Content = "文件", Command = command };
            var second = new Button { Content = "点标记" };
            var host = new DiagnosticOverlayHost(new FakeClipboard());
            var vm = new UiVm(null, seedInitialScene: false);
            vm.RunCommand.Execute("诊断模式"); host.DataContext = vm;
            var window = Show(new StackPanel { Children = { first, second, host } }, host);
            Click(window, first); var firstLock = host.LockedProbeResult;
            var card = (DiagnosticFloatingCard)typeof(DiagnosticOverlayHost)
                .GetField("_toolCard", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(host)!;
            host.ProbeHover(card, false); var cardProbe = host.CurrentProbeResult;
            host.ProbeHover(second, false); var hoverLock = host.LockedProbeResult;
            Click(window, second); var secondLock = host.LockedProbeResult;
            window.Close(); return (firstLock, cardProbe, hoverLock, secondLock, command.Count);
        });
        Assert.Same(result.firstLock?.DeepVisual, result.cardProbe?.DeepVisual);
        Assert.Same(result.firstLock?.DeepVisual, result.hoverLock?.DeepVisual);
        Assert.NotSame(result.firstLock?.DeepVisual, result.secondLock?.DeepVisual);
        Assert.Equal(1, result.Count);
    }

    static Window Show(Control content, DiagnosticOverlayHost host)
    {
        var window = new Window { Width = 320, Height = 180, Content = content };
        window.Show(); window.UpdateLayout(); Dispatcher.UIThread.RunJobs(); return window;
    }

    static void Click(Window window, Control target)
    {
        var point = target.TranslatePoint(new Point(4, 4), window)!.Value;
        window.MouseDown(point, MouseButton.Left); window.MouseUp(point, MouseButton.Left);
        Dispatcher.UIThread.RunJobs();
    }

    sealed class FakeClipboard : IDiagnosticClipboard
    { public Task SetTextAsync(Control _, string text) => Task.CompletedTask; }
    sealed class ProbeCommand : System.Windows.Input.ICommand
    {
        public int Count { get; private set; }
        public event EventHandler? CanExecuteChanged { add { } remove { } }
        public bool CanExecute(object? _) => true;
        public void Execute(object? _) => Count++;
    }
}

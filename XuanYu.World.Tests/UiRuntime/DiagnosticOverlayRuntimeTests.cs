using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;
[Collection("UiRuntime")]
public sealed class DiagnosticOverlayRuntimeTests : IDisposable
{
    readonly UiHeadlessFixture _fixture;
    public DiagnosticOverlayRuntimeTests(UiHeadlessFixture fixture)
    { _fixture = fixture; DiagnosticRegistry.Clear(); }
    public void Dispose() => DiagnosticRegistry.Clear();
    [Fact]
    public void Toggle_command_changes_false_true_false_and_notifies()
    {
        var vm = new UiVm(null, seedInitialScene: false); var changes = new List<string?>();
        vm.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
        Assert.False(vm.IsDiagnosticMode);
        vm.RunCommand.Execute("诊断模式"); Assert.True(vm.IsDiagnosticMode);
        vm.RunCommand.Execute("诊断模式"); Assert.False(vm.IsDiagnosticMode);
        Assert.Equal(2, changes.Count(x => x == nameof(UiVm.IsDiagnosticMode)));
    }

    [Fact]
    public void Overlay_preserves_target_bounds_and_scroll_extent()
    {
        _fixture.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            var target = Target(); var stack = new StackPanel { Height = 600, Children = { target } };
            var scroll = new ScrollViewer { Content = stack }; var host = new DiagnosticOverlayHost();
            var window = new Window { Width = 320, Height = 200, DataContext = vm,
                Content = new Grid { Children = { scroll, host } } };
            window.Show(); window.UpdateLayout(); var bounds = target.Bounds; var extent = scroll.Extent;
            vm.RunCommand.Execute("诊断模式"); Dispatcher.UIThread.RunJobs(); window.UpdateLayout();
            Assert.Equal(bounds, target.Bounds); Assert.Equal(extent, scroll.Extent);
            var popup = UiRuntimeTestHost.Descendants<Avalonia.Controls.Primitives.Popup>(host).Single();
            Assert.False(popup.ShouldUseOverlayLayer); Assert.False(popup.TakesFocusFromNativeControl);
            Assert.Equal((1, 1), (host.ActivePopupCount, host.ActiveBadgeCount)); window.Close();
        });
    }

    [Fact]
    public void Badge_click_copies_id_and_shift_click_copies_snapshot()
    {
        _fixture.Run(() =>
        {
            var target = Target(); var clipboard = new FakeClipboard();
            var badge = new DiagnosticBadge(target, clipboard);
            var window = new Window { Width = 320, Height = 160,
                DataContext = new UiVm(null, seedInitialScene: false),
                Content = new StackPanel { Children = { target, badge } } };
            window.Show(); window.UpdateLayout(); Click(window, badge, RawInputModifiers.None);
            Click(window, badge, RawInputModifiers.Shift);
            Assert.Equal("XYE.TEST.TARGET", clipboard.Values[0]);
            Assert.StartsWith("[XYengine Diagnostic]", clipboard.Values[1]);
            Assert.Contains("DebugId: XYE.TEST.TARGET", clipboard.Values[1]); window.Close();
        });
    }

    [Fact]
    public void Normal_button_stays_executable_and_disabling_removes_popups()
    {
        _fixture.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); var command = new ProbeCommand();
            var target = Target(); var button = new Button { Content = "普通", Command = command };
            var host = new DiagnosticOverlayHost(); var panel = new StackPanel { Children = { target, button } };
            var window = new Window { Width = 320, Height = 180, DataContext = vm,
                Content = new Grid { Children = { panel, host } } };
            window.Show(); window.UpdateLayout(); vm.RunCommand.Execute("诊断模式");
            Dispatcher.UIThread.RunJobs(); Click(window, button, RawInputModifiers.None);
            Assert.Equal(1, command.Count); Assert.Equal(1, host.ActivePopupCount);
            vm.RunCommand.Execute("诊断模式"); Dispatcher.UIThread.RunJobs();
            Assert.Equal((0, 0), (host.ActivePopupCount, host.ActiveBadgeCount)); window.Close();
        });
    }

    static Border Target()
    {
        var target = new Border { Width = 120, Height = 30 };
        XYDiagnostic.SetDebugId(target, "XYE.TEST.TARGET"); DiagnosticRegistry.Register(target); return target;
    }

    static void Click(Window window, Control control, RawInputModifiers modifiers)
    {
        var point = control.TranslatePoint(new Point(4, 4), window)!.Value;
        window.MouseDown(point, MouseButton.Left, modifiers); window.MouseUp(point, MouseButton.Left, modifiers);
        Dispatcher.UIThread.RunJobs();
    }

    sealed class FakeClipboard : IDiagnosticClipboard
    { public List<string> Values { get; } = []; public Task SetTextAsync(Control _, string text) { Values.Add(text); return Task.CompletedTask; } }
    sealed class ProbeCommand : ICommand
    { public int Count { get; private set; } public event EventHandler? CanExecuteChanged { add { } remove { } } public bool CanExecute(object? _) => true; public void Execute(object? _) => Count++; }
}

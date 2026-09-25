using Avalonia.Controls;
using Avalonia.Threading;
using System.Reflection;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticNativeDialogEdgeTests
{
    readonly UiHeadlessFixture _fixture;
    public DiagnosticNativeDialogEdgeTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Diagnostic_mode_off_during_suspend_does_not_restore()
    {
        _fixture.Run(() =>
        {
            var (window, host, target, vm) = Open();
            host.TrackProbe(DiagnosticProbeResolver.Resolve(target));
            host.SuspendForNativeDialog();
            vm.RunCommand.Execute("诊断模式");
            host.RestoreAfterNativeDialog();
            Assert.Equal(0, host.ActiveProbeCardCount);
            window.Close();
        });
    }

    [Fact]
    public void Owner_close_during_suspend_does_not_restore_tool_window()
    {
        _fixture.Run(() =>
        {
            var (window, host, target, _) = Open();
            host.TrackProbe(DiagnosticProbeResolver.Resolve(target));
            host.SuspendForNativeDialog();
            window.Close();
            host.RestoreAfterNativeDialog();
            Assert.Equal(0, host.ActiveProbeCardCount);
        });
    }

    [Fact]
    public void Ten_suspend_restore_cycles_reuse_one_tool_window()
    {
        _fixture.Run(() =>
        {
            var (window, host, target, _) = Open();
            host.TrackProbe(DiagnosticProbeResolver.Resolve(target));
            var field = typeof(DiagnosticOverlayHost).GetField("_toolWindow",
                BindingFlags.Instance | BindingFlags.NonPublic)!;
            var first = field.GetValue(host);
            for (var i = 0; i < 10; i++)
            {
                host.SuspendForNativeDialog();
                host.RestoreAfterNativeDialog();
            }
            Assert.Same(first, field.GetValue(host));
            Assert.Equal(1, host.ActiveProbeCardCount);
            window.Close();
        });
    }

    static (Window, DiagnosticOverlayHost, Button, UiVm) Open()
    {
        var vm = new UiVm(null, seedInitialScene: false);
        vm.RunCommand.Execute("诊断模式");
        var target = new Button { Name = "Target", Width = 120, Height = 30 };
        var host = new DiagnosticOverlayHost { DataContext = vm };
        var window = new Window { Width = 480, Height = 300,
            Content = new Grid { Children = { target, host } } };
        window.Show(); window.UpdateLayout(); Dispatcher.UIThread.RunJobs();
        return (window, host, target, vm);
    }
}

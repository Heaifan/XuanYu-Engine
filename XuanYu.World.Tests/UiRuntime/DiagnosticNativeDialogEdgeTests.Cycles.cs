using Avalonia.Controls;
using Avalonia.Threading;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

public sealed partial class DiagnosticNativeDialogEdgeTests
{
    [Fact]
    public void Ten_suspend_restore_cycles_reuse_one_tool_window()
    {
        _fixture.Run(() =>
        {
            var (window, host, target, _) = Open();
            host.TrackProbe(DiagnosticProbeResolver.Resolve(target));
            var field = typeof(DiagnosticOverlayHost).GetField("_toolWindow",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
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

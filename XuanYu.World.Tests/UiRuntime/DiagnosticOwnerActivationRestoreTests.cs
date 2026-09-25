using System.Reflection;
using Avalonia.Controls;
using Avalonia.Threading;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticOwnerActivationRestoreTests
{
    readonly UiHeadlessFixture _fixture;
    public DiagnosticOwnerActivationRestoreTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Owner_activation_restores_the_same_locked_tool_window()
    {
        _fixture.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            vm.RunCommand.Execute("诊断模式");
            var target = new Button { Name = "Target" };
            var host = new DiagnosticOverlayHost { DataContext = vm };
            var window = new Window { Width = 480, Height = 300,
                Content = new Grid { Children = { target, host } } };
            window.Show(); window.UpdateLayout(); Dispatcher.UIThread.RunJobs();
            host.TrackProbe(DiagnosticProbeResolver.Resolve(target));
            var field = typeof(DiagnosticOverlayHost).GetField("_toolWindow",
                BindingFlags.Instance | BindingFlags.NonPublic)!;
            var tool = field.GetValue(host)!;
            ((Window)tool).Hide();
            typeof(DiagnosticOverlayHost).GetMethod("OnWindowActivated",
                BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(host, [window, EventArgs.Empty]);
            Assert.Same(tool, field.GetValue(host));
            Assert.Equal(1, host.ActiveProbeCardCount);
            window.Close();
        });
    }
}

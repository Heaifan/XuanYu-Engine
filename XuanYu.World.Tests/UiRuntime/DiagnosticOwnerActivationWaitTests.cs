using System.Reflection;
using Avalonia.Controls;
using Avalonia.Threading;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticOwnerActivationWaitTests
{
    readonly UiHeadlessFixture _fixture;
    public DiagnosticOwnerActivationWaitTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Early_activation_keeps_waiting_for_owner_to_become_active()
    {
        _fixture.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            vm.RunCommand.Execute("诊断模式");
            var target = new Button { Name = "Target" };
            var host = new DiagnosticOverlayHost { DataContext = vm };
            var window = new Window { Width = 480, Height = 300, ShowActivated = false,
                Content = new Grid { Children = { target, host } } };
            window.Show(); window.UpdateLayout(); Dispatcher.UIThread.RunJobs();
            host.TrackProbe(DiagnosticProbeResolver.Resolve(target));
            var other = new Window { Width = 120, Height = 80 };
            other.Show();
            typeof(DiagnosticOverlayHost).GetMethod("OnWindowActivated",
                BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(host, [window, EventArgs.Empty]);
            var field = typeof(DiagnosticOverlayHost).GetField("_ownerActivationRestoreTimer",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(field);
            var timer = field!.GetValue(host);
            Assert.NotNull(timer);
            other.Close(); window.Close();
        });
    }
}

using Avalonia.Controls;
using Avalonia.Threading;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticNativeDialogLifecycleTests
{
    readonly UiHeadlessFixture _fixture;
    public DiagnosticNativeDialogLifecycleTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Locked_probe_hides_and_restores_with_frozen_identity()
    {
        _fixture.Run(() =>
        {
            var (window, host, target, _) = Open();
            host.TrackProbe(DiagnosticProbeResolver.Resolve(target));
            var snapshot = host.TrackedSnapshot;
            var bounds = host.LastKnownBounds;
            host.SuspendForNativeDialog();
            Assert.Equal(0, host.ActiveProbeCardCount);
            Assert.True(host.IsProbeLocked);
            Assert.Same(snapshot, host.TrackedSnapshot);
            Assert.Equal(bounds, host.LastKnownBounds);
            host.RestoreAfterNativeDialog();
            Assert.Equal(1, host.ActiveProbeCardCount);
            Assert.Equal("Target", host.TrackedSnapshot?.TargetDisplayName);
            window.Close();
        });
    }

    [Fact]
    public void Restore_after_target_detaches_uses_frozen_snapshot()
    {
        _fixture.Run(() =>
        {
            var (window, host, target, vm) = Open();
            host.TrackProbe(DiagnosticProbeResolver.Resolve(target));
            ((Panel)window.Content!).Children.Remove(target);
            host.SuspendForNativeDialog();
            host.RestoreAfterNativeDialog();
            Assert.Equal(1, host.ActiveProbeCardCount);
            Assert.Equal("Target", host.TrackedSnapshot?.TargetDisplayName);
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

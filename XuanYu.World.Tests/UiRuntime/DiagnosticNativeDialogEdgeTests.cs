using Avalonia.Controls;
using Avalonia.Threading;
using System.Reflection;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed partial class DiagnosticNativeDialogEdgeTests
{
    readonly UiHeadlessFixture _fixture;
    public DiagnosticNativeDialogEdgeTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Native_transition_boundary_is_owned_by_overlay_host()
    {
        Assert.Null(typeof(DiagnosticFloatingToolWindow).GetMethod("ReassertOwnedZOrder"));
        Assert.NotNull(typeof(DiagnosticOverlayHost).GetMethod("ReassertToolWindowZOrder",
            BindingFlags.Instance | BindingFlags.NonPublic));
    }

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
    public void Tool_window_close_during_suspend_recreates_from_frozen_snapshot()
    {
        _fixture.Run(() =>
        {
            var (window, host, target, _) = Open();
            host.TrackProbe(DiagnosticProbeResolver.Resolve(target));
            host.SuspendForNativeDialog();
            var field = typeof(DiagnosticOverlayHost).GetField("_toolWindow",
                BindingFlags.Instance | BindingFlags.NonPublic)!;
            ((Window)field.GetValue(host)!).Close();
            host.RestoreAfterNativeDialog();
            Assert.Equal(1, host.ActiveProbeCardCount);
            Assert.Equal("Target", host.TrackedSnapshot?.TargetDisplayName);
            window.Close();
        });
    }

    [Fact]
    public void Owner_activation_reasserts_locked_tool_window_visibility()
    {
        _fixture.Run(() =>
        {
            var (window, host, target, _) = Open();
            host.TrackProbe(DiagnosticProbeResolver.Resolve(target));
            host.SuspendForNativeDialog(); host.RestoreAfterNativeDialog();
            var field = typeof(DiagnosticOverlayHost).GetField("_toolWindow",
                BindingFlags.Instance | BindingFlags.NonPublic)!;
            ((Window)field.GetValue(host)!).Hide();
            typeof(DiagnosticOverlayHost).GetMethod("OnWindowActivated",
                BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(host, [window, EventArgs.Empty]);
            Assert.Equal(1, host.ActiveProbeCardCount);
            window.Close();
        });
    }

}

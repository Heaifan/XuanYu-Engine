using System.Reflection;
using Avalonia.Controls;
using XuanYu.Editor.UI;
namespace XuanYu.World.Tests.UiRuntime;
public sealed class DiagnosticNativeTargetOwnershipTests
{
    [Fact]
    public void Native_move_snapshot_records_entered_viewport_phase()
    {
        var host = new VulkanNativeHost();
        Send(host, NativePointerMessage.Move, 428, 251);

        var snapshot = ReadSnapshot(host);
        var text = snapshot.GetType().GetMethod("Format")!.Invoke(snapshot, null) as string;

        Assert.Contains("Phase=Entered", text);
        Assert.Contains("Target=XYE.VIEWPORT", text);
    }
    [Fact]
    public void Native_leave_snapshot_records_exited_viewport_phase()
    {
        var host = new VulkanNativeHost();
        Send(host, NativePointerMessage.MouseLeave, 428, 251);

        var snapshot = ReadSnapshot(host);
        var text = snapshot.GetType().GetMethod("Format")!.Invoke(snapshot, null) as string;

        Assert.Contains("Phase=Exited", text);
        Assert.Contains("Target=XYE.VIEWPORT", text);
    }
    [Fact]
    public void Avalonia_probe_cannot_replace_active_native_viewport_target()
    {
        var overlay = new DiagnosticOverlayHost();
        var vm = new UiVm(null, seedInitialScene: false);
        vm.RunCommand.Execute("诊断模式"); overlay.DataContext = vm; AttachVm(overlay);
        var native = new VulkanNativeHost();
        SendDiagnostic(overlay, native, "Entered", 40, 30);

        overlay.ProbeHover(new Button(), false);

        Assert.Equal("XYE.VIEWPORT", overlay.CurrentProbeResult?.DebugId);
    }

    [Fact]
    public void Native_exit_clears_override_and_allows_avalonia_probe()
    {
        var overlay = new DiagnosticOverlayHost();
        var vm = new UiVm(null, seedInitialScene: false);
        vm.RunCommand.Execute("诊断模式"); overlay.DataContext = vm; AttachVm(overlay);
        var native = new VulkanNativeHost(); var button = new Button();
        SendDiagnostic(overlay, native, "Entered", 40, 30);
        SendDiagnostic(overlay, native, "Exited", 40, 30);

        overlay.ProbeHover(button, false);

        Assert.NotEqual("XYE.VIEWPORT", overlay.CurrentProbeResult?.DebugId);
    }

    [Fact]
    public void Turning_diagnostic_mode_off_clears_native_probe_state()
    {
        var overlay = new DiagnosticOverlayHost();
        var vm = new UiVm(null, seedInitialScene: false);
        vm.RunCommand.Execute("诊断模式"); overlay.DataContext = vm; AttachVm(overlay);
        SendDiagnostic(overlay, new VulkanNativeHost(), "Entered", 40, 30);

        vm.RunCommand.Execute("诊断模式");

        Assert.Null(overlay.CurrentProbeResult);
        Assert.Equal(0, overlay.ActiveProbeCardCount);
        Assert.Equal(0, overlay.ActiveProbeHighlightCount);
    }

    static void Send(VulkanNativeHost host, uint message, int x, int y) =>
        typeof(VulkanNativeHost).GetMethod("OnNativePointerMessage",
            BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(host,
            [new NativePointerMessage(message, 0, x, y, (nint)123, 0, 0, 0)]);

    static object ReadSnapshot(VulkanNativeHost host) =>
        typeof(VulkanNativeHost).GetField("_lastNativePointerProbe",
            BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(host)!;

    static void SendDiagnostic(DiagnosticOverlayHost overlay, VulkanNativeHost native,
        string phase, double x, double y)
    {
        var eventType = typeof(VulkanNativeHost).Assembly.GetType(
            "XuanYu.Editor.UI.DiagnosticNativeViewportEvent")!;
        var phaseType = typeof(VulkanNativeHost).Assembly.GetType(
            "XuanYu.Editor.UI.DiagnosticNativeViewportPhase")!;
        var value = Enum.Parse(phaseType, phase);
        var change = Activator.CreateInstance(eventType, native, value, x, y)!;
        typeof(DiagnosticOverlayHost).GetMethod("OnNativeViewportDiagnosticChanged",
            BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(overlay, [native, change]);
    }

    static void AttachVm(DiagnosticOverlayHost overlay) =>
        typeof(DiagnosticOverlayHost).GetMethod("AttachVm",
            BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(overlay, [null]);
}

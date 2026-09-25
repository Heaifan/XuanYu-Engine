using System.Reflection;
using Avalonia.Controls;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class DiagnosticLockedControlNativeMoveTests
{
    [Fact]
    public void Native_move_does_not_override_a_locked_control_target()
    {
        var overlay = new DiagnosticOverlayHost();
        var vm = new UiVm(null, seedInitialScene: false);
        vm.RunCommand.Execute("诊断模式"); overlay.DataContext = vm; AttachVm(overlay);
        overlay.TrackProbe(DiagnosticProbeResolver.Resolve(new Button { Name = "Open" }));
        SendDiagnostic(overlay, new VulkanNativeHost(), "Moved", 40, 30);

        var field = typeof(DiagnosticOverlayHost).GetField("_nativeViewportHost",
            BindingFlags.Instance | BindingFlags.NonPublic)!;
        Assert.Null(field.GetValue(overlay));
    }

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

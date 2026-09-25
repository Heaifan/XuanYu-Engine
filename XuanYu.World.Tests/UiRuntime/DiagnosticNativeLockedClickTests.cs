using System.Reflection;
using Avalonia.Controls;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class DiagnosticNativeLockedClickTests
{
    [Fact]
    public void Native_blank_click_does_not_replace_locked_target()
    {
        var overlay = new DiagnosticOverlayHost();
        var vm = new UiVm(null, seedInitialScene: false);
        vm.RunCommand.Execute("诊断模式"); overlay.DataContext = vm; AttachVm(overlay);
        var first = new Button { Name = "A" };
        overlay.TrackProbe(DiagnosticProbeResolver.Resolve(first));

        SendDiagnostic(overlay, new VulkanNativeHost(), "Clicked", 40, 30);

        Assert.Same(first, overlay.LockedProbeResult?.SemanticTarget);
        Assert.True(overlay.IsProbeLocked);
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

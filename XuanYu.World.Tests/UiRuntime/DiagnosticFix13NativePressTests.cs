using System.Reflection;
using Avalonia.Controls;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class DiagnosticFix13NativePressTests
{
    [Fact]
    public void Native_left_down_publishes_viewport_pressed_phase()
    {
        var host = new VulkanNativeHost();
        var method = typeof(VulkanNativeHost).GetMethod("OnNativePointerMessage",
            BindingFlags.Instance | BindingFlags.NonPublic)!;
        method.Invoke(host, [new NativePointerMessage(NativePointerMessage.LeftDown,
            0, 428, 251, (nint)123, 0, 0, 0)]);
        var probe = typeof(VulkanNativeHost).GetField("_lastNativePointerProbe",
            BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(host);
        Assert.Equal("Clicked", probe?.GetType().GetProperty("Phase")?.GetValue(probe)?.ToString());
    }

    [Fact]
    public void Native_pressed_locks_viewport_probe()
    {
        var overlay = new DiagnosticOverlayHost(new FakeClipboard());
        var vm = new UiVm(null, seedInitialScene: false); vm.RunCommand.Execute("诊断模式");
        overlay.DataContext = vm; typeof(DiagnosticOverlayHost).GetMethod("AttachVm",
            BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(overlay, [null]);
        var native = new VulkanNativeHost(); var eventType = typeof(VulkanNativeHost).Assembly.GetType(
            "XuanYu.Editor.UI.DiagnosticNativeViewportEvent")!;
        var phaseType = typeof(VulkanNativeHost).Assembly.GetType(
            "XuanYu.Editor.UI.DiagnosticNativeViewportPhase")!;
        var change = Activator.CreateInstance(eventType, native, Enum.Parse(phaseType, "Clicked"), 40d, 30d)!;
        typeof(DiagnosticOverlayHost).GetMethod("OnNativeViewportDiagnosticChanged",
            BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(overlay, [native, change]);
        Assert.Same(native, overlay.LockedProbeResult?.DeepVisual);
    }

    sealed class FakeClipboard : IDiagnosticClipboard
    { public Task SetTextAsync(Control _, string text) => Task.CompletedTask; }
}

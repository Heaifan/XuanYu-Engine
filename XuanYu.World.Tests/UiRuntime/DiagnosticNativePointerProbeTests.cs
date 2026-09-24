using System.Reflection;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class DiagnosticNativePointerProbeTests
{
    [Fact]
    public void Native_pointer_move_is_observed_as_viewport_without_consuming_route()
    {
        var host = new VulkanNativeHost();
        var message = new NativePointerMessage(NativePointerMessage.Move, 0,
            428, 251, (nint)123, 0, 0, 0);
        typeof(VulkanNativeHost).GetMethod("OnNativePointerMessage",
            BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(host, [message]);
        var field = typeof(VulkanNativeHost).GetField("_lastNativePointerProbe",
            BindingFlags.Instance | BindingFlags.NonPublic);
        var snapshot = field!.GetValue(host)!;
        var text = snapshot.GetType().GetMethod("Format")!.Invoke(snapshot, null) as string;
        Assert.Contains("Observed=True", text);
        Assert.Contains("Target=XYE.VIEWPORT", text);
        Assert.Contains("X=428", text);
        Assert.Contains("Y=251", text);
    }

    [Fact]
    public void Native_pointer_move_is_forwarded_to_diagnostic_overlay_bridge()
    {
        var host = new VulkanNativeHost();
        var observed = false;
        Action<VulkanNativeHost, double, double> handler = (_, x, y) =>
            observed = x == 428 && y == 251;
        var eventInfo = typeof(VulkanNativeHost).GetEvent("NativePointerMoved",
            BindingFlags.Instance | BindingFlags.NonPublic)!;
        eventInfo.AddEventHandler(host, handler);
        var message = new NativePointerMessage(NativePointerMessage.Move, 0,
            428, 251, (nint)123, 0, 0, 0);
        typeof(VulkanNativeHost).GetMethod("OnNativePointerMessage",
            BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(host, [message]);
        Assert.True(observed);
    }
}

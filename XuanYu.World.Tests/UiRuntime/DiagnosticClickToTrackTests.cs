using Avalonia.Controls;
using XuanYu.Editor.Input;
using XuanYu.Editor.UI;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class DiagnosticClickToTrackTests
{
    [Fact]
    public void Diagnostic_visual_is_not_trackable()
    {
        var overlay = EnabledOverlay(); var target = new Button { Name = "Target" };
        overlay.TrackProbe(DiagnosticProbeResolver.Resolve(target));
        overlay.ProbeClick(new DiagnosticFloatingCard(DiagnosticElementSnapshot.Capture(
            overlay.LockedProbeResult!), _ => Task.CompletedTask, false));
        Assert.Same(target, overlay.LockedProbeResult?.SemanticTarget);
    }
    [Fact]
    public void Explicit_click_locks_target_and_hover_cannot_replace_it()
    {
        var overlay = EnabledOverlay(); var first = new Button { Name = "A" }; var second = new Button { Name = "B" };
        overlay.ProbeHover(first, false); overlay.TrackProbe(DiagnosticProbeResolver.Resolve(first));
        overlay.ProbeHover(second, false);
        Assert.Same(first, overlay.LockedProbeResult?.SemanticTarget);
        Assert.Same(first, overlay.CurrentProbeResult?.SemanticTarget);
    }
    [Fact]
    public void Explicit_click_switches_an_existing_locked_target()
    {
        var overlay = EnabledOverlay(); var first = new Button { Name = "A" }; var second = new Button { Name = "B" };
        overlay.TrackProbe(DiagnosticProbeResolver.Resolve(first));
        overlay.TrackProbe(DiagnosticProbeResolver.Resolve(second));
        Assert.Same(second, overlay.LockedProbeResult?.SemanticTarget);
        Assert.True(overlay.IsProbeLocked);
    }

    [Fact]
    public void Real_probe_click_switches_an_existing_locked_target()
    {
        var overlay = EnabledOverlay(); var first = new Button { Name = "A" }; var second = new Button { Name = "B" };
        overlay.ProbeClick(first);
        overlay.ProbeClick(second);
        Assert.Same(second, overlay.LockedProbeResult?.SemanticTarget);
    }

    [Fact]
    public void Locked_target_ignores_blank_viewport_click()
    {
        var overlay = EnabledOverlay(); var first = new Button { Name = "A" };
        overlay.TrackProbe(DiagnosticProbeResolver.Resolve(first));
        overlay.ProbeClick(new Avalonia.Controls.Panel());
        Assert.Same(first, overlay.LockedProbeResult?.SemanticTarget);
        Assert.True(overlay.IsProbeLocked);
    }

    [Fact]
    public void Native_left_down_is_a_clicked_diagnostic_phase()
    {
        var host = new VulkanNativeHost();
        typeof(VulkanNativeHost).GetMethod("OnNativePointerMessage",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.Invoke(host,
            [new NativePointerMessage(NativePointerMessage.LeftDown, 1, 20, 30, (nint)1, 0, 0, 0)]);
        var probe = typeof(VulkanNativeHost).GetField("_lastNativePointerProbe",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(host)!;
        Assert.Equal("Clicked", probe.GetType().GetProperty("Phase")!.GetValue(probe)!.ToString());
    }

    [Fact]
    public void Native_click_still_reaches_the_production_viewport_sink()
    {
        var host = new VulkanNativeHost(); var vm = new UiVm(null, () => true, seedInitialScene: false);
        host.DataContext = vm;
        typeof(VulkanNativeHost).GetMethod("OnNativePointerMessage",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.Invoke(host,
            [new NativePointerMessage(NativePointerMessage.LeftDown, 1, 20, 30, (nint)1, 0, 0, 0)]);
        Assert.Equal(GestureOwner.Picking, vm.ViewportInput.Router.State.Owner);
    }

    [Fact]
    public void Native_left_down_dismisses_the_xyui_map_context_menu()
    {
        var host = new VulkanNativeHost(); var menu = new XYContextMenu();
        typeof(VulkanNativeHost).GetField("_mapContextMenu",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.SetValue(host, menu);
        menu.Open(host);
        typeof(VulkanNativeHost).GetMethod("OnNativePointerMessage",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.Invoke(host,
            [new NativePointerMessage(NativePointerMessage.LeftDown, 1, 20, 30, (nint)1, 0, 0, 0)]);
        Assert.False(menu.IsOpen);
    }
    static DiagnosticOverlayHost EnabledOverlay()
    {
        var overlay = new DiagnosticOverlayHost(); var vm = new UiVm(null, seedInitialScene: false);
        vm.RunCommand.Execute("诊断模式"); overlay.DataContext = vm;
        typeof(DiagnosticOverlayHost).GetMethod("AttachVm", System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.NonPublic)!.Invoke(overlay, [null]);
        return overlay;
    }
}

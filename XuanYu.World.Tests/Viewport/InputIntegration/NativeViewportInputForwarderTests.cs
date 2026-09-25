using XuanYu.Editor.Input;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Viewport.InputIntegration;

public sealed class NativeViewportInputForwarderTests
{
    [Fact]
    public void Native_pointer_down_reaches_production_sink_once()
    {
        var sink = new RecordingSink();
        var forwarder = new NativeViewportInputForwarder(sink, new("native-hwnd"));

        forwarder.Forward(Message(NativePointerMessage.LeftDown));

        var result = Assert.Single(sink.Events);
        Assert.Equal(EditorPointerEventKind.Pressed, result.Kind);
        Assert.Equal("native-hwnd", result.SourceSurface.Id);
    }

    [Fact]
    public void Native_wheel_and_focus_map_through_the_same_sink()
    {
        var sink = new RecordingSink();
        var forwarder = new NativeViewportInputForwarder(sink, new("native-hwnd"));

        forwarder.Forward(Message(NativePointerMessage.Wheel, 120));
        forwarder.Forward(Message(NativePointerMessage.KillFocus));

        Assert.Equal(2, sink.Events.Count);
        Assert.Equal(EditorPointerEventKind.Wheel, sink.Events[0].Kind);
        Assert.Equal(1, sink.Events[0].WheelDelta);
        Assert.Equal(EditorPointerEventKind.FocusLost, sink.Events[1].Kind);
    }

    [Fact]
    public void Native_host_source_has_no_legacy_route_policy_branch()
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var source = File.ReadAllText(Path.Combine(root, "XuanYu.Editor.UI", "Viewport", "Vulkan", "VulkanNativeHost.Pointer.cs"));
        Assert.Contains("NativeViewportInputForwarder.Forward", source);
        Assert.Contains("NotifyOwnerPointerDown", source);
        Assert.Contains("Win32ViewportHost.ReleaseMouseCapture", source);
        Assert.DoesNotContain("NativePointerRoutePolicy.Resolve", source);
        Assert.DoesNotContain("ReportPointerPicking", source);
    }

    [Fact]
    public void Native_pointer_enters_the_real_production_sink_once()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        var forwarder = new NativeViewportInputForwarder(vm.ViewportInput.Sink, new("native-hwnd"));

        forwarder.Forward(Message(NativePointerMessage.LeftDown));

        Assert.Equal(GestureOwner.Picking, vm.ViewportInput.Router.State.Owner);
    }

    [Fact]
    public void Native_mouse_leave_is_not_invented_as_a_pointer_semantic_event()
    {
        var sink = new RecordingSink();
        var forwarder = new NativeViewportInputForwarder(sink, new("native-hwnd"));

        forwarder.Forward(Message(NativePointerMessage.MouseLeave));

        Assert.Empty(sink.Events);
    }

    [Fact]
    public void Native_viewport_dispose_cancels_the_real_production_lifecycle()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        var source = new ViewportPointerSource("native-hwnd");
        new NativeViewportInputForwarder(vm.ViewportInput.Sink, source).Forward(Message(NativePointerMessage.LeftDown));

        NativeViewportInputForwarder.ForwardLifecycle(vm.ViewportInput.Sink,
            EditorPointerEventKind.ViewportDisposed, source);

        Assert.Equal(ViewportGestureState.Idle, vm.ViewportInput.Router.State);
    }

    static NativePointerMessage Message(uint kind, int wheel = 0) =>
        new(kind, 1 | (wheel << 16), 150, 300, 0, 0, 0, 0, false, 7);

    sealed class RecordingSink : IViewportInputSink
    {
        public List<EditorPointerEvent> Events { get; } = [];
        public void Handle(EditorPointerEvent pointer) => Events.Add(pointer);
    }
}

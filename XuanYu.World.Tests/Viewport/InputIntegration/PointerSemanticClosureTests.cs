using XuanYu.Editor.Input;
using XuanYu.Editor.Input.Lifecycle;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Viewport.InputIntegration;

public sealed class PointerSemanticClosureTests
{
    [Fact]
    public void Native_host_owns_pointer_when_native_handle_exists()
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var pointer = File.ReadAllText(Path.Combine(root, "XuanYu.Editor.UI", "Viewport", "Vulkan", "VulkanNativeHost.AvaloniaPointer.cs"));
        var keyboard = File.ReadAllText(Path.Combine(root, "XuanYu.Editor.UI", "Viewport", "Vulkan", "VulkanNativeHost.AvaloniaKeyboard.cs"));

        Assert.Contains("if (_hwnd != 0) return;", pointer);
        Assert.Contains("if (_hwnd != 0) return;", keyboard);
    }

    [Theory]
    [InlineData(EditorPointerEventKind.Released)]
    [InlineData(EditorPointerEventKind.CaptureLost)]
    [InlineData(EditorPointerEventKind.FocusLost)]
    [InlineData(EditorPointerEventKind.WindowDeactivated)]
    [InlineData(EditorPointerEventKind.ViewportDisposed)]
    public void Every_terminal_path_returns_router_to_idle(EditorPointerEventKind terminal)
    {
        var capture = new CaptureProbe();
        var consumer = new Probe();
        var router = new ViewportInputRouter([consumer], capture);

        router.Dispatch(Event(EditorPointerEventKind.Pressed, 1));
        router.Dispatch(Event(terminal, 1));

        Assert.Equal(ViewportGestureState.Idle, router.State);
        Assert.Equal(1, capture.ReleaseCount);
    }

    [Fact]
    public void Native_and_avalonia_wheel_adapters_expose_equal_canonical_delta()
    {
        var native = NativePointerEventAdapter.Convert(
            new NativePointerMessage(NativePointerMessage.Wheel, 120 << 16, 10, 20, 0, 0, 0, 0), new("native"), 1);
        var avalonia = AvaloniaPointerEventAdapter.Convert(new AvaloniaPointerSample(
            EditorPointerEventKind.Wheel, new(10, 20), EditorPointerButtons.None,
            EditorPointerModifiers.None, 1, 1), new("avalonia"), 1);

        Assert.Equal(native.WheelDelta, avalonia.WheelDelta);
        Assert.Equal(native.Position, avalonia.Position);
    }

    [Fact]
    public void Canonical_pointer_order_is_preserved_through_router()
    {
        var consumer = new Probe();
        var router = new ViewportInputRouter([consumer], new CaptureProbe());
        foreach (var kind in new[] { EditorPointerEventKind.Move, EditorPointerEventKind.Pressed,
            EditorPointerEventKind.Move, EditorPointerEventKind.Move, EditorPointerEventKind.Released })
            router.Dispatch(Event(kind, 1));

        Assert.Equal(new[] { EditorPointerEventKind.Move, EditorPointerEventKind.Pressed,
            EditorPointerEventKind.Move, EditorPointerEventKind.Move, EditorPointerEventKind.Released }, consumer.Events);
    }

    static EditorPointerEvent Event(EditorPointerEventKind kind, long id) => new(kind, new(1, 2),
        EditorPointerButtons.Left, EditorPointerModifiers.None, 0, id, new("test"), 1);

    sealed class Probe : IViewportInputConsumer
    {
        public GestureOwner Owner => GestureOwner.Camera;
        public List<EditorPointerEventKind> Events { get; } = [];
        public bool CanBegin(EditorPointerEvent p, ViewportGestureState s) => p.Kind == EditorPointerEventKind.Pressed;
        public ViewportInputDispatchResult Handle(EditorPointerEvent p, ViewportGestureState s) { Events.Add(p.Kind); return new(ViewportInputDispatchKind.Captured); }
        public void Begin(ViewportGestureContext c) { }
        public void Update(ViewportGestureContext c) { Events.Add(c.Input.Kind); }
        public void Commit(ViewportGestureContext c) { }
        public void Cancel(ViewportCancellationContext c) { }
    }

    sealed class CaptureProbe : IViewportPointerCaptureCoordinator
    {
        public int ReleaseCount { get; private set; }
        public void Capture(long id, GestureOwner owner) { }
        public void Release(long id, GestureOwner owner) => ReleaseCount++;
    }
}

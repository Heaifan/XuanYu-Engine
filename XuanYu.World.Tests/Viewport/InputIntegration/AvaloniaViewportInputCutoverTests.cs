using XuanYu.Editor.Input;
using XuanYu.Editor.Input.Lifecycle;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Viewport.InputIntegration;

public sealed class AvaloniaViewportInputCutoverTests
{
    [Fact]
    public void Avalonia_pointer_down_reaches_one_production_consumer()
    {
        var consumer = new Probe();
        var composition = new ViewportInputComposition([consumer], new CaptureProbe());

        AvaloniaViewportInputForwarder.Forward(composition.Sink,
            new( EditorPointerEventKind.Pressed, new(10, 20), EditorPointerButtons.Left,
                EditorPointerModifiers.None, 0, 7), new("avalonia"), 1);

        Assert.Equal(1, consumer.PointerCount);
    }

    [Fact]
    public void Avalonia_keyboard_down_reaches_the_same_production_consumer()
    {
        var consumer = new Probe();
        var composition = new ViewportInputComposition([consumer], new CaptureProbe());

        AvaloniaViewportKeyboardInputForwarder.Forward(composition.Sink,
            new AvaloniaKeySample(0x41, AvaloniaKeyAction.Down, AvaloniaKeyModifiers.Control, false), new("avalonia"));

        Assert.Equal(1, consumer.KeyCount);
        Assert.Equal(EditorPointerModifiers.Control, consumer.LastKey.Modifiers);
    }

    [Fact]
    public void Avalonia_host_sources_only_forward_to_the_production_adapters()
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var pointer = File.ReadAllText(Path.Combine(root, "XuanYu.Editor.UI", "Viewport", "Vulkan", "VulkanNativeHost.AvaloniaPointer.cs"));
        var key = File.ReadAllText(Path.Combine(root, "XuanYu.Editor.UI", "Viewport", "Vulkan", "VulkanNativeHost.AvaloniaKeyboard.cs"));

        Assert.Contains("AvaloniaViewportInputForwarder", pointer);
        Assert.Contains("AvaloniaViewportKeyboardInputForwarder", key);
        Assert.DoesNotContain("PreviewViewportPointer", pointer);
        Assert.DoesNotContain("ReportPointerPicking", pointer);
        Assert.DoesNotContain("TryNavGizmo", pointer);
        Assert.DoesNotContain("RegionDrawingPointer", pointer);
        Assert.DoesNotContain("CancelInteraction", pointer);
        Assert.DoesNotContain("RegionDrawingPointerMoved", key);
    }

    [Fact]
    public void Avalonia_focus_lost_enters_the_production_lifecycle()
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var lifecycle = File.ReadAllText(Path.Combine(root, "XuanYu.Editor.UI", "Viewport", "Vulkan", "VulkanNativeHost.AvaloniaLifecycle.cs"));

        Assert.Contains("ForwardLifecycle", lifecycle);
        Assert.Contains("EditorPointerEventKind.FocusLost", lifecycle);
        Assert.DoesNotContain("CancelInteraction", lifecycle);
    }

    sealed class Probe : IViewportInputConsumer
    {
        public GestureOwner Owner => GestureOwner.Camera;
        public int PointerCount { get; private set; }
        public int KeyCount { get; private set; }
        public EditorKeyEvent LastKey { get; private set; }
        public bool CanBegin(EditorPointerEvent p, ViewportGestureState s) => p.Kind == EditorPointerEventKind.Pressed;
        public ViewportInputDispatchResult Handle(EditorPointerEvent p, ViewportGestureState s) { PointerCount++; return new(ViewportInputDispatchKind.Captured); }
        public ViewportInputDispatchResult Handle(EditorKeyEvent k, ViewportGestureState s) { KeyCount++; LastKey = k; return ViewportInputDispatchResult.Observed; }
        public void Begin(ViewportGestureContext c) { }
        public void Update(ViewportGestureContext c) { }
        public void Commit(ViewportGestureContext c) { }
        public void Cancel(ViewportCancellationContext c) { }
    }

    sealed class CaptureProbe : IViewportPointerCaptureCoordinator
    {
        public void Capture(long id, GestureOwner owner) { }
        public void Release(long id, GestureOwner owner) { }
    }
}

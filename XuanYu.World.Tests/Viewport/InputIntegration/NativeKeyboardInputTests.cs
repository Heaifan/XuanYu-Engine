using XuanYu.Editor.Input;
using XuanYu.Editor.Input.Lifecycle;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Viewport.InputIntegration;

public sealed class NativeKeyboardInputTests
{
    [Fact]
    public void Native_keydown_reaches_one_sink_and_one_router()
    {
        var consumer = new KeyboardProbeConsumer();
        var composition = new ViewportInputComposition([consumer], new CaptureProbe());

        NativeViewportKeyboardInputForwarder.Forward(
            composition.Sink, Key(NativeKeyMessage.KeyDown), new("native-hwnd"));

        Assert.Single(consumer.Events);
        Assert.Equal(EditorKeyAction.Down, consumer.Events[0].Action);
        Assert.Equal(1, composition.Router.KeyboardDispatchCount);
    }

    [Fact]
    public void Native_keyup_reaches_one_sink_and_one_router()
    {
        var consumer = new KeyboardProbeConsumer();
        var composition = new ViewportInputComposition([consumer], new CaptureProbe());

        NativeViewportKeyboardInputForwarder.Forward(
            composition.Sink, Key(NativeKeyMessage.KeyUp), new("native-hwnd"));

        Assert.Equal(EditorKeyAction.Up, consumer.Events[0].Action);
        Assert.Equal(1, composition.Router.KeyboardDispatchCount);
    }

    [Fact]
    public void Native_keyboard_adapter_preserves_modifiers_and_repeat()
    {
        var key = new NativeKeyMessage(NativeKeyMessage.KeyDown, 0x41, 0, true,
            true, true, true, false);
        var result = NativeKeyboardEventAdapter.Convert(key, new("native"));

        Assert.Equal(new EditorKey(0x41), result.Key);
        Assert.Equal(EditorPointerModifiers.Shift | EditorPointerModifiers.Control |
            EditorPointerModifiers.Alt, result.Modifiers);
        Assert.True(result.IsRepeat);
    }

    [Fact]
    public void Focus_lost_clears_router_keyboard_state()
    {
        var composition = new ViewportInputComposition([new KeyboardProbeConsumer()], new CaptureProbe());
        composition.Dispatch(NativeKeyboardEventAdapter.Convert(Key(NativeKeyMessage.KeyDown), new("native")));
        composition.Dispatch(new EditorPointerEvent(EditorPointerEventKind.FocusLost,
            new(0, 0), EditorPointerButtons.None, EditorPointerModifiers.None, 0, 1,
            new("native"), 1));

        Assert.Empty(composition.Router.PressedKeys);
    }

    [Fact]
    public void Native_host_registers_keyboard_source_without_consumer_bypass()
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var source = File.ReadAllText(Path.Combine(root, "XuanYu.Editor.UI", "Viewport", "Vulkan", "VulkanNativeHost.Keyboard.cs"));
        var host = File.ReadAllText(Path.Combine(root, "XuanYu.Editor.UI", "Viewport", "Vulkan", "Win32ViewportHost.Keyboard.cs"));

        Assert.Contains("NativeViewportKeyboardInputForwarder.Forward", source);
        Assert.Contains("SetKeyboardInputSink", host);
        Assert.DoesNotContain("PreviewViewportPointer", source);
        Assert.DoesNotContain("RegionDrawing", source);
    }

    static NativeKeyMessage Key(uint action) => new(action, 0x41, 0, false, false, false, false, false);

    sealed class KeyboardProbeConsumer : IViewportInputConsumer
    {
        public GestureOwner Owner => GestureOwner.Camera;
        public List<EditorKeyEvent> Events { get; } = [];
        public ViewportInputDispatchResult Handle(EditorKeyEvent key, ViewportGestureState state)
        {
            Events.Add(key);
            return ViewportInputDispatchResult.Observed;
        }
        public bool CanBegin(EditorPointerEvent p, ViewportGestureState s) => false;
        public ViewportInputDispatchResult Handle(EditorPointerEvent p, ViewportGestureState s) =>
            ViewportInputDispatchResult.Ignored;
        public void Begin(ViewportGestureContext c) { }
        public void Update(ViewportGestureContext c) { }
        public void Commit(ViewportGestureContext c) { }
        public void Cancel(ViewportCancellationContext c) { }
    }

    sealed class CaptureProbe : IViewportPointerCaptureCoordinator
    {
        public void Capture(long pointerId, GestureOwner owner) { }
        public void Release(long pointerId, GestureOwner owner) { }
    }
}

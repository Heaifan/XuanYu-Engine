using XuanYu.Editor.Input;

namespace XuanYu.World.Tests.Viewport;

public sealed class UnifiedPointerModelTests
{
    [Fact]
    public void Event_carries_editor_level_state_without_platform_handles()
    {
        var pointer = new EditorPointerEvent(
            EditorPointerEventKind.Move,
            new EditorPointerPosition(12.5, 24.25),
            EditorPointerButtons.Left | EditorPointerButtons.Right,
            EditorPointerModifiers.Shift | EditorPointerModifiers.Alt,
            0, 7, new ViewportPointerSource("main"), 1.5);

        Assert.Equal(EditorPointerEventKind.Move, pointer.Kind);
        Assert.Equal(new EditorPointerPosition(12.5, 24.25), pointer.Position);
        Assert.Equal(EditorPointerButtons.Left | EditorPointerButtons.Right, pointer.Buttons);
        Assert.Equal(EditorPointerModifiers.Shift | EditorPointerModifiers.Alt, pointer.Modifiers);
        Assert.Equal(7, pointer.PointerId);
        Assert.Equal("main", pointer.SourceSurface.Id);
        Assert.Equal(1.5, pointer.DpiScale);
    }

    [Fact]
    public void Sink_receives_one_platform_neutral_event()
    {
        var sink = new RecordingPointerSink();
        var pointer = new EditorPointerEvent(EditorPointerEventKind.Cancel,
            new EditorPointerPosition(1, 2), EditorPointerButtons.None,
            EditorPointerModifiers.None, 0, 1, new ViewportPointerSource("main"), 1);

        sink.Handle(pointer);

        Assert.Equal(pointer, sink.Last);
    }

    sealed class RecordingPointerSink : IViewportInputSink
    {
        public EditorPointerEvent Last { get; private set; }
        public void Handle(EditorPointerEvent pointer) => Last = pointer;
    }
}

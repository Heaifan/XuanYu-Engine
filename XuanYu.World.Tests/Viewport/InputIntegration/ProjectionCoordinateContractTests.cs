using XuanYu.Editor.Input;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Viewport.InputIntegration;

public sealed class ProjectionCoordinateContractTests
{
    [Fact]
    public void Native_forwarder_converts_physical_coordinates_once()
    {
        var sink = new RecordingSink();
        var forwarder = new NativeViewportInputForwarder(sink, new("native-hwnd"));

        forwarder.Forward(new NativePointerMessage(NativePointerMessage.Move, 0, 150, 300, 0, 0, 0, 0), 1.5);

        Assert.Equal(new EditorPointerPosition(100, 200), Assert.Single(sink.Events).Position);
    }

    sealed class RecordingSink : IViewportInputSink
    {
        public List<EditorPointerEvent> Events { get; } = [];
        public void Handle(EditorPointerEvent pointer) => Events.Add(pointer);
    }
}

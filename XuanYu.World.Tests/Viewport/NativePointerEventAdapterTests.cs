using XuanYu.Editor.Input;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Viewport;

public sealed class NativePointerEventAdapterTests
{
    static readonly ViewportPointerSource Source = new("main");

    [Theory]
    [InlineData(NativePointerMessage.Move, EditorPointerEventKind.Move)]
    [InlineData(NativePointerMessage.LeftDown, EditorPointerEventKind.Pressed)]
    [InlineData(NativePointerMessage.LeftUp, EditorPointerEventKind.Released)]
    [InlineData(NativePointerMessage.RightDown, EditorPointerEventKind.Pressed)]
    [InlineData(NativePointerMessage.RightUp, EditorPointerEventKind.Released)]
    [InlineData(NativePointerMessage.Wheel, EditorPointerEventKind.Wheel)]
    [InlineData(NativePointerMessage.CaptureChanged, EditorPointerEventKind.CaptureLost)]
    [InlineData(NativePointerMessage.KillFocus, EditorPointerEventKind.FocusLost)]
    [InlineData(NativePointerMessage.CancelMode, EditorPointerEventKind.Cancel)]
    public void Native_messages_map_to_editor_kinds(uint message, EditorPointerEventKind kind)
    {
        var native = new NativePointerMessage(message, 0x0007 | (120 << 16), 150, 300,
            0, 0, 0, 0, true);

        var result = NativePointerEventAdapter.Convert(native, Source, 1.5);

        Assert.Equal(kind, result.Kind);
    }

    [Fact]
    public void Native_adapter_converts_physical_pixels_and_normalizes_wheel()
    {
        var native = new NativePointerMessage(NativePointerMessage.Wheel,
            0x0007 | (120 << 16), 150, 300, 0, 0, 0, 0, true);

        var result = NativePointerEventAdapter.Convert(native, Source, 1.5);

        Assert.Equal(new EditorPointerPosition(100, 200), result.Position);
        Assert.Equal(EditorPointerButtons.Left | EditorPointerButtons.Right, result.Buttons);
        Assert.Equal(EditorPointerModifiers.Shift | EditorPointerModifiers.Alt, result.Modifiers);
        Assert.Equal(1, result.WheelDelta);
        Assert.Equal(1.5, result.DpiScale);
    }
}

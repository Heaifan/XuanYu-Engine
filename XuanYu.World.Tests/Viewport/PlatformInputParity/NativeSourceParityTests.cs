using XuanYu.Editor.Input;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Viewport.PlatformInputParity;

public sealed class NativeSourceParityTests
{
    [Theory]
    [InlineData(NativePointerMessage.RightDown)]
    [InlineData(NativePointerMessage.RightUp)]
    public void Right_button_messages_are_forwardable(uint message)
    {
        Assert.True(NativePointerSourceBoundary.IsPointerMessage(message));
    }

    [Fact]
    public void Wheel_screen_point_is_normalized_to_client_point()
    {
        var point = NativePointerSourceBoundary.ToClientPoint(1200, 900, 1000, 700);

        Assert.Equal((200, 200), point);
    }

    [Fact]
    public void Native_pointer_id_is_preserved_by_adapter()
    {
        var message = new NativePointerMessage(
            NativePointerMessage.LeftDown, 1, 10, 20, 0, 0, 0, 0, false, 7);

        var result = NativePointerEventAdapter.Convert(
            message, new ViewportPointerSource("test"), 1);

        Assert.Equal(7, result.PointerId);
        Assert.Equal(EditorPointerEventKind.Pressed, result.Kind);
    }

    [Theory]
    [InlineData(120, 1)]
    [InlineData(-120, -1)]
    [InlineData(240, 2)]
    public void Native_wheel_delta_is_signed_and_normalized(int delta, double notch)
    {
        var message = new NativePointerMessage(
            NativePointerMessage.Wheel, delta << 16, 0, 0, 0, 0, 0, 0);

        Assert.Equal(notch, NativePointerEventAdapter.Convert(
            message, new ViewportPointerSource("test"), 1).WheelDelta);
    }

    [Fact]
    public void Native_modifiers_match_Avalonia_modifier_contract()
    {
        var native = new NativePointerMessage(
            NativePointerMessage.LeftDown, 0x000d, 10, 20, 0, 0, 0, 0, true, 1, true);
        var avalonia = new AvaloniaPointerSample(
            EditorPointerEventKind.Pressed, new(10, 20), EditorPointerButtons.Left,
            EditorPointerModifiers.Shift | EditorPointerModifiers.Control |
            EditorPointerModifiers.Alt | EditorPointerModifiers.Meta, 0, 1);

        var nativeEvent = NativePointerEventAdapter.Convert(
            native, new ViewportPointerSource("test"), 1);
        var avaloniaEvent = AvaloniaPointerEventAdapter.Convert(
            avalonia, new ViewportPointerSource("test"), 1);

        Assert.Equal(avaloniaEvent.Modifiers, nativeEvent.Modifiers);
        Assert.Equal(avaloniaEvent.Buttons, nativeEvent.Buttons);
    }
}

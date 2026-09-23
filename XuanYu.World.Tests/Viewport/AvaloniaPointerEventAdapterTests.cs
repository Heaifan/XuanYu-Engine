using XuanYu.Editor.Input;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Viewport;

public sealed class AvaloniaPointerEventAdapterTests
{
    [Fact]
    public void Avalonia_sample_matches_equivalent_native_semantics()
    {
        var sample = new AvaloniaPointerSample(EditorPointerEventKind.Pressed,
            new EditorPointerPosition(100, 200), EditorPointerButtons.Left,
            EditorPointerModifiers.Shift | EditorPointerModifiers.Alt, 0, 1);

        var result = AvaloniaPointerEventAdapter.Convert(sample, new("main"), 1.5);
        var native = NativePointerEventAdapter.Convert(
            new NativePointerMessage(NativePointerMessage.LeftDown, 0x0005, 150, 300,
                0, 0, 0, 0, true), new("main"), 1.5);

        Assert.Equal(native, result);
    }

    [Fact]
    public void Avalonia_wheel_delta_is_already_logical_notches()
    {
        var sample = new AvaloniaPointerSample(EditorPointerEventKind.Wheel,
            new EditorPointerPosition(8, 9), EditorPointerButtons.None,
            EditorPointerModifiers.Control, 1, 2);

        var result = AvaloniaPointerEventAdapter.Convert(sample, new("main"), 2);

        Assert.Equal(1, result.WheelDelta);
        Assert.Equal(EditorPointerModifiers.Control, result.Modifiers);
        Assert.Equal(2, result.PointerId);
    }
}

using XuanYu.Editor.Input;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Viewport;

public sealed class UnifiedPointerReadinessContractTests
{
    [Theory]
    [InlineData(1d)]
    [InlineData(1.25d)]
    [InlineData(1.5d)]
    [InlineData(2d)]
    public void Native_physical_position_becomes_logical_once(double dpi)
    {
        var native = new NativePointerMessage(NativePointerMessage.Move,
            0, (int)(100 * dpi), (int)(80 * dpi), 0, 0, 0, 0);

        var result = NativePointerEventAdapter.Convert(native, new("main"), dpi);

        Assert.Equal(new EditorPointerPosition(100, 80), result.Position);
        Assert.Equal(dpi, result.DpiScale);
    }

    [Fact]
    public void Equivalent_sources_produce_equal_editor_semantics()
    {
        var native = NativePointerEventAdapter.Convert(
            new NativePointerMessage(NativePointerMessage.RightDown, 0x000e, 125, 250,
                0, 0, 0, 0, true), new("main"), 1.25);
        var avalonia = AvaloniaPointerEventAdapter.Convert(
            new AvaloniaPointerSample(EditorPointerEventKind.Pressed,
                new EditorPointerPosition(100, 200), EditorPointerButtons.Right,
                EditorPointerModifiers.Shift | EditorPointerModifiers.Control |
                EditorPointerModifiers.Alt, 0, 1), new("main"), 1.25);

        Assert.Equal(avalonia, native);
    }

    [Fact]
    public void Unified_contract_represents_all_cancellation_origins()
    {
        var kinds = new[]
        {
            EditorPointerEventKind.Released,
            EditorPointerEventKind.CaptureLost,
            EditorPointerEventKind.FocusLost,
            EditorPointerEventKind.Cancel,
            EditorPointerEventKind.WindowDeactivated,
        };

        Assert.Equal(5, kinds.Distinct().Count());
    }
}

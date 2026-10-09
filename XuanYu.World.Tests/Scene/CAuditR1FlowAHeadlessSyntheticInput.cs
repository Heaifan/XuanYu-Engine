using XuanYu.Editor.Input;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Scene;

internal static class CAuditR1FlowAHeadlessSyntheticInput
{
    static readonly ViewportPointerSource Source = new("headless-synthetic");

    internal static void Click(UiVm vm, double x, double y)
    {
        Forward(vm, EditorPointerEventKind.Move, x, y, EditorPointerButtons.None);
        Forward(vm, EditorPointerEventKind.Pressed, x, y, EditorPointerButtons.Left);
        Forward(vm, EditorPointerEventKind.Released, x, y, EditorPointerButtons.None);
    }

    static void Forward(UiVm vm, EditorPointerEventKind kind, double x, double y,
        EditorPointerButtons buttons) =>
        AvaloniaViewportInputForwarder.Forward(vm.ViewportInput.Sink,
            new(kind, new(x, y), buttons, EditorPointerModifiers.None, 0, 7), Source, 1);
}

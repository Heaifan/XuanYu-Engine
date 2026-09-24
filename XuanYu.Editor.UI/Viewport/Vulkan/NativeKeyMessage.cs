namespace XuanYu.Editor.UI;

public readonly record struct NativeKeyMessage(
    uint Message,
    int VirtualKeyCode,
    uint ScanCode,
    bool IsRepeat,
    bool ShiftDown,
    bool ControlDown,
    bool AltDown,
    bool MetaDown)
{
    public const uint KeyDown = 0x0100;
    public const uint KeyUp = 0x0101;
    public const uint SystemKeyDown = 0x0104;
    public const uint SystemKeyUp = 0x0105;
}

namespace XuanYu.Editor.UI;

public readonly record struct NativePointerMessage(
    uint Message,
    int Buttons,
    int PhysicalX,
    int PhysicalY,
    nint Hwnd,
    nint CaptureBefore,
    nint CaptureAfter,
    nint CaptureTarget)
{
    public const uint Move = 0x0200;
    public const uint LeftDown = 0x0201;
    public const uint LeftUp = 0x0202;
    public const uint CaptureChanged = 0x0215;
    public const uint KillFocus = 0x0008;
    public const uint CancelMode = 0x001f;

    public bool IsLeftButtonDown => (Buttons & 0x0001) != 0;
}

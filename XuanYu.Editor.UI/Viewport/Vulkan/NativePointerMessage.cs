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
    public const uint MiddleDown = 0x0207;
    public const uint MiddleUp = 0x0208;
    public const uint Wheel = 0x020a;
    public const uint CaptureChanged = 0x0215;
    public const uint KillFocus = 0x0008;
    public const uint CancelMode = 0x001f;

    public bool IsLeftButtonDown => (Buttons & 0x0001) != 0;
    public bool IsMiddleButtonDown => (Buttons & 0x0010) != 0;
    public bool IsShiftDown => (Buttons & 0x0004) != 0;
    public int WheelDelta => Buttons >> 16;
}

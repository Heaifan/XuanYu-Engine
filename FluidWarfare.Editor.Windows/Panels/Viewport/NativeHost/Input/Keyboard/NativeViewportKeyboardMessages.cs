namespace FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Input.Keyboard;

/// <summary>Win32 键盘消息解析与识别。不含编辑器业务。</summary>
sealed class NativeViewportKeyboardMessages
{
    public const uint WmKeyDown = 0x0100;
    public const uint WmKeyUp = 0x0101;

    public const int VkHome = 0x24;
    public const int VkEscape = 0x1B;
    public const int VkShift = 0x10;
    public const int VkControl = 0x11;
    public const int VkMenu = 0x12;
    public const int VkDecimal = 0x6E;
    public const int VkNumpad5 = 0x65;

    public bool IsKeyboardMessage(uint msg) => msg is WmKeyDown or WmKeyUp;

    public NativeViewportKeyboardRequest? Parse(uint msg, nint wParam) => msg switch
    {
        WmKeyDown => new((int)wParam, NativeViewportKeyboardAction.Down),
        WmKeyUp => new((int)wParam, NativeViewportKeyboardAction.Up),
        _ => null
    };
}

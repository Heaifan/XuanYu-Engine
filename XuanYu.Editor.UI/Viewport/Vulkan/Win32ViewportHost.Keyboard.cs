using System.Collections.Concurrent;

namespace XuanYu.Editor.UI;

static partial class Win32ViewportHost
{
    const uint WM_KEYDOWN = 0x0100, WM_KEYUP = 0x0101;
    const uint WM_SYSKEYDOWN = 0x0104, WM_SYSKEYUP = 0x0105;
    const int VK_SHIFT = 0x10, VK_CONTROL = 0x11, VK_MENU_KEY = 0x12;
    static readonly ConcurrentDictionary<nint, Action<NativeKeyMessage>> KeyboardInputSinks = new();

    public static void SetInputSinks(nint hwnd, Action<NativePointerMessage> pointer, Action<NativeKeyMessage> keyboard)
    {
        SetInputSink(hwnd, pointer);
        SetKeyboardInputSink(hwnd, keyboard);
    }

    public static void ClearInputSinks(nint hwnd)
    {
        SetInputSink(hwnd, null);
        SetKeyboardInputSink(hwnd, null);
    }

    public static void SetKeyboardInputSink(nint hwnd, Action<NativeKeyMessage>? sink)
    {
        if (hwnd == 0) return;
        if (sink is null) KeyboardInputSinks.TryRemove(hwnd, out _);
        else KeyboardInputSinks[hwnd] = sink;
    }

    static void RouteKeyboardMessage(nint hwnd, uint message, nint wParam, nint lParam)
    {
        if (!KeyboardInputSinks.TryGetValue(hwnd, out var sink) || !IsKeyboardMessage(message)) return;
        var bits = (long)lParam;
        sink(new(message, (int)wParam, (uint)((bits >> 16) & 0xff), (bits & (1L << 30)) != 0,
            IsDown(VK_SHIFT), IsDown(VK_CONTROL), IsDown(VK_MENU_KEY), IsMetaDown()));
    }

    static bool IsKeyboardMessage(uint message) => message is WM_KEYDOWN or WM_KEYUP or WM_SYSKEYDOWN or WM_SYSKEYUP;
    static bool IsDown(int key) => (GetKeyState(key) & 0x8000) != 0;
}

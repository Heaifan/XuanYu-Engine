namespace FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Input.Keyboard;

/// <summary>键盘动作类型。</summary>
enum NativeViewportKeyboardAction { Down, Up }

/// <summary>从 Win32 wParam 解析的键盘输入数据。</summary>
sealed record NativeViewportKeyboardRequest(int VirtualKeyCode, NativeViewportKeyboardAction Action);

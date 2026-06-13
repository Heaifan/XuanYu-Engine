namespace FluidWarfare.Editor.Input.Bindings;

/// <summary>
/// Win32 虚拟键码 (VK_*) 到抽象手势代码的静态映射。
/// 未识别的 VK 码返回 null，对应事件不产生 Action match。
/// 纯字典查找，无平台依赖。
/// </summary>
public static class Win32KeyCodeMapper
{
    public static string? Map(int virtualKeyCode) =>
        CodeMap.TryGetValue(virtualKeyCode, out var code) ? code : null;

    private static readonly Dictionary<int, string> CodeMap = new()
    {
        // 字母 A-Z (VK 0x41-0x5A)
        { 0x41, "A" }, { 0x42, "B" }, { 0x43, "C" }, { 0x44, "D" },
        { 0x45, "E" }, { 0x46, "F" }, { 0x47, "G" }, { 0x48, "H" },
        { 0x49, "I" }, { 0x4A, "J" }, { 0x4B, "K" }, { 0x4C, "L" },
        { 0x4D, "M" }, { 0x4E, "N" }, { 0x4F, "O" }, { 0x50, "P" },
        { 0x51, "Q" }, { 0x52, "R" }, { 0x53, "S" }, { 0x54, "T" },
        { 0x55, "U" }, { 0x56, "V" }, { 0x57, "W" }, { 0x58, "X" },
        { 0x59, "Y" }, { 0x5A, "Z" },

        // 数字 0-9 (VK 0x30-0x39)
        { 0x30, "0" }, { 0x31, "1" }, { 0x32, "2" }, { 0x33, "3" },
        { 0x34, "4" }, { 0x35, "5" }, { 0x36, "6" }, { 0x37, "7" },
        { 0x38, "8" }, { 0x39, "9" },

        // 功能键 F1-F12 (VK 0x70-0x7B)
        { 0x70, "F1" }, { 0x71, "F2" }, { 0x72, "F3" }, { 0x73, "F4" },
        { 0x74, "F5" }, { 0x75, "F6" }, { 0x76, "F7" }, { 0x77, "F8" },
        { 0x78, "F9" }, { 0x79, "F10" }, { 0x7A, "F11" }, { 0x7B, "F12" },

        // Numpad (VK_NUMPAD0-9: 0x60-0x69)
        { 0x60, "Numpad0" }, { 0x61, "Numpad1" }, { 0x62, "Numpad2" },
        { 0x63, "Numpad3" }, { 0x64, "Numpad4" }, { 0x65, "Numpad5" },
        { 0x66, "Numpad6" }, { 0x67, "Numpad7" }, { 0x68, "Numpad8" },
        { 0x69, "Numpad9" },

        // Numpad 运算符
        { 0x6B, "Add" }, { 0x6D, "Subtract" }, { 0x6F, "Divide" },
        { 0x6A, "Multiply" }, { 0x6E, "Decimal" },

        // 导航键
        { 0x24, "Home" }, { 0x23, "End" }, { 0x21, "PageUp" },
        { 0x22, "PageDown" }, { 0x2D, "Insert" }, { 0x2E, "Delete" },
        { 0x25, "Left" }, { 0x27, "Right" }, { 0x26, "Up" }, { 0x28, "Down" },

        // 控制键
        { 0x1B, "Escape" }, { 0x0D, "Enter" }, { 0x09, "Tab" },
        { 0x20, "Space" }, { 0x08, "Back" },

        // OEM 符号
        { 0xBC, "Comma" },     // VK_OEM_COMMA
        { 0xBE, "Period" },    // VK_OEM_PERIOD
        { 0xBF, "Slash" },     // VK_OEM_2
        { 0xBD, "Minus" },     // VK_OEM_MINUS
        { 0xBB, "Equals" },    // VK_OEM_PLUS
        { 0xDB, "LeftBracket" },  // VK_OEM_4
        { 0xDD, "RightBracket" }, // VK_OEM_6
        { 0xDC, "Backslash" },    // VK_OEM_5
        { 0xBA, "Semicolon" },    // VK_OEM_1
        { 0xDE, "Quote" },        // VK_OEM_7
        { 0xC0, "Backtick" },     // VK_OEM_3
    };
}

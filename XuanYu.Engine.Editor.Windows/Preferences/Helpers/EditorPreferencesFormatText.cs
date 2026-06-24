using XuanYu.Engine.Editor.Input.Actions;
using XuanYu.Engine.Editor.Input.Bindings;

namespace FluidWarfare.Editor.Windows.Preferences;

static class EditorPreferencesFormatText
{
    public static string FormatGestureText(EditorInputGesture? g)
    {
        if (g is null) return "未绑定";
        var m = g.Modifiers;
        var mt = m switch
        {
            EditorInputModifiers.None => "",
            EditorInputModifiers.Shift => "Shift+",
            EditorInputModifiers.Control => "Ctrl+",
            EditorInputModifiers.Alt => "Alt+",
            EditorInputModifiers.Shift | EditorInputModifiers.Control => "Ctrl+Shift+",
            EditorInputModifiers.Shift | EditorInputModifiers.Alt => "Alt+Shift+",
            EditorInputModifiers.Control | EditorInputModifiers.Alt => "Alt+Ctrl+",
            EditorInputModifiers.Shift | EditorInputModifiers.Control | EditorInputModifiers.Alt => "Alt+Ctrl+Shift+",
            _ => ""
        };
        var kt = g.Device switch
        {
            EditorInputDevice.Keyboard => g.Code switch
            {
                "Escape" => "Esc", "Space" => "空格", "Enter" => "回车",
                "Back" => "退格", "Delete" => "Del", "Insert" => "Ins",
                "PageUp" => "PgUp", "PageDown" => "PgDn",
                "Left" => "←", "Right" => "→", "Up" => "↑", "Down" => "↓",
                "Decimal" => "小键盘.", "Comma" => ",", "Period" => ".",
                "Slash" => "/", "Semicolon" => ";", "Quote" => "'",
                "Minus" => "-", "Equals" => "=", "Backtick" => "`",
                "Backslash" => "\\", "LeftBracket" => "[", "RightBracket" => "]",
                _ => g.Code
            },
            EditorInputDevice.Mouse => g.Code switch
            { "Left" => "左键", "Right" => "右键", "Middle" => "中键", "X1" => "侧键1", "X2" => "侧键2", _ => g.Code },
            EditorInputDevice.Wheel => "滚轮",
            _ => g.Code
        };
        var kd = g.Kind switch { EditorInputGestureKind.MouseDrag => "拖动", EditorInputGestureKind.MouseWheel => "", _ => "" };
        return $"{mt}{kt}{kd}";
    }
}

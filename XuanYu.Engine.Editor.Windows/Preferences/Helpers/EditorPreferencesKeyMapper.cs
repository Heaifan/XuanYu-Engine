using Avalonia.Input;
using XuanYu.Engine.Editor.Input.Bindings;

namespace FluidWarfare.Editor.Windows.Preferences;

static class EditorPreferencesKeyMapper
{
    public static EditorInputModifiers ToMods(KeyModifiers km)
    {
        var r = EditorInputModifiers.None;
        if ((km & KeyModifiers.Shift) != 0) r |= EditorInputModifiers.Shift;
        if ((km & KeyModifiers.Control) != 0) r |= EditorInputModifiers.Control;
        if ((km & KeyModifiers.Alt) != 0) r |= EditorInputModifiers.Alt;
        return r;
    }

    public static string? KeyToCode(Key key) => key switch
    {
        Key.A => "A", Key.B => "B", Key.C => "C", Key.D => "D",
        Key.E => "E", Key.F => "F", Key.G => "G", Key.H => "H",
        Key.I => "I", Key.J => "J", Key.K => "K", Key.L => "L",
        Key.M => "M", Key.N => "N", Key.O => "O", Key.P => "P",
        Key.Q => "Q", Key.R => "R", Key.S => "S", Key.T => "T",
        Key.U => "U", Key.V => "V", Key.W => "W", Key.X => "X",
        Key.Y => "Y", Key.Z => "Z",
        Key.D0 => "0", Key.D1 => "1", Key.D2 => "2", Key.D3 => "3",
        Key.D4 => "4", Key.D5 => "5", Key.D6 => "6", Key.D7 => "7",
        Key.D8 => "8", Key.D9 => "9",
        Key.NumPad0 => "Numpad0", Key.NumPad1 => "Numpad1",
        Key.NumPad2 => "Numpad2", Key.NumPad3 => "Numpad3",
        Key.NumPad4 => "Numpad4", Key.NumPad5 => "Numpad5",
        Key.NumPad6 => "Numpad6", Key.NumPad7 => "Numpad7",
        Key.NumPad8 => "Numpad8", Key.NumPad9 => "Numpad9",
        Key.F1 => "F1", Key.F2 => "F2", Key.F3 => "F3", Key.F4 => "F4",
        Key.F5 => "F5", Key.F6 => "F6", Key.F7 => "F7", Key.F8 => "F8",
        Key.F9 => "F9", Key.F10 => "F10", Key.F11 => "F11", Key.F12 => "F12",
        Key.Escape => "Escape", Key.Space => "Space", Key.Enter => "Enter",
        Key.Tab => "Tab", Key.Back => "Back", Key.Delete => "Delete",
        Key.Insert => "Insert", Key.Home => "Home", Key.End => "End",
        Key.PageUp => "PageUp", Key.PageDown => "PageDown",
        Key.Left => "Left", Key.Right => "Right", Key.Up => "Up", Key.Down => "Down",
        Key.OemPlus => "Equals", Key.OemMinus => "Minus",
        Key.OemPeriod => "Period", Key.OemComma => "Comma",
        Key.OemQuestion => "Slash", Key.OemSemicolon => "Semicolon",
        Key.OemQuotes => "Quote", Key.OemTilde => "Backtick",
        Key.OemBackslash => "Backslash", Key.OemOpenBrackets => "LeftBracket",
        Key.OemCloseBrackets => "RightBracket",
        _ => null
    };
}

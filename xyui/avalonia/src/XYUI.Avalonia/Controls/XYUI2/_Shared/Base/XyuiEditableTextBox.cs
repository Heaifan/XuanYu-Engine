using Avalonia.Controls;
using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public abstract class XyuiEditableTextBox : TextBox
{
    bool _focusSessionActive;

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        var wasFocused = IsFocused;
        base.OnPointerPressed(e);
        if (wasFocused) _focusSessionActive = false;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (_focusSessionActive && IsFocused && !IsReadOnly && !string.IsNullOrEmpty(Text)) { SelectAll(); _focusSessionActive = false; }
    }

    protected override void OnGotFocus(FocusChangedEventArgs e)
    {
        base.OnGotFocus(e);
        if (!IsReadOnly && !string.IsNullOrEmpty(Text))
        {
            _focusSessionActive = true; SelectAll();
        }
    }

    protected override void OnLostFocus(FocusChangedEventArgs e) { base.OnLostFocus(e); _focusSessionActive = false; }
}

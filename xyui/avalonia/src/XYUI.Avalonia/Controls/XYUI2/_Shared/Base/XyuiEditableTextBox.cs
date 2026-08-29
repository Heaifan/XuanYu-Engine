using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;

namespace XYUI.Avalonia.Controls;

public abstract class XyuiEditableTextBox : TextBox
{
    bool _focusSessionActive;
    bool _pointerPressActive;

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        var wasFocused = IsFocused; _pointerPressActive = true;
        base.OnPointerPressed(e);
        _pointerPressActive = false; if (wasFocused) _focusSessionActive = false;
    }

    protected override void OnGotFocus(FocusChangedEventArgs e)
    {
        base.OnGotFocus(e);
        if (!IsReadOnly && !string.IsNullOrEmpty(Text))
        {
            _focusSessionActive = true; SelectAll(); Dispatcher.UIThread.Post(CompleteFocusSession, DispatcherPriority.Input);
        }
    }

    protected override void OnLostFocus(FocusChangedEventArgs e) { base.OnLostFocus(e); _focusSessionActive = false; }
    void CompleteFocusSession() { if (_focusSessionActive && IsFocused && !_pointerPressActive && !IsReadOnly && !string.IsNullOrEmpty(Text) && (SelectionStart != 0 || SelectionEnd != Text.Length)) SelectAll(); _focusSessionActive = false; }
}

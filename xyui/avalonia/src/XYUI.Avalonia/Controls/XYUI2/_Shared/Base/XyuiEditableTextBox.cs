using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;

namespace XYUI.Avalonia.Controls;

public abstract class XyuiEditableTextBox : TextBox
{
    bool _selectAllOnPointerRelease;
    bool _focusSelectAllPending;
    protected virtual bool SelectAllOnPointerActivation => true;

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        var wasFocused = IsFocused;
        _selectAllOnPointerRelease = SelectAllOnPointerActivation && !IsReadOnly && !string.IsNullOrEmpty(Text);
        base.OnPointerPressed(e);
        if (wasFocused) _focusSelectAllPending = false;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (_selectAllOnPointerRelease)
        {
            _selectAllOnPointerRelease = false; SelectAll();
            Dispatcher.UIThread.Post(SelectAll, DispatcherPriority.Input);
        }
    }

    protected override void OnGotFocus(FocusChangedEventArgs e)
    {
        base.OnGotFocus(e);
        if (!IsReadOnly && !string.IsNullOrEmpty(Text))
        {
            _focusSelectAllPending = true; SelectAll(); Dispatcher.UIThread.Post(() => { if (_focusSelectAllPending) SelectAll(); _focusSelectAllPending = false; }, DispatcherPriority.Input);
        }
    }
}

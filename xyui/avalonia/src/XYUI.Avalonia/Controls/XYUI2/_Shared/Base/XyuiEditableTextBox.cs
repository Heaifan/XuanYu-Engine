using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;

namespace XYUI.Avalonia.Controls;

public abstract class XyuiEditableTextBox : TextBox
{
    bool _selectAllOnPointerRelease;

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        _selectAllOnPointerRelease = !IsReadOnly && !string.IsNullOrEmpty(Text);
        base.OnPointerPressed(e);
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
            SelectAll(); Dispatcher.UIThread.Post(SelectAll, DispatcherPriority.Input);
        }
    }
}

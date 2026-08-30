using Avalonia.Controls;
using Avalonia.Media;

namespace XYUI.Avalonia.Controls;

public partial class XYSelect
{
    internal void SetDropDownOpen(bool value)
    {
        if (!IsEnabled) value = false;
        if (_isDropDownOpen == value) return;
        _isDropDownOpen = value; Classes.Set("xyui-select-open", value);
        if (ChevronPart is not null) ChevronPart.RenderTransform = value ? new RotateTransform(180) : null;
        if (value) OpenPopup(); else ClosePopupForLifecycle();
    }

    internal void OpenPopup()
    {
        if (!IsEnabled || PopupPart is null || ListPart is null) return;
        SyncItems(); ListPart.SelectedIndex = SelectedIndex; PopupPart.Height = double.NaN; PopupPart.IsVisible = true; PopupPart.PlacementTarget = this; PopupPart.Width = Bounds.Width; PopupPart.IsOpen = true;
    }

    internal void ClosePopupForLifecycle()
    {
        _isDropDownOpen = false; Classes.Set("xyui-select-open", false);
        if (ChevronPart is not null) ChevronPart.RenderTransform = null;
        if (PopupPart is not null) { PopupPart.IsOpen = false; PopupPart.IsVisible = false; PopupPart.Height = 0; }
    }

    internal void SyncParts()
    {
        SyncItems();
        if (ValuePart is null) return;
        var text = SelectedItem?.ToString(); var hasValue = !string.IsNullOrEmpty(text);
        ValuePart.Text = hasValue ? text : Placeholder; ValuePart.Classes.Set("xyui-select-placeholder", !hasValue);
    }

    void SyncItems()
    {
        if (ListPart is null) return;
        IsKeyboardNavigating = true; ListPart.ItemsSource = ItemsSource ?? Items; ListPart.SelectedIndex = SelectedIndex; IsKeyboardNavigating = false;
    }
}

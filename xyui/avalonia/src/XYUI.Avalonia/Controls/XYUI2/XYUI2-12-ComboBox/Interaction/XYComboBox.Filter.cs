using Avalonia;
using Avalonia.Media;
using System.Collections;

namespace XYUI.Avalonia.Controls;

public partial class XYComboBox
{
    void OnComboPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == ItemsSourceProperty) RefreshItems(false);
        if (e.Property == SelectedItemProperty && SelectedItem is object item) { Text = item.ToString(); SyncText(); }
        if (e.Property == PlaceholderProperty && TextFieldPart is not null) TextFieldPart.Placeholder = Placeholder;
        if (e.Property == TextProperty) SyncText();
    }

    void RefreshItems(bool open)
    {
        if (open) ShowingAllItems = false;
        var all = (ItemsSource as IEnumerable)?.Cast<object>().ToArray() ?? []; var query = Text ?? "";
        FilteredItems = (open ? all.Where(x => (x?.ToString() ?? "").Contains(query, StringComparison.OrdinalIgnoreCase)) : all).ToArray();
        if (ListPart is not null) { ListPart.ItemsSource = FilteredItems; ListPart.ItemTemplate = ItemTemplate; }
        if (open && all.Length > 0) { IsDropDownOpen = true; OpenPopup(); }
    }

    void SetDropDownOpen(bool value)
    {
        if (_isDropDownOpen == value) return; _isDropDownOpen = value; Classes.Set("xyui-combo-open", value); if (ChevronPart?.Content is XYIcon icon) icon.RenderTransform = value ? new RotateTransform(180) : null;
        if (PopupPart is null) return;
        if (value) OpenPopup(); else { PopupPart.IsOpen = false; PopupPart.IsVisible = false; PopupPart.Height = 0; }
    }
}

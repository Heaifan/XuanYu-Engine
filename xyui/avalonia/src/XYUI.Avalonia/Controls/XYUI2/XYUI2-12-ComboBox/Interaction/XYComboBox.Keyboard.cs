using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public partial class XYComboBox
{
    internal void OnComboKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Down) { IsKeyboardSelecting = true; if (!IsDropDownOpen) { RefreshItems(false); IsDropDownOpen = true; ListPart!.SelectedIndex = 0; } else ListPart!.SelectedIndex = Math.Min(ListPart.SelectedIndex + 1, ListPart.ItemCount - 1); IsKeyboardSelecting = false; e.Handled = true; }
        else if (e.Key == Key.Up && IsDropDownOpen) { IsKeyboardSelecting = true; ListPart!.SelectedIndex = Math.Max(ListPart.SelectedIndex - 1, 0); IsKeyboardSelecting = false; e.Handled = true; }
        else if (e.Key == Key.Enter) { if (IsDropDownOpen && ListPart?.SelectedItem is object item) SelectItem(item); else CommitText(); e.Handled = true; }
        else if (e.Key == Key.Escape && IsDropDownOpen) { IsDropDownOpen = false; e.Handled = true; }
    }

    void CommitText()
    {
        var match = (ItemsSource as System.Collections.IEnumerable)?.Cast<object>().FirstOrDefault(x => string.Equals(x?.ToString(), Text, StringComparison.OrdinalIgnoreCase));
        if (match is not null) SelectItem(match); else if (!IsCustomValueAllowed) IsError = true; else { SelectedItem = null; IsError = false; }
    }
}

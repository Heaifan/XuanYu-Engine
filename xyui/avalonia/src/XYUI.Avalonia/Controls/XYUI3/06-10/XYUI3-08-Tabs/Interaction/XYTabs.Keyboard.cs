using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYTabs
{
    void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key is not (Key.Left or Key.Right or Key.Home or Key.End)) return;
        var items = Items.Where(item => item.IsEnabled).ToArray();
        if (items.Length == 0) return;
        var index = Array.IndexOf(items, SelectedItem);
        var target = e.Key == Key.Home ? items[0]
            : e.Key == Key.End ? items[^1]
            : items[Math.Clamp(index + (e.Key == Key.Right ? 1 : -1), 0, items.Length - 1)];
        Select(target);
        e.Handled = true;
    }
}

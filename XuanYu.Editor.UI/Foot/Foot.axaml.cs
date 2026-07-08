using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;

namespace XuanYu.Editor.UI;

public partial class Foot : UserControl
{
    public Foot() => InitializeComponent();

    void LogList_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox lb && DataContext is UiVm vm)
            vm.SetSelectedEntries(lb.SelectedItems?.OfType<LogEntry>().ToArray() ?? []);
    }

    async void LogList_KeyUp(object? sender, KeyEventArgs e)
    {
        if (sender is not ListBox lb || DataContext is not UiVm vm) return;
        if (e.Key == Key.A && e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            lb.SelectAll();
            e.Handled = true;
        }
        else if (e.Key == Key.C && e.KeyModifiers.HasFlag(KeyModifiers.Control)
                 && vm.SelectedEntriesClipboardText is { Length: > 0 } text)
        {
            var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
            if (clipboard is not null) await clipboard.SetTextAsync(text);
            e.Handled = true;
        }
    }
}

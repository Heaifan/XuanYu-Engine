using System.Diagnostics;
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

    async void LogList_KeyDown(object? sender, KeyEventArgs e)
    {
        if (sender is not ListBox lb || DataContext is not UiVm vm) return;
        if (!e.KeyModifiers.HasFlag(KeyModifiers.Control)) return;
        if (e.Key == Key.A) { lb.SelectAll(); e.Handled = true; return; }
        if (e.Key == Key.C && vm.HasSelectedEntries)
        {
            var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
            if (clipboard is not null)
            {
                try { await clipboard.SetTextAsync(vm.SelectedEntriesClipboardText); }
                catch (Exception ex) { Debug.WriteLine($"[LogList] 复制失败: {ex}"); }
            }
            vm.NotifyLogCopied();
            e.Handled = true;
        }
    }
}

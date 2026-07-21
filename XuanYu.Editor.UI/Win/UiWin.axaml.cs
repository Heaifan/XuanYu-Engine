using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;

namespace XuanYu.Editor.UI;

public partial class UiWin : Window
{
    public UiWin()
    {
        InitializeComponent();
        AddHandler(KeyDownEvent, Window_KeyDown, RoutingStrategies.Tunnel);
        Deactivated += (_, _) => (DataContext as UiVm)?.CancelInteractionFromWindowDeactivated();
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        (DataContext as UiVm)?.CancelInteractionFromWindowClosing();
        base.OnClosing(e);
    }

    async void Window_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.C && e.KeyModifiers.HasFlag(KeyModifiers.Control)
            && DataContext is UiVm { IsLogOpen: true, HasSelectedEntries: true } copyVm)
        {
            if (await CopySelectedLogs(copyVm)) e.Handled = true;
            return;
        }

        if (e.Key == Key.Z && e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            (DataContext as UiVm)?.TryUndoFromShortcut();
            e.Handled = true;
            return;
        }

        if (e.Key != Key.Escape) return;
        (DataContext as UiVm)?.CancelInteractionFromEscape();
        e.Handled = true;
    }

    async Task<bool> CopySelectedLogs(UiVm vm)
    {
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard is null) return false;
        try { await clipboard.SetTextAsync(vm.SelectedEntriesClipboardText); }
        catch (Exception ex) { Debug.WriteLine($"[LogList] window copy failed: {ex}"); return false; }
        vm.NotifyLogCopied();
        return true;
    }
}

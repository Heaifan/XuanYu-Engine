using Avalonia.Input;
using Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class UiWin
{
    async void Window_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Tab && TopLevel.GetTopLevel(this)?.FocusManager?.GetFocusedElement() is not TextBox)
        { (DataContext as UiVm)?.ToggleEditorMode(); e.Handled = true; return; }
        if (HandleEntityShortcut(e)) return;
        if (e.Key == Key.C && e.KeyModifiers.HasFlag(KeyModifiers.Control)
            && DataContext is UiVm { IsLogOpen: true, HasSelectedEntries: true } copyVm)
        { if (await CopySelectedLogs(copyVm)) e.Handled = true; return; }
        if (e.Key == Key.Z && e.KeyModifiers.HasFlag(KeyModifiers.Control))
        { (DataContext as UiVm)?.TryUndoFromShortcut(); e.Handled = true; return; }
        if (e.Key == Key.Y && e.KeyModifiers.HasFlag(KeyModifiers.Control))
        { (DataContext as UiVm)?.TryRedoFromShortcut(); e.Handled = true; return; }
        if (await HandleSceneShortcut(e)) return;
        if (e.Key == Key.Enter && (DataContext as UiVm)?.CommitDrawingFromEnter() == true)
        { e.Handled = true; return; }
        if (e.Key != Key.Escape) return;
        (DataContext as UiVm)?.CancelInteractionFromEscape(); e.Handled = true;
    }
}

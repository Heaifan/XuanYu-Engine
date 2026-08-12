using System;
using System.Linq;
using Avalonia.Input;

namespace XuanYu.Editor.UI;

public partial class UiWin
{
    void DialogCard_KeyDown(object? sender, KeyEventArgs e)
    {
        if (TryHandleDialogKey(e)) e.Handled = true;
    }

    bool TryHandleDialogKey(KeyEventArgs e)
    {
        if (_dialogTcs is null) return false;
        if (e.Key == Key.Tab)
        {
            var focusables = DialogButtons.Children.OfType<Avalonia.Controls.Button>()
                .Where(b => b.IsEnabled && b.IsVisible).ToArray();
            if (focusables.Length == 0) return true;
            var current = CurrentFocus();
            var index = DialogFocusTrap.NextIndex(focusables.Length,
                Array.IndexOf(focusables, current),
                e.KeyModifiers.HasFlag(KeyModifiers.Shift));
            focusables[index].Focus();
            return true;
        }
        if (e.Key == Key.Escape) { CompleteDialog("cancel"); return true; }
        if (e.Key != Key.Enter || _dialogDefault is null) return false;
        CompleteDialog((string)_dialogDefault.Content!);
        return true;
    }
}

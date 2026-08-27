using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public partial class XYNumberField
{
    void OnNumberKeyDown(object? sender, KeyEventArgs e)
    {
        var amount = (e.KeyModifiers & KeyModifiers.Shift) != 0 ? LargeStep : (e.KeyModifiers & KeyModifiers.Control) != 0 ? SmallStep : Step;
        if (e.Key == Key.Up) { Adjust(amount); e.Handled = true; }
        else if (e.Key == Key.Down) { Adjust(-amount); e.Handled = true; }
        else if (e.Key == Key.Enter) { CommitText(); e.Handled = true; }
        else if (e.Key == Key.Escape) { if (IsScrubbing || IsScrubArmed) CancelScrub(); else { Value = EditStartValue; SyncText(); IsError = false; } e.Handled = true; }
    }
}

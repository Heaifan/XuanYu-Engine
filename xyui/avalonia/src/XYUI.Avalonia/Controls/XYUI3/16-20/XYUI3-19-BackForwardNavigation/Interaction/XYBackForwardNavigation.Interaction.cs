using Avalonia.Controls;
using Avalonia.Input;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYBackForwardNavigation
{
    void OnBackPointerPressed(object? sender, PointerPressedEventArgs e) { if (e.GetCurrentPoint(BackButton).Properties.PointerUpdateKind == PointerUpdateKind.RightButtonPressed) { OpenHistory(true); e.Handled = true; } }
    void OnForwardPointerPressed(object? sender, PointerPressedEventArgs e) { if (e.GetCurrentPoint(ForwardButton).Properties.PointerUpdateKind == PointerUpdateKind.RightButtonPressed) { OpenHistory(false); e.Handled = true; } }
    void OpenHistory(bool back)
    {
        CloseHistory(); var indexes = back ? Enumerable.Range(0, _index).Reverse().ToArray() : Enumerable.Range(_index + 1, Math.Max(0, _history.Count - _index - 1)).ToArray(); if (indexes.Length == 0) return;
        var menu = back ? BackHistoryMenu : ForwardHistoryMenu; menu.Items = indexes.Select(i => HistoryItem(i, _history[i], menu)).Cast<Control>().ToArray(); var popup = back ? BackHistoryPopup : ForwardHistoryPopup; popup.Child = menu; popup.PlacementTarget = back ? BackButton : ForwardButton; popup.IsOpen = true; menu.ApplyOverlayStyling(); menu.Open();
    }
    XYMenuItem HistoryItem(int index, string label, XYMenu menu) { var item = new XYMenuItem { Label = label }; item.SelectionRequested += (_, _) => { JumpTo(index); menu.Close(); }; return item; }
    void JumpTo(int index) { if (index < 0 || index >= _history.Count) return; _index = index; Sync(); LocationChanged?.Invoke(this, CurrentLocation!); CloseHistory(); }
    public void CloseHistory() { BackHistoryPopup.IsOpen = false; ForwardHistoryPopup.IsOpen = false; BackHistoryMenu.Close(); ForwardHistoryMenu.Close(); }
    protected override void OnKeyDown(KeyEventArgs e)
    {
        if ((e.KeyModifiers & KeyModifiers.Alt) != 0 && e.Key == Key.Left) { Back(); e.Handled = true; return; }
        if ((e.KeyModifiers & KeyModifiers.Alt) != 0 && e.Key == Key.Right) { Forward(); e.Handled = true; return; }
        if (e.Key == Key.Escape && (BackHistoryPopup.IsOpen || ForwardHistoryPopup.IsOpen)) { CloseHistory(); e.Handled = true; return; }
        base.OnKeyDown(e);
    }
}

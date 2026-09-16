using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public sealed class XYMenuHost : ContentControl
{
    readonly Popup _popup = new() { Placement = PlacementMode.Bottom, IsLightDismissEnabled = true };
    XYMenu? _menu;
    bool _closing;

    public XYMenu? Menu
    {
        get => _menu;
        set
        {
            if (ReferenceEquals(_menu, value)) return;
            Close();
            if (_menu is not null) _menu.Closed -= OnMenuClosed;
            _menu = value;
            if (_menu is not null) _menu.Closed += OnMenuClosed;
            Content = value;
        }
    }

    public Control? Target { get; set; }
    public bool IsOpen { get; private set; }

    public event EventHandler? Opened;
    public event EventHandler? Closed;

    public XYMenuHost()
    {
        Classes.Add("xyui-menu-host");
        Width = 0;
        Height = 0;
        IsHitTestVisible = false;
        _popup.Closed += OnPopupClosed;
    }

    public void Toggle()
    {
        if (IsOpen) Close(); else Open();
    }

    public void Open()
    {
        if (Target is null || Menu is null || IsOpen) return;
        Menu.FocusRestoreTarget = Target;
        Content = null;
        _popup.PlacementTarget = Target;
        _popup.Child = Menu;
        _popup.IsOpen = true;
        IsOpen = true;
        Menu.ApplyOverlayStyling();
        Menu.Open();
        Opened?.Invoke(this, EventArgs.Empty);
    }

    public void Close()
    {
        if (_closing || (!IsOpen && _popup.Child is null)) return;
        _closing = true;
        IsOpen = false;
        _popup.IsOpen = false;
        _popup.Child = null;
        Menu?.Close();
        if (Menu is not null && Content is null) Content = Menu;
        _closing = false;
        Closed?.Invoke(this, EventArgs.Empty);
    }

    void OnPopupClosed(object? sender, EventArgs e) => Close();
    void OnMenuClosed(object? sender, EventArgs e) => Close();

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape && IsOpen) { Close(); e.Handled = true; return; }
        base.OnKeyDown(e);
    }
}

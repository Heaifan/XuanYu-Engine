using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYTabBar
{
    const double ScrollStep = 120;
    public event EventHandler? NewRequested;

    void InitializeInteraction()
    {
        PreviousButton.Click += (_, _) => ScrollBy(-ScrollStep);
        NextButton.Click += (_, _) => ScrollBy(ScrollStep);
        OverflowButton.Click += (_, _) => ToggleOverflow();
        NewButton.Click += (_, _) => NewRequested?.Invoke(this, EventArgs.Empty);
        _viewport.PointerWheelChanged += OnWheel;
        _viewport.ScrollChanged += (_, _) => RefreshScrollState();
        LayoutUpdated += (_, _) => RefreshScrollState();
        Tabs.TabClosed += (_, _) => { CloseOverflow(); RefreshScrollState(); };
    }

    public void ScrollBy(double delta)
    {
        var maximum = Math.Max(0, _viewport.Extent.Width - _viewport.Viewport.Width);
        _viewport.Offset = new global::Avalonia.Vector(Math.Clamp(_viewport.Offset.X + delta, 0, maximum), 0);
        RefreshScrollState();
    }

    void OnWheel(object? sender, PointerWheelEventArgs e)
    {
        if (Math.Abs(e.Delta.Y) < double.Epsilon) return;
        ScrollBy(-e.Delta.Y * ScrollStep); e.Handled = true;
    }

    void ToggleOverflow()
    {
        if (_overflowPopup.IsOpen) { CloseOverflow(); return; }
        var items = Tabs.Items.Select(TabMenuItem).Cast<Control>().ToArray();
        var menu = new XYMenu(items); menu.Closed += (_, _) => CloseOverflow();
        _overflowPopup.Child = menu; _overflowPopup.PlacementTarget = OverflowButton;
        _overflowPopup.Height = double.NaN; _overflowPopup.IsVisible = true; _overflowPopup.IsOpen = true; menu.Open();
    }

    XYMenuItem TabMenuItem(XYTab tab)
    {
        var item = new XYMenuItem { Label = tab.Label, IsChecked = tab.IsSelected, CheckKind = XyuiMenuCheckKind.Radio };
        item.SelectionRequested += (_, _) => { Tabs.Select(tab); tab.BringIntoView(); CloseOverflow(); };
        return item;
    }

    void CloseOverflow()
    {
        _overflowPopup.IsOpen = false; _overflowPopup.IsVisible = false; _overflowPopup.Height = 0;
    }

    void RefreshScrollState()
    {
        var maximum = Math.Max(0, _viewport.Extent.Width - _viewport.Viewport.Width);
        PreviousButton.IsEnabled = _viewport.Offset.X > 0.5;
        NextButton.IsEnabled = _viewport.Offset.X < maximum - 0.5;
        OverflowButton.Classes.Set("xyui-tab-bar-overflow-active", _overflowPopup.IsOpen);
    }
}

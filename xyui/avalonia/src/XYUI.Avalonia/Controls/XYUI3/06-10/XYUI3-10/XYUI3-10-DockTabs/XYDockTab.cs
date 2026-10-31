using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Metadata;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYDockTab : Border
{
    XYIcon _grip = null!;
    Border _gripHitArea = null!;
    XYTab? _tab;
    [Content] public XYTab Tab { get => _tab!; set { if (ReferenceEquals(_tab, value)) return; DetachTab(); _tab = value; AttachTab(); if (_gripHitArea is not null) Child = Build(); } }
    public XYIcon Grip => _grip;
    public Border GripHitArea => _gripHitArea;
    public XYDockTab() : this(new XYTab()) { }
    public XYDockTab(XYTab tab) { Classes.Add("xyui-dock-tab"); _tab = tab; AttachTab(); Child = Build(); RefreshSelected(); InitializeInteraction(); }
    void AttachTab() { if (_tab is null) return; _tab.Classes.Add("xyui-dock-tab-inner"); _tab.VerticalAlignment = VerticalAlignment.Center; _tab.PropertyChanged += OnTabChanged; }
    void DetachTab() { if (_tab is not null) _tab.PropertyChanged -= OnTabChanged; }
    void OnTabChanged(object? sender, AvaloniaPropertyChangedEventArgs change) { if (change.Property == XYTab.IsSelectedProperty) RefreshSelected(); }
    Grid Build()
    {
        _grip = new XYIcon { Icon = XyuiVectorIcon.DragGrip, Size = XyuiIconSize.Tiny, Classes = { "xyui-dock-grip" }, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        var indicator = new Border { Classes = { "xyui-dock-drop-indicator" }, IsVisible = false, IsHitTestVisible = false, Width = 2, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Stretch };
        var divider = new XYSeparator { Variant = XyuiSeparatorVariant.VerticalSplit, Classes = { "xyui-dock-divider" } }; _gripHitArea = new Border { Width = XyuiCompactNavigationTokens.DockGripHitWidth, Background = Brushes.Transparent, Child = _grip, Classes = { "xyui-dock-grip-hit-area" } };
        var grid = new Grid { ColumnDefinitions = new ColumnDefinitions($"{XyuiCompactNavigationTokens.DockGripHitWidth},*,Auto") }; Add(grid, _gripHitArea, 0); Add(grid, Tab, 1); Add(grid, divider, 2); grid.Children.Add(indicator); Grid.SetColumnSpan(indicator, 3); return grid;
    }
    void RefreshSelected() => Classes.Set("xyui-dock-tab-selected", Tab.IsSelected);
    internal void SetDropIndicator(bool visible, bool after = false) { if (Child is not Grid grid) return; var indicator = grid.Children.OfType<Border>().FirstOrDefault(x => x.Classes.Contains("xyui-dock-drop-indicator")); if (indicator is null) return; indicator.IsVisible = visible; indicator.HorizontalAlignment = after ? HorizontalAlignment.Right : HorizontalAlignment.Left; }
    static void Add(Grid grid, Control control, int column) { grid.Children.Add(control); Grid.SetColumn(control, column); }
}

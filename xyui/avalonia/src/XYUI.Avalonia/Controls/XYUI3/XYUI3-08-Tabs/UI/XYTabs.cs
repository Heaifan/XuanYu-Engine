using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYTabs : Border
{
    readonly StackPanel _panel = new() { Orientation = Orientation.Horizontal };
    readonly List<XYTab> _items = [];
    public IReadOnlyList<XYTab> Items => _items;
    public event EventHandler<XYTab>? TabClosed;
    public event EventHandler<XYTab>? SelectionChanged;
    public XYTabs() { Classes.Add("xyui-tabs"); Child = _panel; }
    public XYTabs(params XYTab[] items) : this() { _items.AddRange(items); if (_items.Count > 0 && !_items.Any(x => x.IsSelected)) _items[0].IsSelected = true; Build(); }
    public void Build()
    {
        _panel.Children.Clear();
        foreach (var item in Items) { item.SelectionRequested -= OnSelected; item.SelectionRequested += OnSelected; item.CloseRequested -= OnCloseRequested; item.CloseRequested += OnCloseRequested; item.PropertyChanged -= OnTabPropertyChanged; item.PropertyChanged += OnTabPropertyChanged; _panel.Children.Add(item); }
    }
    void OnTabPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e) { if (e.Property == XYTab.IsSelectedProperty && sender is XYTab tab && tab.IsSelected) Select(tab); }
}

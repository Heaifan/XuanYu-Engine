using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed class XYBackForwardNavigation : Border
{
    readonly List<string> _history = []; int _index = -1;
    public XYIconButton BackButton { get; } = Action(XyuiVectorIcon.ChevronLeft);
    public XYIconButton ForwardButton { get; } = Action(XyuiVectorIcon.ChevronRight);
    public string? CurrentLocation => _index >= 0 ? _history[_index] : null;
    public bool CanGoBack => _index > 0; public bool CanGoForward => _index >= 0 && _index < _history.Count - 1;
    public event EventHandler<string>? LocationChanged;
    public XYBackForwardNavigation() { Classes.Add("xyui-back-forward"); BackButton.Click += (_, _) => Back(); ForwardButton.Click += (_, _) => Forward(); Child = Build(); Sync(); }
    Control Build() { var grid = new Grid { Width = 500, Height = 60, ColumnDefinitions = new ColumnDefinitions("34,34,1,*") }; var surface = new Border { Margin = new Avalonia.Thickness(10, 12), Classes = { "xyui-back-forward-surface" } }; Grid.SetColumnSpan(surface, 4); grid.Children.Add(surface); Grid.SetColumn(BackButton, 0); Grid.SetColumn(ForwardButton, 1); grid.Children.Add(BackButton); grid.Children.Add(ForwardButton); var location = new StackPanel { Margin = new Avalonia.Thickness(14, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center }; location.Children.Add(new TextBlock { Text = "当前位置", Classes = { "xyui-location-title" } }); location.Children.Add(new TextBlock { Text = CurrentLocation ?? "roads / 道路编辑", Classes = { "xyui-location-text" } }); Grid.SetColumn(location, 3); grid.Children.Add(location); return grid; }
    public void Navigate(string location) { if (_index >= 0 && _history[_index] == location) return; if (_index < _history.Count - 1) _history.RemoveRange(_index + 1, _history.Count - _index - 1); _history.Add(location); _index++; Sync(); LocationChanged?.Invoke(this, location); }
    public void Back() { if (!CanGoBack) return; _index--; Sync(); LocationChanged?.Invoke(this, CurrentLocation!); }
    public void Forward() { if (!CanGoForward) return; _index++; Sync(); LocationChanged?.Invoke(this, CurrentLocation!); }
    void Sync() { BackButton.IsEnabled = CanGoBack; ForwardButton.IsEnabled = CanGoForward; }
    static XYIconButton Action(XyuiVectorIcon icon) => new() { Content = new XYIcon { Icon = icon, Size = XyuiIconSize.Small }, Classes = { "xyui-back-forward-action" } };
}

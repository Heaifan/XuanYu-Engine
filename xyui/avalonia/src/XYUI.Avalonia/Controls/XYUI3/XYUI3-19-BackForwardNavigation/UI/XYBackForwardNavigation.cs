using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYBackForwardNavigation : Border
{
    readonly List<string> _history = []; int _index = -1;
    readonly TextBlock _location = new() { Classes = { "xyui-location-text" }, TextTrimming = TextTrimming.CharacterEllipsis };
    public XYIconButton BackButton { get; } = Action(XyuiVectorIcon.ChevronLeft);
    public XYIconButton ForwardButton { get; } = Action(XyuiVectorIcon.ChevronRight);
    public IReadOnlyList<string> History => _history;
    public int CurrentIndex => _index;
    public string? CurrentLocation => _index >= 0 ? _history[_index] : null;
    public bool CanGoBack => _index > 0; public bool CanGoForward => _index >= 0 && _index < _history.Count - 1;
    public XYMenu BackHistoryMenu { get; } = new(); public XYMenu ForwardHistoryMenu { get; } = new();
    public Popup BackHistoryPopup { get; } = NewPopup(); public Popup ForwardHistoryPopup { get; } = NewPopup();
    public event EventHandler<string>? LocationChanged;
    public XYBackForwardNavigation() { Classes.Add("xyui-back-forward"); BackButton.Click += (_, _) => Back(); ForwardButton.Click += (_, _) => Forward(); BackButton.PointerPressed += OnBackPointerPressed; ForwardButton.PointerPressed += OnForwardPointerPressed; Child = Build(); Sync(); }
    Control Build()
    {
        var inner = new Grid { Height = 34, ColumnDefinitions = new ColumnDefinitions("28,28,1,Auto"), ColumnSpacing = 4, VerticalAlignment = VerticalAlignment.Center };
        BackButton.Width = 28; BackButton.Height = 28; BackButton.VerticalAlignment = VerticalAlignment.Center; ForwardButton.Width = 28; ForwardButton.Height = 28; ForwardButton.VerticalAlignment = VerticalAlignment.Center;
        Grid.SetColumn(BackButton, 0); Grid.SetColumn(ForwardButton, 1); inner.Children.Add(BackButton); inner.Children.Add(ForwardButton);
        var divider = new Border { Width = 1, Height = 20, VerticalAlignment = VerticalAlignment.Center, Classes = { "xyui-back-forward-divider" } }; Grid.SetColumn(divider, 2); inner.Children.Add(divider);
        var location = new StackPanel { VerticalAlignment = VerticalAlignment.Center, MaxWidth = 280 }; location.Children.Add(new TextBlock { Text = "当前位置", Classes = { "xyui-location-title" } }); location.Children.Add(_location); Grid.SetColumn(location, 3); inner.Children.Add(location);
        return new Border { Height = 34, Classes = { "xyui-back-forward-surface" }, Child = inner };
    }
    public void Navigate(string location) { if (_index >= 0 && _history[_index] == location) return; if (_index < _history.Count - 1) _history.RemoveRange(_index + 1, _history.Count - _index - 1); _history.Add(location); _index++; Sync(); LocationChanged?.Invoke(this, location); }
    public void Back() { if (!CanGoBack) return; _index--; Sync(); LocationChanged?.Invoke(this, CurrentLocation!); }
    public void Forward() { if (!CanGoForward) return; _index++; Sync(); LocationChanged?.Invoke(this, CurrentLocation!); }
    void Sync() { BackButton.IsEnabled = CanGoBack; ForwardButton.IsEnabled = CanGoForward; _location.Text = CurrentLocation ?? "—"; }
    static XYIconButton Action(XyuiVectorIcon icon) => new() { Content = new XYIcon { Icon = icon, Size = XyuiIconSize.Small }, Classes = { "xyui-back-forward-action" } };
    static Popup NewPopup() => new() { Placement = PlacementMode.Bottom, IsLightDismissEnabled = true };
}

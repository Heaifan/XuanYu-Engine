using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace XuanYu.Editor.UI;

public sealed class DiagnosticPopupHost : Panel
{
    internal static event Action<TopLevel, bool>? PopupRootChanged;
    readonly Border _bounds = new() { BorderBrush = Brushes.Gold, BorderThickness = new Thickness(2), IsHitTestVisible = false };
    readonly Border _label = new() { Background = Brushes.Black, Padding = new Thickness(4, 1), IsHitTestVisible = false };
    UiVm? _vm;
    bool _popupOpen;
    bool _diagnosticEnabled;
    TopLevel? _popupRoot;

    public static readonly StyledProperty<string> DebugIdProperty =
        AvaloniaProperty.Register<DiagnosticPopupHost, string>(nameof(DebugId), "N/A");
    public string DebugId { get => GetValue(DebugIdProperty); set => SetValue(DebugIdProperty, value); }
    public bool IsDiagnosticVisible => _bounds.IsVisible;
    public int ActiveRectangleCount => IsDiagnosticVisible ? 1 : 0;
    public int ActiveLabelCount => IsDiagnosticVisible ? 1 : 0;

    public DiagnosticPopupHost()
    {
        _label.Child = new TextBlock { Foreground = Brushes.White, FontSize = 10 };
        Children.Add(_bounds); Children.Add(_label);
        DataContextChanged += (_, _) => HookVm(DataContext as UiVm);
        Refresh();
    }

    public void SetPopupOpen(bool open)
    {
        _popupOpen = open;
        if (open) _popupRoot = TopLevel.GetTopLevel(this);
        if ((_popupRoot ?? TopLevel.GetTopLevel(this)) is { } root) PopupRootChanged?.Invoke(root, open);
        if (!open) _popupRoot = null;
        Refresh();
    }
    public void SetDiagnosticEnabled(bool enabled) { _diagnosticEnabled = enabled; Refresh(); }

    protected override Size MeasureOverride(Size availableSize)
    {
        var desired = new Size();
        foreach (var child in Children)
        {
            if (ReferenceEquals(child, _bounds) || ReferenceEquals(child, _label)) continue;
            child.Measure(availableSize); desired = new Size(Math.Max(desired.Width, child.DesiredSize.Width), Math.Max(desired.Height, child.DesiredSize.Height));
        }
        _bounds.Measure(desired); _label.Measure(availableSize); return desired;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        foreach (var child in Children)
            child.Arrange(ReferenceEquals(child, _label) ? new Rect(0, -_label.DesiredSize.Height, _label.DesiredSize.Width, _label.DesiredSize.Height) : new Rect(finalSize));
        return finalSize;
    }

    void HookVm(UiVm? next)
    {
        if (_vm is not null) _vm.PropertyChanged -= OnVmChanged;
        _vm = next;
        if (_vm is not null) _vm.PropertyChanged += OnVmChanged;
        Refresh();
    }

    void OnVmChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    { if (e.PropertyName is nameof(UiVm.IsDiagnosticMode) or nameof(UiVm.IsDiagnosticRegionBoundsMode)) Refresh(); }

    void Refresh()
    {
        var visible = _popupOpen && (_diagnosticEnabled || _vm?.IsDiagnosticRegionBoundsMode == true);
        _bounds.IsVisible = visible; _label.IsVisible = visible;
        if (_label.Child is TextBlock text) text.Text = DebugId;
    }
}

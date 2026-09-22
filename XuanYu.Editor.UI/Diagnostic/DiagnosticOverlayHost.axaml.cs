using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost : UserControl
{
    readonly List<Popup> _popups = [];
    readonly IDiagnosticClipboard _clipboard;
    UiVm? _vm;
    bool _loaded;
    Canvas? _floatingLayer;

    public DiagnosticOverlayHost() : this(new DiagnosticClipboard()) { }

    public DiagnosticOverlayHost(IDiagnosticClipboard clipboard)
    {
        _clipboard = clipboard;
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        DataContextChanged += OnDataContextChanged;
    }

    public int ActivePopupCount => _popups.Count;
    public int ActiveBadgeCount => _popups.Count(x => x.Child is DiagnosticBadge);

    void AttachFloatingLayer(Window window)
    {
        if (window.Content is not Panel root) return;
        if (_floatingLayer?.Parent is Panel) return;
        _floatingLayer = new Canvas { HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch, IsHitTestVisible = true };
        _floatingLayer.SetValue(Panel.ZIndexProperty, 80); root.Children.Add(_floatingLayer);
    }

    void DetachFloatingLayer()
    {
        if (_floatingLayer?.Parent is Panel root) root.Children.Remove(_floatingLayer);
        _floatingLayer = null;
    }

    void OnLoaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _loaded = true;
        AttachTopLevel();
        if (_topLevel is Window window) AttachFloatingLayer(window);
        AttachVm();
        Reconcile();
    }

    void OnUnloaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _loaded = false;
        DetachTopLevel();
        DetachVm();
        CloseAll();
        ClearProbeVisuals();
        Dispatcher.UIThread.Post(DetachFloatingLayer);
    }

    void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (!_loaded) return;
        AttachVm();
        Reconcile();
    }

    void AttachVm(UiVm? next = null)
    {
        DetachVm();
        _vm = next ?? DataContext as UiVm;
        if (_vm is not null) _vm.PropertyChanged += OnVmPropertyChanged;
    }

    void DetachVm()
    {
        if (_vm is not null) _vm.PropertyChanged -= OnVmPropertyChanged;
        _vm = null;
    }

    void OnVmPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(UiVm.IsDiagnosticMode) or nameof(UiVm.IsDiagnosticProbeMode) or
            nameof(UiVm.IsDiagnosticRegionBoundsMode) or nameof(UiVm.InspectorIdentity))
            Reconcile();
    }
}

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
    VulkanNativeHost? _nativeViewportHost;
    readonly List<TopLevel> _probeRoots = [];

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

    void OnLoaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _loaded = true;
        DiagnosticRegistry.Changed += OnDiagnosticRegistryChanged;
        DiagnosticPopupHost.PopupRootChanged += OnPopupRootChanged;
        AttachTopLevel();
        if (_topLevel is Window window)
        {
            AttachFloatingLayer(window);
            AttachNativeViewportProbe(window);
        }
        AttachVm();
        Reconcile();
    }

    void OnUnloaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _loaded = false;
        DiagnosticRegistry.Changed -= OnDiagnosticRegistryChanged;
        DetachNativeViewportProbe();
        DetachTopLevel();
        DiagnosticPopupHost.PopupRootChanged -= OnPopupRootChanged;
        DetachVm();
        CloseAll(); ResetCardPlacement();
        ClearProbeVisuals();
        DisposeToolWindow();
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
        if (e.PropertyName is nameof(UiVm.IsDiagnosticMode) or nameof(UiVm.IsDiagnosticRegionBoundsMode) or nameof(UiVm.InspectorIdentity)) Reconcile();
    }
}

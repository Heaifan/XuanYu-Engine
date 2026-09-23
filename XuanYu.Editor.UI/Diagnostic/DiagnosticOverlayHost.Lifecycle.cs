using Avalonia;
using Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    TopLevel? _topLevel;

    static Panel? EnsureWindowRoot(Window window)
    {
        if (window.Content is Panel panel) return panel;
        if (window.Content is not Control content) return null;
        var root = new Grid { HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch };
        root.Children.Add(content); window.Content = root; return root;
    }

    void AttachTopLevel()
    {
        _topLevel = TopLevel.GetTopLevel(this);
        if (_topLevel is Window window)
        {
            AttachProbeRoot(window);
            window.Activated += OnWindowActivated;
            window.Deactivated += OnWindowDeactivated;
            window.PropertyChanged += OnWindowPropertyChanged;
            window.SizeChanged += OnWindowSizeChanged;
        }
    }

    void DetachTopLevel()
    {
        if (_topLevel is not Window window) return;
        window.Activated -= OnWindowActivated;
        window.Deactivated -= OnWindowDeactivated;
        window.PropertyChanged -= OnWindowPropertyChanged;
        window.SizeChanged -= OnWindowSizeChanged;
        foreach (var root in _probeRoots.ToArray()) DetachProbeRoot(root);
        _topLevel = null;
    }

    void AttachProbeRoot(TopLevel root)
    {
        if (_probeRoots.Contains(root)) return;
        _probeRoots.Add(root); AttachProbeHandlers(root);
    }

    void DetachProbeRoot(TopLevel root)
    {
        DetachProbeHandlers(root); _probeRoots.Remove(root);
    }

    void OnWindowActivated(object? sender, EventArgs e)
    {
        Reconcile();
        if (_vm is null || _vm.IsDiagnosticMode) RenderProbe();
    }

    void OnWindowDeactivated(object? sender, EventArgs e)
    {
        CloseAll();
        ClearProbeVisuals();
    }

    void OnWindowPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == Window.WindowStateProperty && e.NewValue is WindowState.Minimized)
        {
            CloseAll();
            ClearProbeVisuals();
        }
    }

    void OnWindowSizeChanged(object? sender, SizeChangedEventArgs e) => ClampCardToWindow();

    void OnPopupRootChanged(TopLevel root, bool open)
    { if (open) AttachProbeRoot(root); else if (!ReferenceEquals(root, _topLevel)) DetachProbeRoot(root); }
}

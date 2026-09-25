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
            AttachProbePopups(window);
            window.Activated += OnWindowActivated;
            window.Deactivated += OnWindowDeactivated;
            window.Closed += OnWindowClosed;
            window.PropertyChanged += OnWindowPropertyChanged;
        }
    }

    void DetachTopLevel()
    {
        if (_topLevel is not Window window) return;
        window.Activated -= OnWindowActivated;
        window.Deactivated -= OnWindowDeactivated;
        window.Closed -= OnWindowClosed;
        window.PropertyChanged -= OnWindowPropertyChanged;
        foreach (var root in _probeRoots.ToArray()) DetachProbeRoot(root);
        foreach (var popup in _probePopups.ToArray()) DetachProbePopup(popup);
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
        RestoreAfterOwnerActivation();
    }

    void OnWindowDeactivated(object? sender, EventArgs e)
    {
        ClearProbeHighlight();
    }

    void OnWindowClosed(object? sender, EventArgs e)
    {
        ClearProbeVisuals();
        DisposeToolWindow();
    }

    void OnWindowPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == Window.WindowStateProperty && e.NewValue is WindowState.Minimized)
        {
            CloseAll();
            ClearProbeVisuals();
        }
    }

    void OnPopupRootChanged(TopLevel root, bool open)
    {
        if (open) AttachProbeRoot(root);
        else
        {
            ClearProbeForRoot(root);
            if (!ReferenceEquals(root, _topLevel)) DetachProbeRoot(root);
        }
    }
}

using Avalonia;
using Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    TopLevel? _topLevel;

    void AttachTopLevel()
    {
        _topLevel = TopLevel.GetTopLevel(this);
        if (_topLevel is Window window)
        {
            AttachProbeHandlers(window);
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
        DetachProbeHandlers(window);
        _topLevel = null;
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
}

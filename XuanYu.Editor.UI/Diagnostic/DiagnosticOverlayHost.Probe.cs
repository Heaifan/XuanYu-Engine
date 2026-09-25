using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    DiagnosticProbeResult? _probeResult;
    DiagnosticProbeResult? _lockedProbeResult;
    Border? _probeHighlight;

    public int ActiveProbeHighlightCount => _probeHighlight is null ? 0 : 1;
    public int ActiveProbeCardCount => _toolWindow?.IsVisible == true ? 1 : 0;
    public bool IsProbeLocked => _lockedProbeResult is not null;

    public void LockProbe()
    {
        if (_probeResult is not null) _lockedProbeResult = _probeResult;
        RenderProbe();
    }

    public void UnlockProbe()
    {
        _lockedProbeResult = null;
        RenderProbe();
    }

    public void SetProbeResult(DiagnosticProbeResult? result)
    {
        if (result is not null && _probeResult is { } current &&
            ReferenceEquals(current.SemanticTarget ?? current.DeepVisual, result.SemanticTarget ?? result.DeepVisual) &&
            current.ProbeMode == result.ProbeMode) return;
        _probeResult = result;
        if (IsProbeLocked && result is not null) return;
        RenderProbe();
    }

    public void PreviewProbe(DiagnosticProbeResult? result)
    {
        if (IsProbeLocked && result is not null) return;
        SetProbeResult(result);
    }

    public void TrackProbe(DiagnosticProbeResult result)
    {
        _nativeViewportHost = result.DeepVisual as VulkanNativeHost;
        _probeResult = result;
        _lockedProbeResult = result;
        RenderProbe();
    }

    void RenderProbe()
    {
        var result = IsProbeLocked ? _lockedProbeResult : _probeResult;
        if (result is null || !_loaded || _floatingLayer is null)
        {
            ClearProbeHighlight(); HideToolWindow(); return;
        }
        if (!TryGetFloatingBounds(result.DeepVisual, out var bounds)) return;
        ClearProbeHighlight();
        if (!IsViewportDiagnosticTarget(result))
        {
            _probeHighlight = new Border
            {
                Width = bounds.Width, Height = bounds.Height,
                BorderBrush = Brushes.Orange, BorderThickness = new Thickness(2),
                IsHitTestVisible = false,
            };
            Canvas.SetLeft(_probeHighlight, bounds.X); Canvas.SetTop(_probeHighlight, bounds.Y);
            ProbeOwner.Children.Add(_probeHighlight);
        }
        var snapshot = DiagnosticElementSnapshot.Capture(result);
        UpdateToolWindow(snapshot, result.SemanticTarget as Control ?? result.DeepVisual as Control, bounds);
    }
}

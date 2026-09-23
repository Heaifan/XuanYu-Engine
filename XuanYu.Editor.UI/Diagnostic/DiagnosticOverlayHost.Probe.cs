using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    DiagnosticProbeResult? _probeResult;
    DiagnosticProbeResult? _lockedProbeResult;
    Border? _probeHighlight;
    Border? _probeCard;

    public int ActiveProbeHighlightCount => _probeHighlight is null ? 0 : 1;
    public int ActiveProbeCardCount => _probeCard is null ? 0 : 1;
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

    void RenderProbe()
    {
        ClearProbeVisuals();
        var result = IsProbeLocked ? _lockedProbeResult : _probeResult;
        if (result is null || !_loaded || _floatingLayer is null) return;
        if (!TryGetFloatingBounds(result.DeepVisual, out var bounds)) return;
        _probeHighlight = new Border
        {
            Width = bounds.Width, Height = bounds.Height,
            BorderBrush = Brushes.Orange,
            BorderThickness = new Thickness(2), IsHitTestVisible = false,
        };
        var snapshot = DiagnosticElementSnapshot.Capture(result);
        var target = result.SemanticTarget as Control ?? result.DeepVisual as Control;
        var card = new DiagnosticFloatingCard(snapshot,
            text => target is null ? Task.CompletedTask : _clipboard.SetTextAsync(target, text), IsProbeLocked);
        card.IsHitTestVisible = true;
        AttachCardDrag(card);
        card.Expanded += () => OpenDetails(snapshot, target);
        card.Closed += () => { UnlockProbe(); SetProbeResult(null); };
        card.PinToggled += () => { if (IsProbeLocked) UnlockProbe(); else LockProbe(); };
        _probeCard = new Border { Child = card, IsHitTestVisible = true };
        _probeCard.SizeChanged += (_, _) => ClampCardToWindow();
        Canvas.SetLeft(_probeHighlight, bounds.X); Canvas.SetTop(_probeHighlight, bounds.Y);
        _floatingLayer.Children.Add(_probeHighlight);
        _floatingLayer.Children.Add(_probeCard);
        ClampCardToWindow();
        if (!IsProbeLocked)
        {
            PlacePreviewCard(bounds);
        }
        ClampCardToWindow();
        _probeHighlight.SetValue(Panel.ZIndexProperty, 200); _probeCard.SetValue(Panel.ZIndexProperty, 201);
    }

    bool TryGetFloatingBounds(Visual visual, out Rect bounds)
    {
        if (_floatingLayer is not null && visual.TranslatePoint(default, _floatingLayer) is { } local)
        { bounds = new Rect(local, visual.Bounds.Size); return true; }
        if (_floatingLayer is null) { bounds = default; return false; }
        var scale = _topLevel?.RenderScaling ?? 1d;
        var layer = _floatingLayer.PointToScreen(default);
        var origin = visual.PointToScreen(default);
        var point = new Point((origin.X - layer.X) / scale, (origin.Y - layer.Y) / scale);
        bounds = new Rect(point, visual.Bounds.Size); return true;
    }
}

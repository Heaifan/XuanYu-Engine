using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

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
        _probeResult = result;
        RenderProbe();
    }

    void RenderProbe()
    {
        ClearProbeVisuals();
        var result = IsProbeLocked ? _lockedProbeResult : _probeResult;
        if (result is null || !_loaded || _floatingLayer is null) return;
        if (result.DeepVisual.TranslatePoint(default, _floatingLayer) is not { } origin) return;
        var bounds = new Rect(origin, result.DeepVisual.Bounds.Size);
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
        if (!IsProbeLocked)
        {
            CardLeft = bounds.X + bounds.Width + 12;
            CardTop = Math.Max(12, bounds.Y);
        }
        ClampCardToWindow();
        _probeHighlight.SetValue(Panel.ZIndexProperty, 200); _probeCard.SetValue(Panel.ZIndexProperty, 201);
    }
}

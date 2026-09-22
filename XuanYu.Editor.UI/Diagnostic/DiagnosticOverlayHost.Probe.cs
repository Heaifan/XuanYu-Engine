using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    DiagnosticProbeResult? _probeResult;
    DiagnosticProbeResult? _lockedProbeResult;
    Border? _probeHighlight;
    Border? _probeCard;
    Popup? _probePopup;
    OverlayLayer? _overlayLayer;

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
        if (_probeResult is not { } result || !_loaded) return;
        if (result.DeepVisual.TranslatePoint(default, ProbeOwner) is not { } origin) return;
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
        Canvas.SetLeft(_probeHighlight, bounds.X); Canvas.SetTop(_probeHighlight, bounds.Y);
        Canvas.SetLeft(_probeCard, bounds.X); Canvas.SetTop(_probeCard, Math.Max(0, bounds.Y - 24));
        _probeHighlight.SetValue(Panel.ZIndexProperty, 200); _probeCard.SetValue(Panel.ZIndexProperty, 201);
        ProbeOwner.Children.Add(_probeHighlight);
        if (IsProbeLocked) ShowLockedCard();
        else ShowPreviewCard();
    }
}

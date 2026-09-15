using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    DiagnosticProbeResult? _probeResult;
    Border? _probeHighlight;
    Border? _probeCard;
    TextBlock? _traceCard;

    public int ActiveProbeHighlightCount => _probeHighlight is null ? 0 : 1;
    public int ActiveProbeCardCount => _probeCard is null ? 0 : 1;

    public void SetProbeResult(DiagnosticProbeResult? result)
    {
        _probeResult = result;
        RenderProbe();
    }

    void RenderProbe()
    {
        ClearProbeVisuals();
        RenderTraceCard();
        if (_probeResult is not { } result || !_loaded) return;
        DiagnosticProbeTrace.MarkTranslateAttempt();
        if (result.DeepVisual.TranslatePoint(default, ProbeOwner) is not { } origin) return;
        DiagnosticProbeTrace.MarkTranslateSuccess();
        var bounds = new Rect(origin, result.DeepVisual.Bounds.Size);
        _probeHighlight = new Border
        {
            Width = bounds.Width, Height = bounds.Height,
            BorderBrush = Brushes.Orange,
            BorderThickness = new Thickness(2), IsHitTestVisible = false,
        };
        _probeCard = new Border
        {
            Child = new TextBlock { Text = result.ControlType, Foreground = Brushes.White },
            Background = Brushes.Black, Opacity = 0.8,
            Padding = new Thickness(4, 2), IsHitTestVisible = false,
        };
        Canvas.SetLeft(_probeHighlight, bounds.X); Canvas.SetTop(_probeHighlight, bounds.Y);
        Canvas.SetLeft(_probeCard, bounds.X); Canvas.SetTop(_probeCard, Math.Max(0, bounds.Y - 24));
        _probeHighlight.SetValue(Panel.ZIndexProperty, 200); _probeCard.SetValue(Panel.ZIndexProperty, 201);
        ProbeOwner.Children.Add(_probeHighlight); ProbeOwner.Children.Add(_probeCard);
        DiagnosticProbeTrace.MarkRender(); RenderTraceCard();
    }

    void ClearProbeVisuals()
    {
        ProbeOwner.Children.Clear(); _probeHighlight = null; _probeCard = null; _traceCard = null;
    }

    void RenderTraceCard()
    {
        if (_vm?.IsDiagnosticProbeMode != true) return;
        _traceCard ??= new TextBlock { Foreground = Brushes.White, IsHitTestVisible = false };
        _traceCard.Text = DiagnosticProbeTrace.CardText(_vm.IsDiagnosticMode, _vm.IsDiagnosticProbeMode);
        Canvas.SetLeft(_traceCard, 8); Canvas.SetTop(_traceCard, 8);
        _traceCard.SetValue(Panel.ZIndexProperty, 210);
        if (!ProbeOwner.Children.Contains(_traceCard)) ProbeOwner.Children.Add(_traceCard);
    }
}

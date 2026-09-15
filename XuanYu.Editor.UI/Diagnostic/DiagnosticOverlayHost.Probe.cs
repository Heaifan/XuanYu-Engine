using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    DiagnosticProbeResult? _probeResult;
    Border? _probeHighlight;
    Border? _probeCard;

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
        if (_probeResult is not { } result || !_loaded) return;
        if (result.DeepVisual.TranslatePoint(default, ProbeOwner) is not { } origin) return;
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
    }

    void ClearProbeVisuals()
    {
        ProbeOwner.Children.Clear(); _probeHighlight = null; _probeCard = null;
    }
}

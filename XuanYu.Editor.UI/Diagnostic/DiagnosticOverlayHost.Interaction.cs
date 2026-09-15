using Avalonia.Controls;
using Avalonia;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    public DiagnosticProbeResult? CurrentProbeResult => _probeResult;
    public bool ProbeEnabled => _vm?.IsDiagnosticProbeMode == true;

    public void ProbeHover(Visual hit, bool deepVisual)
    {
        if (ProbeEnabled)
        {
            DiagnosticProbeTrace.MarkProbeHover();
            var result = DiagnosticProbeResolver.Resolve(hit, deepVisual);
            DiagnosticProbeTrace.MarkResolverResult(); SetProbeResult(result);
        }
    }

    public async Task ProbeClick()
    {
        if (!ProbeEnabled || _probeResult?.DeepVisual is not Control target) return;
        await _clipboard.SetTextAsync(target, DiagnosticElementFormatter.Format(_probeResult));
    }

    public void ExitProbe()
    {
        _vm?.ExitDiagnosticProbe();
        SetProbeResult(null);
    }

    void AttachProbeHandlers(TopLevel topLevel)
    {
        topLevel.AddHandler(InputElement.PointerMovedEvent, OnProbePointerMoved, RoutingStrategies.Tunnel, true);
        topLevel.AddHandler(InputElement.PointerPressedEvent, OnProbePointerPressed, RoutingStrategies.Tunnel, true);
        topLevel.AddHandler(InputElement.KeyDownEvent, OnProbeKeyDown, RoutingStrategies.Tunnel, true);
    }

    void DetachProbeHandlers(TopLevel topLevel)
    {
        topLevel.RemoveHandler(InputElement.PointerMovedEvent, OnProbePointerMoved);
        topLevel.RemoveHandler(InputElement.PointerPressedEvent, OnProbePointerPressed);
        topLevel.RemoveHandler(InputElement.KeyDownEvent, OnProbeKeyDown);
    }

    void OnProbePointerMoved(object? sender, PointerEventArgs e)
    {
        DiagnosticProbeTrace.MarkPointer(e.Source);
        if (!ProbeEnabled || e.Source is not Visual hit || ReferenceEquals(hit, this)) return;
        ProbeHover(hit, e.KeyModifiers.HasFlag(KeyModifiers.Alt));
    }

    void OnProbePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!ProbeEnabled || e.Source is not Visual hit || ReferenceEquals(hit, this)) return;
        ProbeHover(hit, e.KeyModifiers.HasFlag(KeyModifiers.Alt)); _ = ProbeClick(); e.Handled = true;
    }

    void OnProbeKeyDown(object? sender, KeyEventArgs e)
    {
        if (!ProbeEnabled || e.Key != Key.Escape) return;
        ExitProbe(); e.Handled = true;
    }
}

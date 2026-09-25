using Avalonia.Controls;
using Avalonia;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    public DiagnosticProbeResult? CurrentProbeResult => _probeResult;
    public DiagnosticProbeResult? LockedProbeResult => _lockedProbeResult;
    public bool ProbeEnabled => _vm?.IsDiagnosticMode == true;

    public void ProbeHover(Visual hit, bool deepVisual)
    {
        if (HasNativeViewportOverride) return;
        if (ProbeEnabled) PreviewProbe(DiagnosticProbeResolver.Resolve(hit, deepVisual));
    }

    public void ProbeClick(Visual hit, bool deepVisual = false)
    {
        if (!ProbeEnabled || IsOverlayVisual(hit)) return;
        TrackProbe(DiagnosticProbeResolver.Resolve(hit, deepVisual));
    }

    public async Task ProbeClick()
    {
        if (!ProbeEnabled || _probeResult?.DeepVisual is not Control target) return;
        var snapshot = DiagnosticElementSnapshot.Capture(_probeResult);
        LockProbe();
        await _clipboard.SetTextAsync(target, DiagnosticReportFormatter.FormatAi(snapshot));
    }

    public void ExitProbe()
    {
        UnlockProbe();
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

    void OnProbePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!ProbeEnabled || e.Source is not Visual hit || IsOverlayVisual(hit)) return;
        if (!e.GetCurrentPoint(hit).Properties.IsLeftButtonPressed) return;
        ProbeClick(hit, e.KeyModifiers.HasFlag(KeyModifiers.Alt));
    }

    void OnProbePointerMoved(object? sender, PointerEventArgs e)
    {
        if (!ProbeEnabled || e.Source is not Visual hit || IsOverlayVisual(hit)) return;
        RememberProbePointer(e);
        if (HasNativeViewportOverride) return;
        if (TryRepositionViewportProbe(hit)) return;
        ProbeHover(hit, e.KeyModifiers.HasFlag(KeyModifiers.Alt));
    }

    bool IsOverlayVisual(Visual hit) => hit is DiagnosticFloatingCard or DiagnosticDetailPanel or DiagnosticBadge ||
        ReferenceEquals(hit, _floatingLayer) || hit.GetVisualAncestors().Any(IsOverlayAncestor);

    bool IsOverlayAncestor(Visual visual) => visual is DiagnosticFloatingCard or DiagnosticDetailPanel or DiagnosticBadge ||
        ReferenceEquals(visual, _floatingLayer);

    async void OnProbeKeyDown(object? sender, KeyEventArgs e)
    {
        if (!ProbeEnabled) return;
        if (e.Key == Key.C && e.KeyModifiers.HasFlag(KeyModifiers.Control) &&
            e.KeyModifiers.HasFlag(KeyModifiers.Shift))
        {
            await ProbeClick(); e.Handled = true; return;
        }
        if (e.Key != Key.Escape) return;
        if (IsProbeLocked) UnlockProbe(); else ExitProbe(); e.Handled = true;
    }
}

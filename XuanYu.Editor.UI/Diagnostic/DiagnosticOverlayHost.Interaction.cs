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
        if (ProbeEnabled) SetProbeResult(DiagnosticProbeResolver.Resolve(hit, deepVisual));
    }

    public async Task ProbeClick()
    {
        if (!ProbeEnabled || _probeResult?.DeepVisual is not Control target) return;
        LockProbe();
        await _clipboard.SetTextAsync(target, DiagnosticElementFormatter.Format(_probeResult));
    }

    public void ExitProbe()
    {
        UnlockProbe();
        _vm?.ExitDiagnosticProbe();
        SetProbeResult(null);
    }

    void AttachProbeHandlers(TopLevel topLevel)
    {
        topLevel.AddHandler(InputElement.PointerMovedEvent, OnProbePointerMoved, RoutingStrategies.Tunnel, true);
        topLevel.AddHandler(InputElement.KeyDownEvent, OnProbeKeyDown, RoutingStrategies.Tunnel, true);
    }

    void DetachProbeHandlers(TopLevel topLevel)
    {
        topLevel.RemoveHandler(InputElement.PointerMovedEvent, OnProbePointerMoved);
        topLevel.RemoveHandler(InputElement.KeyDownEvent, OnProbeKeyDown);
    }

    void OnProbePointerMoved(object? sender, PointerEventArgs e)
    {
        if (!ProbeEnabled || e.Source is not Visual hit || IsOverlayVisual(hit) || ReferenceEquals(hit, _floatingLayer)) return;
        ProbeHover(hit, e.KeyModifiers.HasFlag(KeyModifiers.Alt));
    }

    static bool IsOverlayVisual(Visual hit) => hit is DiagnosticFloatingCard or DiagnosticDetailPanel or DiagnosticBadge;

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

using Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    void RestoreAfterOwnerActivation() => RestoreLockedToolWindow("OwnerActivated");

    void RestoreLockedToolWindow(string reason)
    {
        if (!ProbeEnabled || !IsProbeLocked || TrackedSnapshot is null) return;
        if (_topLevel is not Window owner || !owner.IsActive || owner.WindowState == WindowState.Minimized)
        {
            LogNativeDialog("RestoreDeferred", false);
            return;
        }
        if (_lockedProbeResult is null || !TryGetTrackedBounds(_lockedProbeResult, out var bounds)) return;
        LogNativeDialog($"{reason}Restore", true);
        _restoreOnOwnerActivation = false;
        _cardPlacementMode = DiagnosticCardPlacementMode.Auto;
        _toolWindow?.Show();
        if (_toolWindow is null)
        {
            RenderProbe();
            return;
        }
        PlaceToolWindow(bounds);
        ApplyToolPosition();
        _toolWindow.ReassertOwnedZOrder();
    }
}

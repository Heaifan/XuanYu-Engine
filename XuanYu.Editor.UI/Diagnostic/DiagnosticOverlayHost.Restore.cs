using Avalonia.Controls;
using Avalonia.Threading;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    int _ownerActivationRestoreAttempt;

    void RestoreAfterOwnerActivation() => TryRestoreAfterOwnerActivation(false);

    void TryRestoreAfterOwnerActivation(bool retry)
    {
        if (!ProbeEnabled || !IsProbeLocked || TrackedSnapshot is null) return;
        if (RestoreLockedToolWindow(retry ? "OwnerActivatedRetry" : "OwnerActivated")) return;
        if (_ownerActivationRestoreAttempt >= 2) return;
        _ownerActivationRestoreAttempt++;
        var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(75) };
        timer.Tick += (_, _) => { timer.Stop(); TryRestoreAfterOwnerActivation(true); };
        timer.Start();
    }

    bool RestoreLockedToolWindow(string reason)
    {
        if (!ProbeEnabled || !IsProbeLocked || TrackedSnapshot is null) return false;
        if (_topLevel is not Window owner || !owner.IsActive || owner.WindowState == WindowState.Minimized)
        {
            LogNativeDialog("RestoreDeferred", false);
            return false;
        }
        if (_lockedProbeResult is null || !TryGetTrackedBounds(_lockedProbeResult, out var bounds)) return false;
        LogNativeDialog($"{reason}Restore", true);
        _restoreOnOwnerActivation = false;
        _cardPlacementMode = DiagnosticCardPlacementMode.Auto;
        _toolWindow?.Show();
        if (_toolWindow is null)
        {
            RenderProbe();
            return true;
        }
        PlaceToolWindow(bounds);
        ApplyToolPosition();
        _toolWindow.ReassertOwnedZOrder();
        return true;
    }
}

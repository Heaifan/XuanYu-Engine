using Avalonia.Controls;
using Avalonia.Threading;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    DispatcherTimer? _ownerActivationRestoreTimer;

    void RestoreAfterOwnerActivation() => TryRestoreAfterOwnerActivation(false);

    void TryRestoreAfterOwnerActivation(bool retry)
    {
        if (!ProbeEnabled || !IsProbeLocked || TrackedSnapshot is null)
        {
            StopOwnerActivationRestore();
            return;
        }
        if (RestoreLockedToolWindow(retry ? "OwnerActivatedRetry" : "OwnerActivated"))
        {
            StopOwnerActivationRestore();
            return;
        }
        if (_topLevel is not Window owner || owner.WindowState == WindowState.Minimized)
        {
            StopOwnerActivationRestore();
            return;
        }
        _ownerActivationRestoreTimer ??= CreateOwnerActivationRestoreTimer();
        if (!_ownerActivationRestoreTimer.IsEnabled) _ownerActivationRestoreTimer.Start();
    }

    DispatcherTimer CreateOwnerActivationRestoreTimer()
    {
        var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(75) };
        timer.Tick += (_, _) => TryRestoreAfterOwnerActivation(true);
        return timer;
    }

    void StopOwnerActivationRestore()
    {
        _ownerActivationRestoreTimer?.Stop();
        _ownerActivationRestoreTimer = null;
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

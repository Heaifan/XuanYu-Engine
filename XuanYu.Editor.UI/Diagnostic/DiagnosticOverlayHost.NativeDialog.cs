using XuanYu.Editor.UI.Diagnostic;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    bool _nativeDialogSuspended;
    bool _restoreAfterNativeDialog;
    bool _restoreOnOwnerActivation;
    DiagnosticCardPlacementMode? _suspendedPlacementMode;
    bool _restoringNativeDialog;

    public void SuspendForNativeDialog()
    {
        if (_nativeDialogSuspended) return;
        _nativeDialogSuspended = true;
        _restoreAfterNativeDialog = _toolWindow?.IsVisible == true;
        _suspendedPlacementMode = _cardPlacementMode;
        if (_restoreAfterNativeDialog) HideToolWindow();
    }

    public void RestoreAfterNativeDialog()
    {
        if (!_nativeDialogSuspended) return;
        var restore = _restoreAfterNativeDialog;
        var placement = _suspendedPlacementMode ?? DiagnosticCardPlacementMode.Auto;
        _nativeDialogSuspended = false;
        _restoreAfterNativeDialog = false;
        _restoreOnOwnerActivation = restore && ProbeEnabled && IsProbeLocked;
        _suspendedPlacementMode = null;
        if (!restore || !ProbeEnabled) return;
        if (IsProbeLocked && TrackedSnapshot is null) return;
        _restoringNativeDialog = true;
        _cardPlacementMode = placement;
        try { RenderProbe(); }
        finally
        {
            _restoringNativeDialog = false;
        }
    }

    void RestoreAfterOwnerActivation()
    {
        if (!_restoreOnOwnerActivation || !ProbeEnabled || !IsProbeLocked) return;
        _restoreOnOwnerActivation = false;
        if (_lockedProbeResult is null || !TryGetTrackedBounds(_lockedProbeResult, out var bounds)) return;
        _restoringNativeDialog = true;
        try { ShowToolWindow(bounds); }
        finally { _restoringNativeDialog = false; }
    }
}

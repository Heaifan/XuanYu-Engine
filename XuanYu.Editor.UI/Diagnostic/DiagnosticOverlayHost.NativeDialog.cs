using XuanYu.Editor.UI.Diagnostic;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    bool _nativeDialogSuspended;
    bool _restoreAfterNativeDialog;
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
        _suspendedPlacementMode = null;
        if (!restore || !ProbeEnabled || _toolWindow is null) return;
        if (IsProbeLocked && TrackedSnapshot is null) return;
        _restoringNativeDialog = true;
        _cardPlacementMode = placement;
        try { RenderProbe(); }
        finally
        {
            _restoringNativeDialog = false;
        }
    }
}

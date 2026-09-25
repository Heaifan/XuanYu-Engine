using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
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
        LogNativeDialog("Suspend", _restoreAfterNativeDialog);
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
        LogNativeDialog("Restore", restore);
        if (!restore || !ProbeEnabled) return;
        if (IsProbeLocked && TrackedSnapshot is null) return;
        _restoringNativeDialog = true;
        _cardPlacementMode = placement;
        try { RenderProbe(); }
        finally
        {
            _restoringNativeDialog = false;
        }
        if (_restoreOnOwnerActivation)
            Dispatcher.UIThread.Post(RestoreAfterOwnerActivation, DispatcherPriority.ApplicationIdle);
    }

    static void LogNativeDialog(string phase, bool restore)
    {
        Console.WriteLine($"{DateTime.Now:HH:mm:ss} 【诊断悬浮窗】{phase}；恢复={restore}");
    }

}

using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    DiagnosticFloatingToolWindow? _toolWindow;
    DiagnosticFloatingCard? _toolCard;
    Control? _toolTarget;
    string? _lastToolTarget;

    void UpdateToolWindow(DiagnosticElementSnapshot snapshot, Control? target, Rect bounds)
    {
        var owner = _topLevel as Window;
        if (owner is null) return;
        _toolTarget = target is not null && TopLevel.GetTopLevel(target) is not null ? target : this;
        if (_toolWindow is null) CreateToolWindow(owner, snapshot);
        else
        {
            _toolCard!.UpdateSnapshot(snapshot, CopyToolText, IsProbeLocked);
            if (!ReferenceEquals(_toolWindow.Content, _toolCard))
            {
                _toolWindow.ReplaceContent(_toolCard);
                _toolWindow.SetDragSurface(_toolCard.DragSurface, BeginToolDrag, EndToolDrag);
            }
        }
        if (!_toolWindow!.IsVisible) ShowToolWindow(bounds);
        var key = $"{snapshot.DebugId}|{snapshot.InstanceName}|{snapshot.Probe.ProbeMode}";
        if (_lastToolTarget == key) return;
        _lastToolTarget = key;
    }

    void CreateToolWindow(Window owner, DiagnosticElementSnapshot snapshot)
    {
        _toolWindow = new DiagnosticFloatingToolWindow(owner);
        _toolCard = new DiagnosticFloatingCard(snapshot, CopyToolText, IsProbeLocked);
        _toolCard.Expanded += OpenToolDetails;
        _toolCard.Closed += CloseToolWindow;
        _toolCard.PinToggled += ToggleToolLock;
        _toolWindow.ReplaceContent(_toolCard);
        _toolWindow.SetDragSurface(_toolCard.DragSurface, BeginToolDrag, EndToolDrag);
        _toolWindow.Closed += OnToolWindowClosed;
    }

    void ShowToolWindow(Rect target)
    {
        if (_toolWindow is null) return;
        if (!_restoringNativeDialog) _cardPlacementMode = DiagnosticCardPlacementMode.Auto;
        _toolWindow.Show();
        Dispatcher.UIThread.Post(() => PlaceToolWindow(target));
    }

    void HideToolWindow()
    {
        if (_toolWindow?.IsVisible != true) return;
        _toolWindow.Hide();
    }

    void DisposeToolWindow()
    {
        if (_toolWindow is null) return;
        var tool = _toolWindow;
        _toolWindow = null; _toolCard = null; _toolTarget = null;
        _nativeDialogSuspended = false;
        _restoreAfterNativeDialog = false;
        _suspendedPlacementMode = null;
        tool.Close();
    }

    void OnToolWindowClosed(object? sender, EventArgs e)
    {
        _toolWindow = null;
        _nativeDialogSuspended = false;
        _restoreAfterNativeDialog = false;
        _suspendedPlacementMode = null;
    }
    void CloseToolWindow() => ExitProbe();
    void ToggleToolLock() { if (IsProbeLocked) UnlockProbe(); else LockProbe(); }
    Task CopyToolText(string text) => _toolTarget is null ? Task.CompletedTask : _clipboard.SetTextAsync(_toolTarget, text);
}

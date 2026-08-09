using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;

namespace XuanYu.Editor.UI;

public partial class UiWin : Window
{
    UiVm? _attachedVm;
    bool _allowClosing;

    public UiWin()
    {
        InitializeComponent();
        AddHandler(KeyDownEvent, Window_KeyDown, RoutingStrategies.Tunnel);
        DataContextChanged += (_, _) => AttachVm();
        Deactivated += (_, _) => (DataContext as UiVm)?.CancelInteractionFromWindowDeactivated();
    }

    protected override async void OnClosing(WindowClosingEventArgs e)
    {
        if (_allowClosing || DataContext is not UiVm vm || !vm.IsSceneDirty)
        {
            (DataContext as UiVm)?.CancelInteractionFromWindowClosing();
            base.OnClosing(e);
            return;
        }
        e.Cancel = true;
        var proceed = await ConfirmUnsavedBeforeContinue(vm);
        if (!proceed) return;
        vm.CancelInteractionFromWindowClosing();
        _allowClosing = true;
        Close();
    }

    async Task<bool> CopySelectedLogs(UiVm vm)
    {
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard is null) return false;
        try { await clipboard.SetTextAsync(vm.SelectedEntriesClipboardText); }
        catch (Exception ex) { Debug.WriteLine($"[LogList] window copy failed: {ex}"); return false; }
        vm.NotifyLogCopied();
        return true;
    }

    void AttachVm()
    {
        if (_attachedVm is not null)
        {
            _attachedVm.FileCommandRequested -= OnFileCommandRequested;
            _attachedVm.DangerousCommandConfirmRequested -= OnDangerousCommandRequested;
        }
        _attachedVm = DataContext as UiVm;
        if (_attachedVm is null) return;
        _attachedVm.FileCommandRequested += OnFileCommandRequested;
        _attachedVm.DangerousCommandConfirmRequested += OnDangerousCommandRequested;
        Title = _attachedVm.DocumentWindowTitle;
        _attachedVm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(UiVm.DocumentWindowTitle)) Title = _attachedVm.DocumentWindowTitle;
        };
    }

    async void OnFileCommandRequested(string command) => await RunSceneCommand(command);
}

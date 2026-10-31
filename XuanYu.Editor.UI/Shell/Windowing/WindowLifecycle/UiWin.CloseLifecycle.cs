using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;

namespace XuanYu.Editor.UI;

public partial class UiWin
{
    protected override void OnClosing(WindowClosingEventArgs e)
    {
        var vm = DataContext as UiVm;
        if (_allowClosing || vm is null || !vm.IsSceneDirty)
        {
            vm?.CancelInteractionFromWindowClosing();
            base.OnClosing(e);
            return;
        }
        e.Cancel = true;
        if (_closePromptActive)
            return;
        _closePromptActive = true;
        Dispatcher.UIThread.Post(() =>
        {
            _ = ConfirmCloseAsync(vm);
        });
    }

    async Task ConfirmCloseAsync(UiVm vm)
    {
        try
        {
            var proceed = await ConfirmUnsavedBeforeContinue(vm);
            if (!proceed) return;
            vm.CancelInteractionFromWindowClosing();
            _allowClosing = true;
            Close();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[WindowClose] confirmation failed: {ex}");
            CompleteDialog("cancel");
        }
        finally
        {
            _closePromptActive = false;
        }
    }
}

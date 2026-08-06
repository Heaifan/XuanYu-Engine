using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;

namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D5：弹窗宿主（普通确认/危险操作/未保存确认）。
// 危险按钮不是默认焦点；Enter 触发默认按钮（非危险）；Escape 取消。
public partial class UiWin
{
    TaskCompletionSource<string>? _dialogTcs;
    Button? _dialogDefault;

    Task<string> ShowDialogCore(string title, string message,
        (string Text, bool Danger, string Value)[] buttons, string defaultValue)
    {
        var tcs = _dialogTcs = new TaskCompletionSource<string>();
        DialogTitle.Text = title;
        DialogMessage.Text = message;
        DialogButtons.Children.Clear();
        _dialogDefault = null;
        foreach (var (text, danger, value) in buttons)
        {
            var button = new Button
            {
                Content = text,
                MinWidth = 0,
                Classes = { danger ? "uiDanger" : "uiTextButton" }
            };
            button.Click += (_, _) => CompleteDialog(value);
            DialogButtons.Children.Add(button);
            if (value == defaultValue) _dialogDefault = button;
        }
        DialogOverlay.IsVisible = true;
        DialogCard.IsVisible = true;
        // 默认焦点落在非危险默认按钮；危险弹窗的默认值必须是取消/非危险按钮
        Dispatcher.UIThread.Post(() => _dialogDefault?.Focus());
        return tcs.Task;
    }

    void CompleteDialog(string value)
    {
        DialogOverlay.IsVisible = false;
        DialogCard.IsVisible = false;
        _dialogTcs?.TrySetResult(value);
    }

    void DialogCard_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape) { CompleteDialog("cancel"); e.Handled = true; return; }
        if (e.Key != Key.Enter || _dialogDefault is null) return;
        CompleteDialog((string)_dialogDefault.Content!); // Enter=默认按钮（非危险）
        e.Handled = true;
    }

    public Task<string> ShowMessage(string title, string message) =>
        ShowDialogCore(title, message, [("确定", false, "ok")], "ok");

    public Task<string> ShowConfirm(string title, string message) =>
        ShowDialogCore(title, message, [("确定", false, "ok"), ("取消", false, "cancel")], "ok");

    public Task<string> ShowDanger(string title, string message) =>
        ShowDialogCore(title, message, [("取消", false, "cancel"), ("继续", true, "ok")], "cancel");

    // D5：危险操作统一走弹窗确认（危险按钮非默认焦点，Enter=取消）
    async void OnDangerousCommandRequested(string name)
    {
        if (_attachedVm is null) return;
        var message = name switch
        {
            "删除图层" => "删除图层将移除该图层及其中的对象，此操作不可撤销。是否继续？",
            _ => $"执行「{name}」将丢弃相关修改且不可撤销。是否继续？"
        };
        if (await ShowDanger("危险操作", message) == "ok")
            _attachedVm.ConfirmDangerousCommand(name);
    }
}

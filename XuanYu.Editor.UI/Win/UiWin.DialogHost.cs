using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;

namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D5（纠偏）：弹窗宿主（普通确认/危险操作/未保存确认）。
// 危险按钮不是默认焦点；Enter 触发默认按钮（非危险）；Escape 取消；
// Tab/Shift+Tab 焦点陷阱（不离开弹窗）；关闭后焦点返回原操作控件。
public partial class UiWin
{
    TaskCompletionSource<string>? _dialogTcs;
    Button? _dialogDefault;
    IInputElement? _focusBeforeDialog;

    IInputElement? CurrentFocus() => FocusManager?.GetFocusedElement();

    Task<string> ShowDialogCore(string title, string message,
        (string Text, bool Danger, string Value)[] buttons, string defaultValue, string iconKey = "")
    {
        var tcs = _dialogTcs = new TaskCompletionSource<string>();
        _focusBeforeDialog = CurrentFocus(); // 打开前焦点（关闭后返回）
        DialogTitle.Text = title;
        DialogMessage.Text = message;
        if (string.IsNullOrEmpty(iconKey)
            || !TryGetResource(iconKey, null, out var iconObj)
            || iconObj is not Avalonia.Media.StreamGeometry icon)
        {
            DialogIcon.IsVisible = false;
        }
        else
        {
            DialogIcon.Data = icon;
            DialogIcon.IsVisible = true;
        }
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
        var tcs = _dialogTcs;
        _dialogTcs = null;
        DialogOverlay.IsVisible = false;
        DialogCard.IsVisible = false;
        tcs?.TrySetResult(value);
        // D5 纠偏：关闭后焦点返回原操作控件
        Dispatcher.UIThread.Post(() => _focusBeforeDialog?.Focus());
    }
    public Task<string> ShowMessage(string title, string message) =>
        ShowDialogCore(title, message, [("确定", false, "ok")], "ok");

    public Task<string> ShowConfirm(string title, string message) =>
        ShowDialogCore(title, message, [("确定", false, "ok"), ("取消", false, "cancel")], "ok");

    // D5 纠偏：危险弹窗按钮写具体动作（如「删除图层」），不以「继续」代替
    public Task<string> ShowDanger(string title, string message, string actionText = "继续") =>
        ShowDialogCore(title, message, [("取消", false, "cancel"), (actionText, true, "ok")], "cancel");

}

using System.Threading.Tasks;

namespace XuanYu.Editor.UI;

// D4/D5（纠偏）：UiWin 错误/警告/重试弹窗实现——全部宿主化到 DialogHost（不再构建 Window）。
// 错误带 Error 图标（非仅颜色），重试弹窗返回用户选择（重试/取消）。
public sealed partial class UiWin : IEditorDialogService
{
    Task IEditorDialogService.ShowErrorAsync(string title, string message) =>
        ShowDialogCore(title, message, [("确定", false, "ok")], "ok", iconKey: "ErrorIcon");

    Task IEditorDialogService.ShowWarningAsync(string title, string message) =>
        ShowDialogCore(title, message, [("确定", false, "ok")], "ok", iconKey: "WarningIcon");

    Task<bool> IEditorDialogService.ShowRetryAsync(string title, string message) =>
        ShowRetryAsync(title, message);

    async Task<bool> ShowRetryAsync(string title, string message)
    {
        var choice = await ShowDialogCore(title, message,
            [("重试", false, "retry"), ("取消", false, "cancel")], "retry", iconKey: "ErrorIcon");
        return choice == "retry";
    }
}

namespace XuanYu.Editor.UI;

// D4：无窗口环境的空实现（测试 / 无 UI 宿主），避免 NRE。
sealed class NullEditorDialogService : IEditorDialogService
{
    public Task ShowErrorAsync(string title, string message) => Task.CompletedTask;

    public Task ShowWarningAsync(string title, string message) => Task.CompletedTask;
}

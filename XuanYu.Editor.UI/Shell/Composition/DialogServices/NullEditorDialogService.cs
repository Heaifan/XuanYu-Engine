using System.Threading.Tasks;

namespace XuanYu.Editor.UI;

// D4：无窗口环境的空实现（测试 / 无 UI 宿主），避免 NRE。
// D5（纠偏）：ShowRetryAsync 返回 false——无确认宿主时不自动重试（fail-safe）。
sealed class NullEditorDialogService : IEditorDialogService
{
    public Task ShowErrorAsync(string title, string message) => Task.CompletedTask;

    public Task ShowWarningAsync(string title, string message) => Task.CompletedTask;

    public Task<bool> ShowRetryAsync(string title, string message) => Task.FromResult(false);
}
